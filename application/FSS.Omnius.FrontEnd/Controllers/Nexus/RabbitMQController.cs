using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using System.Collections.Generic;
using System;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Web.Helpers;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Nexus.Service;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Nexus")]
    public class RabbitMQController : Controller
    {
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;
            
            ViewData["LdapServersCount"] = e.Ldaps.Count();
            ViewData["WebServicesCount"] = e.WSs.Count();
            ViewData["ExtDatabasesCount"] = e.ExtDBs.Count();
            ViewData["WebDavServersCount"] = e.WebDavServers.Count();
            ViewData["APICount"] = e.APIs.Count();
            ViewData["TCPSocketCount"] = e.TCPListeners.Count();
            ViewData["RabbitMQCount"] = e.RabbitMQs.Count();

            return View(e.RabbitMQs.ToList());
        }

        #region configuration methods

        public ActionResult Create()
        {
            DBEntities e = DBEntities.instance;
            
            List<SelectListItem> appList = new List<SelectListItem>();
            foreach(Application a in e.Applications.OrderBy(a => a.Name)) {
                appList.Add(new SelectListItem() { Value = a.Id.ToString(), Text = a.Name });
            }
            ViewData["ApplicationList"] = appList;

            return View("~/Views/Nexus/RabbitMQ/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(RabbitMQ model, int? id)
        {
            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid)
            {
                // Záznam ji. existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    RabbitMQ row = e.RabbitMQs.Single(m => m.Id == model.Id);
                    e.Entry(row).CurrentValues.SetValues(model);
                }
                else
                {
                    e.RabbitMQs.Add(model);
                    e.SaveChanges();
                }

                e.SaveChanges();

                if (model.Type == ChannelType.RECEIVE) {
                    RabbitMQListenerService.AddListener(model);
                }

                return RedirectToRoute("Nexus", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Nexus/RabbitMQ/Form.cshtml", model);
            }
        }
        
        public ActionResult Edit(int? id)
        {
            DBEntities e = DBEntities.instance;
            RabbitMQ model = e.RabbitMQs.Single(l => l.Id == id);

            List<SelectListItem> appList = new List<SelectListItem>();
            foreach (Application a in e.Applications.OrderBy(a => a.Name)) {
                appList.Add(new SelectListItem() { Value = a.Id.ToString(), Text = a.Name, Selected = model.ApplicationId == a.Id });
            }
            
            ViewData["ApplicationList"] = appList;
            
            return View("~/Views/Nexus/RabbitMQ/Form.cshtml", model);
        }

        public ActionResult Delete(int? id)
        {
            DBEntities e = DBEntities.instance;
            RabbitMQ row = e.RabbitMQs.Single(l => l.Id == id);

            if (row == null)
                throw new Exception("Došlo k neoèekávané chybì");

            e.RabbitMQs.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Nexus", new { @action = "Index" });
        }
        
        #endregion

        #region tools

        public ActionResult LoadBlockList(int appId, string selectedBlockName)
        {
            DBEntities e = DBEntities.instance;
            TapestryDesignerMetablock root = e.TapestryDesignerMetablocks.Where(b => b.ParentAppId == appId && b.ParentMetablock_Id == null).FirstOrDefault();

            BlockJsonResponse blockList = LoadFromMetablock(root, 0, selectedBlockName);   
            
            return Json(blockList);
        }

        public ActionResult LoadWorkflowList(string blockName, int appId, string selectedWorkflowName)
        {
            DBEntities e = DBEntities.instance;
            List<SelectListItem> result = new List<SelectListItem>();

            TapestryDesignerBlock block = e.TapestryDesignerBlocks.Where(b => b.Name == blockName && b.ParentMetablock.ParentAppId == appId).SingleOrDefault();
            if(block != null) {
                foreach(TapestryDesignerWorkflowRule wf in block.BlockCommits.OrderByDescending(c => c.Id).First().WorkflowRules) {
                    foreach(TapestryDesignerSwimlane sl in wf.Swimlanes) {
                        foreach(TapestryDesignerWorkflowItem wi in sl.WorkflowItems.Where(i => i.SymbolType == "circle-event")) {
                            result.Add(new SelectListItem()
                            {
                                Text = wf.Name,
                                Value = wi.Label,
                                Selected = wi.Label == selectedWorkflowName
                            });
                        }
                    }
                }
            }

            return Json(result.OrderBy(i => i.Text).ToList());
        }

        private BlockJsonResponse LoadFromMetablock(TapestryDesignerMetablock parent, int level, string selectedBlockName)
        {
            BlockJsonResponse item = new BlockJsonResponse()
            {
                Name = parent.Name,
                Value = "",
                Selected = false,
                IsMetablock = true,
                Level = level,
                ChildBlocks = new List<BlockJsonResponse>()
            };

            foreach(TapestryDesignerBlock block in parent.Blocks.OrderBy(b => b.Name)) {
                item.ChildBlocks.Add(new BlockJsonResponse()
                {
                    Name = block.Name,
                    Value = block.Name,
                    Selected = block.Name == selectedBlockName,
                    IsMetablock = false,
                    Level = level + 1,
                    ChildBlocks = null
                });
            }
            foreach(TapestryDesignerMetablock mBlock in parent.Metablocks.OrderBy(m => m.Name)) {
                item.ChildBlocks.Add(LoadFromMetablock(mBlock, level + 1, selectedBlockName));
            }
            
            return item;
        }



        #endregion
    }
}