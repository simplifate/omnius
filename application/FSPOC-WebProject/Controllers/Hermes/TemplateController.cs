using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace FSS.Omnius.Controllers.Hermes
{
    [PersonaAuthorize(Roles = "Admin")]
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
            DBEntities e = new DBEntities();
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            return View(e.EmailTemplates);
        }

        #region configuration methods

        public ActionResult Create()
        {
            return View("~/Views/Hermes/Template/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(EmailTemplate model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    EmailTemplate row = e.EmailTemplates.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
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
            DBEntities e = new DBEntities();
            return View("~/Views/Hermes/Template/Detail.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Hermes/Template/Form.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            EmailTemplate row = e.EmailTemplates.Single(m => m.Id == id);

            e.EmailTemplates.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        public ActionResult EditContent(int id)
        {
            DBEntities e = new DBEntities();
            EmailTemplate template = e.EmailTemplates.Single(m => m.Id == id);

            ViewData["LanguageList"] = LanguageList;

            return View("~/Views/Hermes/Template/Content.cshtml", template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveContent(EmailTemplate model)
        {
            DBEntities e = new DBEntities();

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

        #endregion

        #region tools



        #endregion
    }
}