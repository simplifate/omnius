using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Tapestry")]
    public class BuilderController : Controller
    {
        public ActionResult Index(FormCollection formParams)
        {
            JArray blockTree = JArray.Parse("[]");

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

                    ViewData["currentUserId"] = HttpContext.GetCORE().User.Id;
                    
                    GetBlockTree(context.TapestryDesignerMetablocks.FirstOrDefault(m => m.Id == rootMetablockId), ref blockTree, 0);
                }
            }
            else // TODO: remove after switching to real IDs
            {
                using (var context = DBEntities.instance) {
                    ViewData["appId"] = 1;
                    ViewData["blockId"] = 1;
                    ViewData["parentMetablockId"] = 1;
                    ViewData["currentUserId"] = HttpContext.GetCORE().User.Id;

                    GetBlockTree(context.TapestryDesignerMetablocks.FirstOrDefault(m => m.Id == 1), ref blockTree, 0);
                }
            }

            ViewData["blockTree"] = blockTree;
            return View();
        }

        private void GetBlockTree(TapestryDesignerMetablock metablock, ref JArray blockTree, int level)
        {
            JObject item = new JObject();
            item["Id"] = "";
            item["Name"] = metablock.Name;
            item["IsMetablock"] = true;
            item["Level"] = level;
            item["Items"] = JArray.Parse("[]");

            blockTree.Add(item);

            foreach(TapestryDesignerBlock block in metablock.Blocks) {
                JObject bi = new JObject();
                bi["Id"] = block.Id;
                bi["Name"] = block.Name;
                bi["IsMetablock"] = false;
                bi["Level"] = level;

                ((JArray)item["Items"]).Add(bi);
            }

            foreach(TapestryDesignerMetablock mb in metablock.Metablocks) {
                GetBlockTree(mb, ref blockTree, level + 1);
            }
        }
    }
}
