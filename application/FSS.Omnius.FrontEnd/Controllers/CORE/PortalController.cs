using FSS.Omnius.Modules.Persona;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.CORE
{
    [PersonaAuthorize(NeedsAdmin = true)]
    public class PortalController : Controller
    {
        // GET: Portal
        public ActionResult Index()
        {
            ViewBag.loggedUserCount = Persona.GetLoggedCount();

            return View();
        }
        public ActionResult ModuleAdmin()
        {
            return View();
        }
        public ActionResult UsersOnline()
        {
            return View();
        }
        public ActionResult ActiveProfile()
        {
            return View();
        }
        public ActionResult AppValidation()
        {
            return View();
        }
    }
}
