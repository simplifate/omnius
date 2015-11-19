using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class BuilderController : Controller
    {
        // GET: Builder
        public ActionResult Index()
        {
            return View("~/Views/Tapestry/Index.cshtml");
        }
    }
}
