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

        public override String GetFileHash(String virtualPath, IEnumerable virtualPathDependencies)
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
                DBEntities db = new DBEntities();
                return db.Pages.SingleOrDefault(x => x.ViewPath == virtualPath);
            }
            else
            {
                return null;
            }            
        }
    }
}