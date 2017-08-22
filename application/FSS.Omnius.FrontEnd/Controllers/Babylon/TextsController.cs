using System.Web.Mvc;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Data.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using i18n.Domain.Concrete;
using System.Collections.Generic;
using System.Linq;
using System;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Controllers.Babylon
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Babylon")]
    public class TextsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Generate()
        {
            string fileName = "messages.pot";

            DBEntities db = DBEntities.instance;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Page row in db.Pages) {
                data.Add(row.Id.ToString() + "-" + row.ViewName, row.ViewContent);
            }

            foreach (Application app in db.Applications) {
                foreach (TapestryDesignerMetablock mb in db.TapestryDesignerMetablocks.Where(mb => mb.ParentAppId == app.Id)) {
                    foreach (TapestryDesignerBlock b in mb.Blocks) {
                        TapestryDesignerBlockCommit lastCommit = b.BlockCommits.OrderByDescending(c => c.Id).FirstOrDefault();
                        if (lastCommit != null) {
                            foreach (TapestryDesignerWorkflowRule wf in lastCommit.WorkflowRules) {
                                List<string> messages = new List<string>();
                                foreach (TapestryDesignerSwimlane sl in wf.Swimlanes) {
                                    foreach (TapestryDesignerWorkflowItem item in sl.WorkflowItems) {
                                        if (item.ActionId == 182) { // Show message action
                                            string[] vars = item.InputVariables.Split(';');
                                            foreach(string v in vars) {
                                                string[] kv = v.Split('=');
                                                if(kv.Length == 2 && kv[0] == "Message") {
                                                    if(kv[1].StartsWith("s$")) {
                                                        messages.Add($"t._(\"{kv[1].Substring(2)}\")");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if(messages.Count > 0) {
                                    data.Add($"APP {app.Name} / Blok {b.Name} / Workflow {wf.Name}", string.Join(" ", messages));
                                }
                            }
                        }
                    }
                }
            }

            var settings = new i18nSettings(new WebConfigSettingService());
            var repository = new POTranslationRepository(settings);

            var nuggetFinder = new StringNuggetFinder(settings, data);
            var items = nuggetFinder.ParseAll();

            byte[] potContent = repository.GetTemplateForDownload(items);
                
            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(potContent);
            Response.End();
            
            return View("Index");
        }
        public ActionResult GeneratePot(int id)
        {
            string fileName = "messages.pot";
            DBEntities db = DBEntities.instance;
            string appName = db.Applications.SingleOrDefault(a => a.Id == id).Name;
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (Page row in db.Pages)
            {
                if ((row.ViewPath.Contains(appName + "\\Page")) || (row.ViewPath.Contains(appName + "\\menuLayout.cshtml")) || (row.ViewPath.ToString().StartsWith("/Views/App/") && Convert.ToInt32(row.ViewPath.ToString().Split('/')[3]) == id))
                    data.Add(row.Id.ToString() + "-" + row.ViewName, row.ViewContent);
            }

            var settings = new i18nSettings(new WebConfigSettingService());
            var repository = new POTranslationRepository(settings);

            var nuggetFinder = new StringNuggetFinder(settings, data);
            var items = nuggetFinder.ParseAll();

            byte[] potContent = repository.GetTemplateForDownload(items);

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(potContent);
            Response.End();

            return View("Index");
        }
    }
}
