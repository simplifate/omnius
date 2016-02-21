using System.Linq;
using System.Web.Mvc;

using FSS.Omnius.Modules.Entitron.Entity;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class EditorController : Controller
    {
        // GET: Editor
        public ActionResult Index(FormCollection formParams)
        {
            using (var context = new DBEntities())
            {
                if (formParams["appId"] != null)
                {
                    int appId = int.Parse(formParams["appId"]);
                    ViewData["appId"] = appId;
                    ViewData["pageId"] = formParams["pageId"];
                    ViewData["appName"] = context.Applications.Find(appId).DisplayName;
                }
                else
                {
                    var firstApp = context.Applications.First();
                    ViewData["appId"] = firstApp.Id;
                    ViewData["pageId"] = 0;
                    ViewData["appName"] = firstApp.DisplayName;
                }

                return View();
            }
        }
    }
}
