using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.IO;

namespace FSS.Omnius.FrontEnd.Views
{
    public class MyVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            var view = GetViewFromDatabase(virtualPath);

            if (view == false)
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

            if (view == false)
            {
                return base.GetFile(virtualPath);
            }
            else
            {
                //byte[] content = Encoding.UTF8.GetBytes(view.ViewContent);
                byte[] content = File.ReadAllBytes(virtualPath);
                return new MyVirtualFile(virtualPath, content);
            }
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            /*var view = GetViewFromDatabase(virtualPath);

            if (view != null)
            {
                return null;
            }*/

            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            /*var view = GetViewFromDatabase(virtualPath);

            if (view != null)
            {
                return Guid.NewGuid().ToString();
            }*/

            return Previous.GetFileHash(virtualPath, virtualPathDependencies);
        }


        //zde budou rozřazeny pohledy vzniklé v mozaiku od ostatních 
        private bool GetViewFromDatabase(string virtualPath)
        {
            if (File.Exists(virtualPath))
            {
                return true;
            }
            return false;
            /*
            virtualPath = virtualPath.Replace("~", "");

            // ve startwith je zatím nějaká blbost 
            //je to protože zatím není známo jaký rozlišovací cestu budou mít pohledy z mozaicu
            if (virtualPath.StartsWith("/Views/App/"))
            {
                DBEntities db = DBEntities.instance;
                return db.Pages.FirstOrDefault(x => x.ViewPath == virtualPath);
            }
            else
            {
                return null;
            } */
        }
    }
}
