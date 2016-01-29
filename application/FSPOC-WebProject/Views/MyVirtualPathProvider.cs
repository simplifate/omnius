using System;
using System.Collections;
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

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {

            var view = GetViewFromDatabase(virtualPath);

            if (view != null)
            {
                return null;
            }

            return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }
        //zde budou rozřazeny pohledy vzniklé v mozaiku od ostatních 
        private Page GetViewFromDatabase(string virtualPath)
        {
            virtualPath = virtualPath.Replace("~", "");

            //je zde zatím nějaká blbost
            //je to protože zatím není známo jaký rozlišovacími znaky budou mít pohledy z mozaicu
            if (virtualPath.StartsWith("kjhdaskd"))
            {
                DBEntities db = new DBEntities();
                //var view = from v in db.Pages
                //where v.ViewPath == virtualPath
                //select v;
                var view = db.Pages.SingleOrDefault(x => x.ViewPath == virtualPath);
                return view;
            }
            else
            {
                return null;
            }            
        }
    }
}