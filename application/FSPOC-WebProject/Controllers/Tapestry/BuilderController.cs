using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class BuilderController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            if (Request.HttpMethod == "POST")
            {
                using (var context = new DBEntities())
                {
                    int blockId = int.Parse(formParams["blockId"]);
                    var parentMetablock = context.TapestryDesignerBlocks.Include("ParentMetablock")
                        .Where(c => c.Id == blockId).First().ParentMetablock;
                    ViewData["blockId"] = blockId;
                    if (parentMetablock == null)
                        ViewData["parentMetablockId"] = 0;
                    else
                        ViewData["parentMetablockId"] = parentMetablock.Id;
                }
            }
            else // TODO: remove after switching to real IDs
            {
                ViewData["blockId"] = 1;
                ViewData["parentMetablockId"] = 1;
            }
            return View();
        }
    }
}
