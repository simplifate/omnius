using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FSS.Omnius.Controllers.Hermes
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
    public class TemplateController : Controller
    {
        public static readonly Dictionary<int, string> LanguageList = new Dictionary<int, string>
        {
            {1, "Čeština" },
            {2, "English" }
        };

        // GET: SMTP
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            ViewData["IncomingEmailCount"] = e.IncomingEmail.Count();
            return View(e.EmailTemplates);
        }

        #region configuration methods

        public ActionResult Create()
        {
            DBEntities e = DBEntities.instance;
            ViewData["ApplicationList"] = e.Applications;
            return View("~/Views/Hermes/Template/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(EmailTemplate model)
        {
            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    EmailTemplate row = e.EmailTemplates.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.AppId = model.AppId;
                    row.Is_HTML = model.Is_HTML;

                    e.SaveChanges();
                }
                else
                {
                    e.EmailTemplates.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("Hermes", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Hermes/Template/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Hermes/Template/Detail.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = DBEntities.instance;
            ViewData["ApplicationList"] = e.Applications;

            return View("~/Views/Hermes/Template/Form.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            EmailTemplate row = e.EmailTemplates.Single(m => m.Id == id);

            e.EmailTemplates.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        public ActionResult EditContent(int id)
        {
            DBEntities e = DBEntities.instance;
            EmailTemplate template = e.EmailTemplates.Single(m => m.Id == id);

            ViewData["LanguageList"] = LanguageList;

            return View("~/Views/Hermes/Template/Content.cshtml", template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveContent(EmailTemplate model)
        {
            DBEntities e = DBEntities.instance;

            if (!model.Id.Equals(null))
            {
                EmailTemplate row = e.EmailTemplates.Single(m => m.Id == model.Id);

                NameValueCollection form = Request.Unvalidated.Form;
                foreach(KeyValuePair<int, string> lang in LanguageList)
                {
                    int langId = lang.Key;
                    string lId = langId.ToString();

                    EmailTemplateContent contentModel = row.ContentList.SingleOrDefault(c => c.LanguageId == langId);
                    bool exists = contentModel != null;

                    if (!exists)
                        contentModel = new EmailTemplateContent();

                    contentModel.LanguageId = langId;
                    contentModel.Hermes_Email_Template_Id = row.Id;
                    contentModel.From_Name = form["content.From_Name." + lId];
                    contentModel.From_Email = form["content.From_Email." + lId];
                    contentModel.Subject = form["content.Subject." + lId];
                    contentModel.Content = form["content.Content." + lId];
                    contentModel.Content_Plain = form["content.Content_Plain." + lId];
                    
                    if(!exists)
                        row.ContentList.Add(contentModel);
                }
                e.SaveChanges();
            }
            else
            {
                throw new System.Exception("Požadovaná šablona neexistuje.");
            }

            return RedirectToRoute("Hermes", new { @action = "Index" });            
        }

        public ActionResult Clone(int id)
        {
            DBEntities e = DBEntities.instance;
            ViewData["ApplicationList"] = e.Applications;
            ViewData["Action"] = "SaveClone";

            return View("~/Views/Hermes/Template/Form.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        [HttpPost]
        public ActionResult SaveClone(EmailTemplate model)
        {
            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid) {
                EmailTemplate row = e.EmailTemplates.Include("PlaceholderList").Include("ContentList").Single(m => m.Id == model.Id);

                // Naklonujeme šablonu
                EmailTemplate newRow = new EmailTemplate();
                newRow.Name = model.Name;
                newRow.AppId = model.AppId;
                newRow.Is_HTML = model.Is_HTML;
                
                e.EmailTemplates.Add(newRow);
                e.SaveChanges();

                // Naklonujeme proměnné
                foreach(EmailPlaceholder plc in row.PlaceholderList) {
                    EmailPlaceholder newPlc = new EmailPlaceholder();
                    newPlc.Description = plc.Description;
                    newPlc.Hermes_Email_Template_Id = newRow.Id;
                    newPlc.Num_Order = plc.Num_Order;
                    newPlc.Prop_Name = plc.Prop_Name;

                    e.EmailPlaceholders.Add(newPlc);
                }

                // Naklonujeme content
                foreach(EmailTemplateContent content in row.ContentList) {
                    EmailTemplateContent newContent = new EmailTemplateContent();
                    newContent.Content = content.Content;
                    newContent.Content_Plain = content.Content_Plain;
                    newContent.From_Email = content.From_Email;
                    newContent.From_Name = content.From_Name;
                    newContent.Hermes_Email_Template_Id = newRow.Id;
                    newContent.LanguageId = content.LanguageId;
                    newContent.Subject = content.Subject;

                    e.EmailContents.Add(newContent);
                }

                e.SaveChanges();
                
                return RedirectToRoute("Hermes", new { @action = "Index" });
            }
            else {
                return View("~/Views/Hermes/Template/Form.cshtml", model);
            }
        }


        #endregion

        #region tools



        #endregion
    }
}