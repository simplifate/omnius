using System.Web.Mvc;
using FSS.Omnius.BussinesObjects.Service;

namespace FSPOC_WebProject.Controllers
{
    public class NexusController : Controller
    {
        public ActionResult Index()
        {
            NexusService service = new NexusService();

            //string test = service.searchByLogin("martin.novak");

            return View();
        }

    }
}
