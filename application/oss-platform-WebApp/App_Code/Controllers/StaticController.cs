using System.Collections.Generic;
using System.Web.Mvc;
using FSPOC.Models;

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
        [Route("tapestry")]
        public ActionResult Tapestry()
        {
            return View();
        }
        [Route("apps")]
        public ActionResult AppManager()
        {
            List<AppTile> appTiles = new List<AppTile>();

            appTiles.Add(new AppTile
            {
                Name = "Intranet",
                TileWidth = 2,
                TileHeight = 2,
                Icon = "fa-users",
                PositionX = 764,
                PositionY = 136,
                InnerHTML = "<span class='mediumFont'>V současnosti je online<br /><span class='highlight'>381</span> uživatelů</span>"
            });
            appTiles.Add(new AppTile
            {
                Name = "Spammer",
                TileWidth = 2,
                TileHeight = 2,
                Icon = "fa-stack-overflow",
                PositionX = 1004,
                PositionY = 136,
                InnerHTML = "<span class='mediumFont'>Odeslaných zpráv: <span class='highlight'>68</span></span>"
            });
            appTiles.Add(new AppTile { Name = "Trvalé zlepšování", Color = 0, TileWidth = 2, TileHeight = 1, Icon = "fa-child", PositionX = 764, PositionY = 376 });
            appTiles.Add(new AppTile
            {
                Name = "Aukční systém",
                TileWidth = 4,
                TileHeight = 4,
                Icon = "fa-money",
                PositionX = 284,
                PositionY = 136,
                InnerHTML = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
            });
            appTiles.Add(new AppTile { Name = "Passworder", Color = 1, TileWidth = 2, TileHeight = 1, Icon = "fa-lock", PositionX = 764, PositionY = 496 });
            appTiles.Add(new AppTile { Name = "App1", Color = 1, PositionX = 1004, PositionY = 376 });
            appTiles.Add(new AppTile { Name = "App2", Color = 2, PositionX = 1004, PositionY = 496 });

            ViewData["Tiles"] = appTiles;
            return View();
        }
    }
}
