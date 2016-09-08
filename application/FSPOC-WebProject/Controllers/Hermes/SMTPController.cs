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
    public class SMTPController : Controller
    {
        [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
        // GET: SMTP
        public ActionResult Index()
        {
            DBEntities e = DBEntities.instance;
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
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
            DBEntities e = DBEntities.instance;
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
            DBEntities e = DBEntities.instance;
            return View("~/Views/Hermes/SMTP/Detail.cshtml", e.SMTPs.Single(m => m.Id == id));
        }

        public ActionResult Edit(int id)
        {
            DBEntities e = DBEntities.instance;
            return View("~/Views/Hermes/SMTP/Form.cshtml", e.SMTPs.Single(m => m.Id == id));
        }

        public ActionResult Delete(int id)
        {
            DBEntities e = DBEntities.instance;
            Smtp row = e.SMTPs.Single(m => m.Id == id);

            e.SMTPs.Remove(row);
            e.SaveChanges(); 

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        #endregion

        #region tools

        public ActionResult Test()
        {
            DBEntities e = DBEntities.instance;
            
            Dictionary<string, object> model = new Dictionary<string, object>();
            model.Add("count", e.WSs.Count());
            model.Add("ws", e.WSs);

            FileMetadata file = e.FileMetadataRecords.First();

            Mailer mail = new Mailer("Test", "Seznam WS", model);
            mail.To("martin.novak@futuresolutionservices.com", "Martin Novák");
            mail.Attachment(new KeyValuePair<int, string>(file.Id, file.Filename));
            mail.SendMail();

            ViewData["result"] = "OK";

            return View("~/Views/Hermes/SMTP/Test.cshtml");
        }

        public ActionResult TestSender()
        {
            DBEntities e = DBEntities.instance;
            /*Modules.Entitron.Entity.Nexus.Ldap m = e.Ldaps.Single(l => l.Is_Default == true);
            
            Mailer mail = new Mailer("Test", "Založení AD serveru", m);
            mail.To("martin.novak@futuresolutionservices.com", "Martin Novák");
            mail.Attachment("c:\\Users\\mnvk8\\Pictures\\Wallpapers\\world2.jpg");*/

            FileMetadata file = e.FileMetadataRecords.First();

            Dictionary<string, object> model = new Dictionary<string, object>();
            model.Add("count", e.WSs.Count());
            model.Add("ws", e.WSs);

            Mailer mail = new Mailer("Test", "Seznam WS", model);
            mail.To("martin.novak@futuresolutionservices.com", "Martin Novák");
            mail.Attachment(new KeyValuePair<int, string>(1, "neexistujici-soubor.doc"));
            mail.SendBySender();

            ViewData["result"] = "OK";

            return View("~/Views/Hermes/SMTP/Test.cshtml");
        }
       
        #endregion
    }
}