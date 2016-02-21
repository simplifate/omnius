using System.Web.Mvc;
using FSS.Omnius.Modules.Nexus.Service;

namespace FSS.Omnius.Controllers.Nexus
{
    [PersonaAuthorize(Roles = "Admin", Module = "Nexus")]
    public class NexusController : Controller
    {
        public ActionResult Index()
        {
            NexusLdapService service = new NexusLdapService();

            //string test = service.searchByLogin("martin.novak");

            return View();
        }

    }
}
