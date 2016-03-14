using System.Web.Mvc;
using FSS.Omnius.Modules.Nexus.Service;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(Roles = "Admin", Module = "Nexus")]
    public class CortexController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
