using System.Linq;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Controllers.Hermes
{
    [PersonaAuthorize(Roles = "Admin", Module = "Hermes")]
    public class LogController : Controller
    {
        public ActionResult Detail(int id)
        {
            DBEntities e = new DBEntities();

            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();

            EmailLog item = e.EmailLogItems.Single(m => m.Id == id);

            JToken mail = JToken.Parse(item.Content);

            ViewData["Id"] = item.Id;
            ViewData["Content"] = mail["Body"];
            ViewData["From_Name"] = mail["From"]["DisplayName"];
            ViewData["From_Email"] = mail["From"]["Address"];
            ViewData["Subject"] = mail["Subject"];
            ViewData["To"] = mail["To"];
            ViewData["Bcc"] = mail["Bcc"];
            ViewData["CC"] = mail["CC"];
            ViewData["Date_Send"] = item.DateSend.ToLongDateString();
            ViewData["Status"] = item.Status == EmailSendStatus.success ? "Odesláno" : "Neodesláno";
            ViewData["SMTP_Error"] = item.SMTP_Error;
            ViewData["Is_HTML"] = (bool)mail["IsBodyHtml"];

            return View("~/Views/Hermes/Log/Detail.cshtml");
        }
    }
}