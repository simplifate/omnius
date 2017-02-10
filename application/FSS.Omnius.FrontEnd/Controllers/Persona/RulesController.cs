using System.Web.Mvc;

namespace FSS.Omnius.FrontEnd.Controllers.Persona
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
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