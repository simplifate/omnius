using System.Web.Mvc;

namespace FSPOC.Controllers
{
    public class StaticController : Controller
    {
        [Route("workflow")]
        [Route("~/", Name = "default")]
        public ActionResult WfDesigner()
        {
            return View();
        }
        [Route("database")]
        public ActionResult DbDesigner()
        {
            return View();
        }
        /* Coming soon...
        [Route("apps")]
        public ActionResult AppManager()
        {
            return View();
        }*/
        [Route("tapestry")]
        public ActionResult Tapestry()
        {
            return View();
        }
    }
}