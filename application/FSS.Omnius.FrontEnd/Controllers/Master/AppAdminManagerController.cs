using System;
using System.Linq;
using System.Web.Mvc;
using System.Net;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Master;
using Microsoft.Web.WebSockets;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Master")]
    public class AppAdminManagerController : Controller
    {
        public ActionResult Index()
        {
            var context = COREobject.i.Context;
            ViewData["Apps"] = context.Applications.ToList();
            return View();
        }
        public ActionResult BuildApp(int Id)
        {
            string menuPath = Server.MapPath("~/Views/Shared/_ApplicationMenu.cshtml");

            if (HttpContext.IsWebSocketRequest)
            {
                HttpContext.AcceptWebSocketRequest(new BuildWebSocketHandler(Id, menuPath));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.SwitchingProtocols;
                return null;
            }

            return RedirectToAction("Index");
        }
        public ActionResult RebuildApp(int Id)
        {
            string menuPath = Server.MapPath("~/Views/Shared/_ApplicationMenu.cshtml");

            if (HttpContext.IsWebSocketRequest)
            {
                HttpContext.AcceptWebSocketRequest(new BuildWebSocketHandler(Id, menuPath, true));
                HttpContext.Response.StatusCode = (int)HttpStatusCode.SwitchingProtocols;
                return null;
            }

            return RedirectToAction("Index");
        }
    }

    class BuildWebSocketHandler : WebSocketHandler
    {
        private int _AppId;
        private string _menuPath;
        private bool _rebuildInAction;

        public BuildWebSocketHandler(int appId, string menuPath, bool rebuilInAction = false)
        {
            _AppId = appId;
            _menuPath = menuPath;
            _rebuildInAction = rebuilInAction;
        }
        
        public override void OnOpen()
        {
            ApplicationBuilder builder = new ApplicationBuilder(Send);
            builder.Build(_AppId, _menuPath, _rebuildInAction);
            Close();
        }
    }
}
