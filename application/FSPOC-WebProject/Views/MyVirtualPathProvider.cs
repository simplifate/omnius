using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Collections.Generic;

namespace FSPOC_WebProject.Views
{
    public class MyVirtualPathProvider: VirtualPathProvider
    {
        private static List<MyVirtualPathProvider> _all = new List<MyVirtualPathProvider>();
        public static void ClearAllCache()
        {
            foreach(MyVirtualPathProvider provider in _all)
            {
                provider.ClearCache();
            }
        }

        private Dictionary<string, Page> _cache = new Dictionary<string, Page>();

        public MyVirtualPathProvider()
        {
            _all.Add(this);
        }
        public void ClearCache()
        {
            _cache.Clear();
        }

        public override bool FileExists(string virtualPath)
        {
            var view = GetViewFromDatabase(virtualPath);

            if (view == null)
            {
                return base.FileExists(virtualPath);
            }
            else
            {
                return true;
            }
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var view = GetViewFromDatabase(virtualPath);

            if (view == null)
            {
                return base.GetFile(virtualPath);
            }
            else
            {
                byte[] content = Encoding.UTF8.GetBytes(view.ViewContent);
                return new MyVirtualFile (virtualPath, content);
            }
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var view = GetViewFromDatabase(virtualPath);

            if (view != null)
            {
                return null;
            }

            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            var view = GetViewFromDatabase(virtualPath);

            if (view != null)
            {
                return Guid.NewGuid().ToString();
            }

            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }


        //zde budou rozřazeny pohledy vzniklé v mozaiku od ostatních 
        private Page GetViewFromDatabase(string virtualPath)
        {
            virtualPath = virtualPath.Replace("~", "");

            // ve startwith je zatím nějaká blbost 
            //je to protože zatím není známo jaký rozlišovací cestu budou mít pohledy z mozaicu
            if (virtualPath.StartsWith("/Views/App/"))
            {
                if (_cache.ContainsKey(virtualPath))
                    return _cache[virtualPath];

                DBEntities db = new DBEntities();
                Page page = db.Pages.FirstOrDefault(x => x.ViewPath == virtualPath);

                _cache.Add(virtualPath, page);
                return page;
            }

            return null;
        }
    }
}
