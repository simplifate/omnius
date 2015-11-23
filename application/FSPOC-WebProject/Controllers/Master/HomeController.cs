using System.Collections.Generic;
using System.Web.Mvc;
using FSPOC_WebProject.Models;

namespace FSS.Omnius.Controllers.Master
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var appTiles = new List<AppTile>
            {
                new AppTile
                {
                    Name            = "Rezervace služeb",
                    TileWidth       = 2,
                    TileHeight      = 2,
                    Color           = 1,
                    Icon            = "fa-calendar",
                    PositionX       = 710,
                    PositionY       = 100,
                    InnerHTML       =
                        "<span class='mediumFont'>Zasedací místnost A:<br />12:30-14:00<hr />Zasedací místnost B:<br /><span class=\"highlight\">volná</span></span>"
                },
                new AppTile
                {
                    Name       = "Překlady",
                    TileWidth  = 2,
                    TileHeight = 1,
                    Color      = 2,
                    Icon       = "fa-copy",
                    PositionX  = 950,
                    PositionY  = 340,
                    InnerHTML  = ""
                },
                new AppTile
                {
                    Name          = "Evidence externích přístupů",
                    TileWidth     = 2,
                    TileHeight    = 1,
                    Icon          = "fa-wifi",
                    TitleFontSize = 13,
                    PositionX     = 710,
                    PositionY     = 340,
                    InnerHTML     = ""
                },
                new AppTile
                {
                    Name            = "Portál interních auditů",
                    TileWidth       = 2,
                    TileHeight      = 2,
                    Icon            = "fa-check",
                    TitleFontSize   = 15,
                    PositionX       = 950,
                    PositionY       = 100,
                    InnerHTML       =
                        "<span class='mediumFont'>Výsledky auditu z <span class=\"highlight\">6.10.2015</span> jsou nyní dostupné ke stažení</span>"
                },
                new AppTile
                {
                    Name       = "Náklady a výnosy",
                    TileWidth  = 4,
                    TileHeight = 4,
                    Icon       = "fa-line-chart",
                    PositionX  = 230,
                    PositionY  = 100,
                    InnerHTML  = "<svg width=\"430\" height=\"400\">"
                                +
                                "<path d=\"M 0 100 L 430 100\"  stroke=\"#53ccff\" stroke-width=\"1\" stroke-opacity=\"0.50\" fill=\"none\" />"
                                +
                                "<path d=\"M 0 200 L 430 200\"  stroke=\"#53ccff\" stroke-width=\"1\" fill=\"none\" />"
                                +
                                "<path d=\"M 0 300 L 430 300\"  stroke=\"#53ccff\" stroke-width=\"1\" stroke-opacity=\"0.50\" fill=\"none\" />"
                                +
                                "<path d=\"M 100 10 L 100 400\"  stroke=\"#53ccff\" stroke-width=\"1\" stroke-opacity=\"0.50\"  fill=\"none\" />"
                                +
                                "<path d=\"M 200 10 L 200 400\"  stroke=\"#53ccff\" stroke-width=\"1\" fill=\"none\" />"
                                +
                                "<path d=\"M 300 10 L 300 400\"  stroke=\"#53ccff\" stroke-width=\"1\" stroke-opacity=\"0.50\" fill=\"none\" />"
                                +
                                "<path d=\"M 400 10 L 400 400\"  stroke=\"#53ccff\" stroke-width=\"1\" fill=\"none\" />"
                                +
                                "<path d=\"M 0 400 L 50 350 L 100 320 L 150 200 L 200 340 L 250 260 L 300 190 L 350 230 L 400 110 L 429 50 L 429 400 Z\" "
                                + "stroke=\"none\" fill=\"#000020\" fill-opacity=\"0.45\"/>"
                                + "<path d=\"M 0 10 L 430 10\"  stroke=\"#53ccff\" stroke-width=\"2\" fill=\"none\" />"
                                + "<path d=\"M 0 400 L 0 10\"  stroke=\"#53ccff\" stroke-width=\"2\" fill=\"none\" />"
                                +
                                "<path d=\"M 0 400 L 430 400\"  stroke=\"#53ccff\" stroke-width=\"2\" fill=\"none\" />"
                                +
                                "<path d=\"M 430 400 L 430 10\"  stroke=\"#53ccff\" stroke-width=\"2\" fill=\"none\" />"
                                +
                                "<path d=\"M 0 400 L 50 350 L 100 320 L 150 200 L 200 340 L 250 260 L 300 190 L 350 230 L 400 110 L 429 50\" "
                                + "stroke=\"#ffdf00\" stroke-width=\"4\" fill=\"none\" /></svg>"
                },
                new AppTile
                {
                    Name       = "Tracking RWE",
                    TileWidth  = 2,
                    TileHeight = 1,
                    Color      = 1,
                    Icon       = "fa-eye",
                    PositionX  = 710,
                    PositionY  = 460,
                    InnerHTML  = ""
                }
            };

            ViewData["Tiles"] = appTiles;
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}