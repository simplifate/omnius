using System.Web.Mvc;

namespace FSPOC_WebProject.Controllers.Persona
{
    [PersonaAuthorize(Roles = "Admin", Module = "Persona")]
    public class RulesController : Controller
    {
        // GET: Rules
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RuleDetail(int id)
        {
            ViewBag.DetailId = id;
            return View();
        }
    }
}