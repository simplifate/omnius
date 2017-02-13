using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System.Collections.Generic;
using System;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Web.Helpers;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Controllers.Hermes
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
    public class IncomingRuleController : Controller
    {
        // GET: Placeholder list
        public ActionResult Index(int? mailboxId)
        {
            DBEntities e = DBEntities.instance;
            IncomingEmail mailbox = e.IncomingEmail.Single(m => m.Id == mailboxId);

            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            ViewData["IncomingEmailCount"] = e.IncomingEmail.Count();
            ViewData["MailboxName"] = mailbox.Name;
            ViewData["mailboxId"] = mailbox.Id;

            return View(mailbox.IncomingEmailRule.ToList());
        }

        #region configuration methods

        public ActionResult Create(int? mailboxId)
        {
            DBEntities e = DBEntities.instance;

            ViewData["mailboxId"] = mailboxId;

            List<SelectListItem> appList = new List<SelectListItem>();
            foreach(Application a in e.Applications.OrderBy(a => a.Name)) {
                appList.Add(new SelectListItem() { Value = a.Id.ToString(), Text = a.Name });
            }
            ViewData["ApplicationList"] = appList;

            return View("~/Views/Hermes/IncomingRule/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(IncomingEmailRule model, int? mailboxId, int? id)
        {
            if (mailboxId == null)
                throw new Exception("Do?lo k neoèekávané chybì");

            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid)
            {
                // Záznam ji. existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    IncomingEmailRule row = e.IncomingEmailRule.Single(m => m.Id == model.Id);
                    row.ApplicationId = model.ApplicationId;
                    row.IncomingEmailId = (int)mailboxId;
                    row.BlockName = model.BlockName;
                    row.WorkflowName = model.WorkflowName;
                    row.Rule = model.Rule;
                    row.Name = model.Name;

                    e.SaveChanges();
                }
                else
                {
                    model.IncomingEmailId = (int)mailboxId;
                    
                    e.IncomingEmailRule.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("HermesIncomingRule", new { @action = "Index", @mailboxId = mailboxId});
            }
            else
            {
                return View("~/Views/Hermes/IncomingRule/Form.cshtml", model);
            }
        }
        
        public ActionResult Edit(int? mailboxId, int? id)
        {
            DBEntities e = DBEntities.instance;
            IncomingEmailRule rule = e.IncomingEmail.Single(m => m.Id == mailboxId).IncomingEmailRule.Single(r => r.Id == id);

            List<SelectListItem> appList = new List<SelectListItem>();
            foreach (Application a in e.Applications.OrderBy(a => a.Name)) {
                appList.Add(new SelectListItem() { Value = a.Id.ToString(), Text = a.Name, Selected = rule.ApplicationId == a.Id });
            }

            ViewData["mailboxId"] = mailboxId;
            ViewData["ApplicationList"] = appList;
            
            return View("~/Views/Hermes/IncomingRule/Form.cshtml", rule);
        }

        public ActionResult Delete(int? mailboxId, int? id)
        {
            DBEntities e = DBEntities.instance;
            IncomingEmailRule row = e.IncomingEmail.Single(t => t.Id == mailboxId).IncomingEmailRule.Single(r => r.Id == id);

            if (row == null)
                throw new Exception("Do?lo k neoèekávané chybì");

            e.IncomingEmailRule.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("HermesIncomingRule", new { @action = "Index", @mailboxId = mailboxId });
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
                        foreach(TapestryDesignerWorkflowItem wi in sl.WorkflowItems.Where(i => i.SymbolType == "envelope-start")) {
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

    class BlockJsonResponse
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
        public bool IsMetablock { get; set; }
        public int Level { get; set; }
        public List<BlockJsonResponse> ChildBlocks { get; set; }
    }
}