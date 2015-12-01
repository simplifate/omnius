using System.Web.Mvc;
using FSS.Omnius.BussinesObjects.Service;

namespace FSS.Omnius.Controllers.Nexus
{
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
