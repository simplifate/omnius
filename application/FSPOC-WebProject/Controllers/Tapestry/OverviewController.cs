using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSPOC_WebProject.Controllers.Tapestry
{
    public class OverviewController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            if (Request.HttpMethod == "POST")
            {
                using(var context = new DBEntities())
                {
                    int metablockId = int.Parse(formParams["metablockId"]);
                    var parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock")
                        .Where(c => c.Id == metablockId).First().ParentMetablock;
                    ViewData["metablockId"] = metablockId;
                    if (parentMetablock == null)
                        ViewData["parentMetablockId"] = 0;
                    else
                        ViewData["parentMetablockId"] = parentMetablock.Id;
                }
            }
            else // TODO: remove after switching to real IDs
            {
                ViewData["metablockId"] = 1;
                ViewData["parentMetablockId"] = 0;
            }
            return View();
        }

    }
}
