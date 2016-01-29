using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;

namespace FSPOC_WebProject.Views
{
    public class MyVirtualPathProvider: VirtualPathProvider
    {
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
                byte[] content = ASCIIEncoding.ASCII.
                                 GetBytes(view.ViewContent);
                return new MyVirtualFile
                              (virtualPath, content);
            }
        }

        //public override CacheDependency GetCacheDependency
        // (string virtualPath, Enumerable virtualPathDependencies,
        //  DateTime utcStart)
        //{

        //    var view = GetViewFromDatabase(virtualPath);

        //    if (view != null)
        //    {
        //        return null;
        //    }

        //    return Previous.GetCacheDependency(virtualPath,
        //       virtualPathDependencies, utcStart);
        //}

        private Page GetViewFromDatabase(string virtualPath)
        {
            virtualPath = virtualPath.Replace("~", "");

            DBEntities db = new DBEntities();
            var view = from v in db.Pages
                       where v.ViewPath == virtualPath
                       select v;
            return view.SingleOrDefault();
        }
    }
}