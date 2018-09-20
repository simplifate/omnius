using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.FrontEnd;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Hermes;
using Microsoft.AspNet.Identity.Owin;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Persona;

namespace FSS.Omnius.Controllers.Hermes
{
    public class QueueController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ApplicationUserManager UserManager
        {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set {
                _signInManager = value;
            }
        }

        private string userName;

        // GET: Queue
        [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
        public ActionResult Index()
        {
            DBEntities e = COREobject.i.Context;
            ViewData["SMTPServersCount"] = e.SMTPs.Count();
            ViewData["EmailTemplatesCount"] = e.EmailTemplates.Count();
            ViewData["EmailQueueCount"] = e.EmailQueueItems.Count();
            ViewData["IncomingEmailCount"] = e.IncomingEmail.Count();
            return View(e.EmailQueueItems);
        }

        [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
        public ActionResult Detail(int id)
        {
            DBEntities e = COREobject.i.Context;
            
            return PartialView("~/Views/Hermes/Queue/Detail.cshtml", e.EmailQueueItems.Single(m => m.Id == id));
        }

        [PersonaAuthorize(NeedsAdmin = true, Module = "Hermes")]
        public ActionResult Delete(int id)
        {
            DBEntities e = COREobject.i.Context;
            EmailQueue row = e.EmailQueueItems.Single(m => m.Id == id);

            e.EmailQueueItems.Remove(row);
            e.SaveChanges();

            return RedirectToRoute("Hermes", new { @action = "Index" });
        }

        [AllowAnonymous]
        public ActionResult RunSender(string serverName = "")
        {
            bool isAuthorized = Authorize();
            bool redirect = true;

            if (!isAuthorized)
                return new Http403Result();

            COREobject core = COREobject.i;

            if (core.User == null) {
                redirect = false;
                core.User = Persona.GetAuthenticatedUser(userName, false, Request);
            }

            Mailer mailer = new Mailer(serverName);
            mailer.RunSender();

            if (redirect) {
                return RedirectToRoute("Hermes", new { @action = "Index" });
            }
            else {
                return new EmptyResult();
            }
        }

        private bool Authorize()
        {
            userName = HttpContext.Request.QueryString["User"];
            string password = HttpContext.Request.QueryString["Password"];

            var result = SignInManager.PasswordSignIn(userName, password, false, shouldLockout: false);
            if (result == SignInStatus.Success) {
                return true;
            }
            return false;
        }
    }
}