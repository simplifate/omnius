using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Hermes;
using System.Net.Mail;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;

namespace FSS.Omnius.Controllers.Hermes
{
    public class IncomingController : Controller
    {
        [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
        // GET: SMTP
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            ViewData["IncomingEmailCount"] = e.IncomingEmail.Count();

            return View(e.IncomingEmail);
        }

        #region configuration methods

        public ActionResult Create()
        {
            return View("~/Views/Hermes/Incoming/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(IncomingEmail model)
        {
            DBEntities e = DBEntities.instance;
            if (ModelState.IsValid)
            {
                // Záznam ji. existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    IncomingEmail row = e.IncomingEmail.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.ImapServer = model.ImapServer;
                    row.ImapPort = model.ImapPort;
                    row.UserName = model.UserName;
                    row.Password = model.Password;

                    e.SaveChanges();
                }
                else
                {
                    e.IncomingEmail.Add(model);
                    e.SaveChanges();
                }

                IncomingEmailListener.AddListener(model);

                return RedirectToRoute("Hermes", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Hermes/Incoming/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Hermes/Incoming/Detail.cshtml", e.IncomingEmail.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Hermes/Incoming/Form.cshtml", e.IncomingEmail.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            IncomingEmail row = e.IncomingEmail.Single(m => m.Id == id);

            IncomingEmailListener.RemoveListener(row.Name);

            e.IncomingEmail.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        #endregion

        #region tools

       
       
        #endregion
    }
}