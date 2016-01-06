using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;

namespace FSS.Omnius.Controllers.Hermes
{
    public class TemplateController : Controller
    {
        // GET: SMTP
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
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
            return View("~/Views/Hermes/Template/Content.cshtml", e.EmailTemplates.Single(m => m.Id == id));
        }

        [HttpPost]
        public ActionResult SaveContent(EmailTemplate model)
        {
            DBEntities e = new DBEntities();
            // Záznam již existuje - pouze upravujeme
            if (!model.Id.Equals(null))
            {
                EmailTemplate row = e.EmailTemplates.Single(m => m.Id == model.Id);
                //row.Content = model.Content;
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