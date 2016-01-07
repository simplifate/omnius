using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Hermes;

namespace FSS.Omnius.Controllers.Hermes
{
    public class SMTPController : Controller
    {
        // GET: SMTP
        public ActionResult Index()
        {
            DBEntities e = new DBEntities();
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            return View(e.SMTPs);
        }

        #region configuration methods

        public ActionResult Create()
        {
            return View("~/Views/Hermes/SMTP/Form.cshtml");
        }

        [HttpPost]
        public ActionResult Save(Smtp model)
        {
            DBEntities e = new DBEntities();
            if (ModelState.IsValid)
            {
                // Záznam již existuje - pouze upravujeme
                if (!model.Id.Equals(null))
                {
                    Smtp row = e.SMTPs.Single(m => m.Id == model.Id);
                    row.Name = model.Name;
                    row.Server = model.Server;
                    row.Auth_User = model.Auth_User;
                    row.Auth_Password = model.Auth_Password.Length > 0 ? model.Auth_Password : row.Auth_Password;
                    row.Use_SSL = model.Use_SSL;
                    row.Is_Default = model.Is_Default;

                    e.SaveChanges();
                }
                else
                {
                    e.SMTPs.Add(model);
                    e.SaveChanges();
                }
                return RedirectToRoute("Hermes", new { @action = "Index" });
            }
            else
            {
                return View("~/Views/Hermes/SMTP/Form.cshtml", model);
            }
        }

        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Hermes/SMTP/Detail.cshtml", e.SMTPs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = new DBEntities();
            return View("~/Views/Hermes/SMTP/Form.cshtml", e.SMTPs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = new DBEntities();
            Smtp row = e.SMTPs.Single(m => m.Id == id);

            e.SMTPs.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        #endregion

        #region tools

        public ActionResult Test()
        {
            DBEntities e = new DBEntities();
            //Modules.Entitron.Entity.Nexus.Ldap m = e.Ldaps.Single(l => l.Is_Default == true);

            Dictionary<string, object> model = new Dictionary<string, object>();
            model.Add("count", e.WSs.Count());
            model.Add("ws", e.WSs);

            Mailer mail = new Mailer("Test", "Seznam WS", model);
            mail.To("martin.novak@futuresolutionservices.com", "Martin Novák");
            mail.SendMail();

            ViewData["result"] = "OK";

            return View("~/Views/Hermes/SMTP/Test.cshtml");
        }
       
        #endregion
    }
}