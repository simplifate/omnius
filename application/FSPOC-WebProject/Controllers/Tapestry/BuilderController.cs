using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Tapestry")]
    public class BuilderController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            if (Request.HttpMethod == "POST")
            {
                using (var context = DBEntities.instance)
                {
                    int blockId = int.Parse(formParams["blockId"]);
                    var parentMetablock = context.TapestryDesignerBlocks.Include("ParentMetablock")
                        .First(c => c.Id == blockId).ParentMetablock;
                    ViewData["blockId"] = blockId;
                    if (parentMetablock == null)
                        ViewData["parentMetablockId"] = 0;
                    else
                        ViewData["parentMetablockId"] = parentMetablock.Id;

                    int appId = 0;
                    int rootMetablockId = parentMetablock.Id;
                    parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock").Include("ParentApp")
                                                                        .Where(c => c.Id == parentMetablock.Id).First();
                    while (parentMetablock != null)
                    {
                        rootMetablockId = parentMetablock.Id;
                        parentMetablock = context.TapestryDesignerMetablocks.Include("ParentMetablock").Include("ParentApp")
                            .Where(c => c.Id == parentMetablock.Id).First().ParentMetablock;
                    }
                    Application app = context.Applications.SingleOrDefault(a => a.TapestryDesignerMetablocks.Any(mb => mb.Id == rootMetablockId));

                    ViewData["appId"] = app != null ? app.Id : appId;
                    //ViewData["screenCount"] = context.TapestryDesignerBlocks.Find(blockId).Pages.Count();
                }
            }
            else // TODO: remove after switching to real IDs
            {
                ViewData["appId"] = 1;
                ViewData["blockId"] = 1;
                ViewData["parentMetablockId"] = 1;
            }
            return View();
        }
    }
}
