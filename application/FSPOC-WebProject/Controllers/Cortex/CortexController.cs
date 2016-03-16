using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Cortex;
using System.Data.Entity;
using System.Web.Mvc;

namespace FSS.Omnius.Controllers.Cortex
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Cortex")]
    public class CortexController : Controller
    {
        public ActionResult Index()
        {
            DBEntities context = new DBEntities();
            return View("~/Views/Cortex/Index.cshtml", context.Tasks);
        }

        public ActionResult Create()
        {
            return View("~/Views/Cortex/Form.cshtml");
        }
    }
}
