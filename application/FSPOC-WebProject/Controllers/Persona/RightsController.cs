using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    [PersonaAuthorize(Roles = "Admin")]
    public class RightsController : Controller
    {
        public ActionResult Index()
        {
            ADgroup.RefreshFromAD(new FSS.Omnius.Modules.CORE.CORE());
         
            return View();
        }
    }
}