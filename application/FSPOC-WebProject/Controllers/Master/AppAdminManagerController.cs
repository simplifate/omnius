using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Tapestry.Service;
using System.Web.Helpers; 
using System.Net;
using Microsoft.Web.WebSockets;
using RazorEngine.Templating;
using RazorEngine;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Master")]
    public class AppAdminManagerController : Controller
    {
        private IDatabaseGenerateService DatabaseGenerateService { get; set; }

        public AppAdminManagerController(IDatabaseGenerateService databaseGenerateService)
        {
            DatabaseGenerateService = databaseGenerateService; // Unity dependency injection
        }
        public ActionResult Index()
        {
            using (var context = new DBEntities())
            {
                var appList = new List<Application>();

                foreach (var app in context.Applications)
                {
                    appList.Add(app);
                }
                ViewData["Apps"] = appList;
                return View();
            }
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

    }

    class BuildWebSocketHandler : WebSocketHandler
    {
        private int _AppId;
        private string _menuPath;

        public BuildWebSocketHandler(int appId, string menuPath)
        {
            _AppId = appId;
            _menuPath = menuPath;
        }

        public override void OnOpen()
        {
            var core = new Modules.CORE.CORE();
            core.Entitron.AppId = _AppId;
            var app = core.Entitron.Application;
            DBEntities context = core.Entitron.GetStaticTables();

            try
            {
                // Entitron Generate Database
                if (app.DbSchemeLocked)
                    throw new InvalidOperationException("This application's database scheme is locked because another process is currently working with it.");

                if (app.EntitronChangedSinceLastBuild)
                    Send(Json.Encode(new { id = "entitron", type = "info", message = "proběhne aktualizace databáze" }));
                else
                    Send(Json.Encode(new { id = "entitron", type = "success", message = "databázi není potřeba aktualizovat" }));

                if (app.MozaicChangedSinceLastBuild)
                    Send(Json.Encode(new { id = "mozaic", type = "info", message = "proběhne aktualizace uživatelského rozhraní" }));
                else
                    Send(Json.Encode(new { id = "mozaic", type = "success", message = "uživatelské rozhraní není potřeba aktualizovat" }));
                if (app.TapestryChangedSinceLastBuild || app.MenuChangedSinceLastBuild)
                {
                    Send(Json.Encode(new { id = "tapestry", type = "info", message = "proběhne aktualizace workflow" }));
                    Send(Json.Encode(new { id = "menu", type = "info", message = "proběhne aktualizace menu" }));
                }
                else
                {
                    Send(Json.Encode(new { id = "tapestry", type = "success", message = "workflow není potřeba aktualizovat" }));
                    Send(Json.Encode(new { id = "menu", type = "success", message = "menu není potřeba aktualizovat" }));
                }
                if (!app.EntitronChangedSinceLastBuild && !app.MozaicChangedSinceLastBuild && !app.TapestryChangedSinceLastBuild && !app.MenuChangedSinceLastBuild)
                {
                    Send(Json.Encode(new { type = "success", message = "od poslední aktualizace neproběhly žádné změny", done = true }));
                    return;
                }

                app.DbSchemeLocked = true;
                context.SaveChanges();

                if (app.EntitronChangedSinceLastBuild)
                {
                    try
                    {
                        core.Entitron.AppId = app.Id;
                        var dbSchemeCommit = app.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                        if (dbSchemeCommit == null)
                            dbSchemeCommit = new DbSchemeCommit();
                        new DatabaseGenerateService().GenerateDatabase(dbSchemeCommit, core);
                        app.DbSchemeLocked = false;
                        app.EntitronChangedSinceLastBuild = false;
                        context.SaveChanges();
                        Send(Json.Encode(new { id = "entitron", type = "success", message = "proběhla aktualizace databáze" }));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "entitron", type = "error", message = ex.Message, abort = true }));
                    }
                }

                // Mozaic pages
                if (app.MozaicChangedSinceLastBuild)
                {
                    try
                    {
                        foreach (var editorPage in app.MozaicEditorPages)
                        {
                            editorPage.Recompile();
                            string requestedPath = $"/Views/App/{_AppId}/Page/{editorPage.Id}.cshtml";
                            var oldPage = context.Pages.FirstOrDefault(c => c.ViewPath == requestedPath);
                            if (oldPage == null)
                            {
                                var newPage = new Page
                                {
                                    ViewName = editorPage.Name,
                                    ViewPath = $"/Views/App/{_AppId}/Page/{editorPage.Id}.cshtml",
                                    ViewContent = editorPage.CompiledPartialView
                                };
                                context.Pages.Add(newPage);
                                context.SaveChanges();
                                editorPage.CompiledPageId = newPage.Id;
                            }
                            else
                            {
                                oldPage.ViewName = editorPage.Name;
                                oldPage.ViewContent = editorPage.CompiledPartialView;
                                editorPage.CompiledPageId = oldPage.Id;
                            }
                        }
                        app.MozaicChangedSinceLastBuild = false;
                        context.SaveChanges();

                        Send(Json.Encode(new { id = "mozaic", type = "success", message = "proběhla aktualizace uživatelského rozhraní" }));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "mozaic", type = "error", message = ex.Message, abort = true }));
                    }
                }

                if (app.TapestryChangedSinceLastBuild || app.MenuChangedSinceLastBuild)
                {
                    // Tapestry
                    Dictionary<int, Block> blockMapping = null;
                    try
                    {
                        var service = new TapestryGeneratorService();
                        blockMapping = service.GenerateTapestry(core, x => Send(x));
                        app.TapestryChangedSinceLastBuild = false;
                        Send(Json.Encode(new { id = "tapestry", type = "success", message = "proběhla aktualizace workflow" }));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "tapestry", type = "error", message = ex.Message, abort = true }));
                    }

                    // menu layout
                    try
                    {
                        string path = $"/Views/App/{_AppId}/menuLayout.cshtml";
                        var menuLayout = context.Pages.FirstOrDefault(c => c.ViewPath == path);
                        if (menuLayout == null)
                        {
                            menuLayout = new Page
                            {
                                ViewPath = $"/Views/App/{_AppId}/menuLayout.cshtml"
                            };
                            context.Pages.Add(menuLayout);
                        }
                        menuLayout.ViewName = $"{app.Name} layout";
                        menuLayout.ViewContent = GetApplicationMenu(core, blockMapping).Item1;

                        app.IsPublished = true;
                        app.MenuChangedSinceLastBuild = false;
                        context.SaveChanges();
                        Send(Json.Encode(new { id = "menu", type = "success", message = "proběhla aktualizace menu"}));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "menu", type = "error", message = ex.Message, abort = true }));
                    }
                }

                // DONE
                Send(Json.Encode(new { message = "aktualizace proběhla úsěpšně", type = "success", done = true }));
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
            finally
            {
                context.DiscardChanges();
                app.DbSchemeLocked = false;
                context.SaveChanges();
                core.Entitron.CloseStaticTables();
            }
        }


        private Tuple<string, HashSet<string>> GetApplicationMenu(Modules.CORE.CORE core, Dictionary<int, Block> blockMapping, int rootId = 0, int level = 0)
        {
            DBEntities e = core.Entitron.GetStaticTables();
            rootId = rootId == 0 ? core.Entitron.Application.TapestryDesignerRootMetablock.Id : rootId;
            HashSet<string> rights = new HashSet<string>();

            List<TapestryDesignerMenuItem> items = new List<TapestryDesignerMenuItem>();
            foreach (TapestryDesignerMetablock m in e.TapestryDesignerMetablocks.Include("ParentMetablock").Where(m => m.ParentMetablock.Id == rootId && m.IsInMenu == true))
            {
                var menuResult = GetApplicationMenu(core, blockMapping, m.Id, level + 1);
                rights.AddRange(menuResult.Item2);
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    SubMenu = menuResult.Item1,
                    IsInitial = m.IsInitial,
                    IsInMenu = m.IsInMenu,
                    MenuOrder = m.MenuOrder,
                    IsMetablock = true,
                    rights = string.Join(",", menuResult.Item2)
                });
            }

            foreach (TapestryDesignerBlock b in e.TapestryDesignerBlocks.Include("ParentMetablock").Where(b => !b.IsDeleted && b.ParentMetablock.Id == rootId && b.IsInMenu == true))
            {
                var commit = b.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                if (commit != null && !string.IsNullOrWhiteSpace(commit.RoleWhitelist))
                    rights.AddRange(commit.RoleWhitelist.Split(','));
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsInitial = b.IsInitial,
                    IsInMenu = b.IsInMenu,
                    MenuOrder = b.MenuOrder,
                    IsBlock = true,
                    BlockName = blockMapping[b.Id].Name,
                    rights = commit != null ? commit.RoleWhitelist : ""
                });
            }

            DynamicViewBag ViewBag = new DynamicViewBag();
            ViewBag.AddValue("Level", level);
            ViewBag.AddValue("AppId", core.Entitron.AppId);
            ViewBag.AddValue("AppName", core.Entitron.AppName);

            string source = File.ReadAllText(_menuPath);

            string result = Engine.Razor.RunCompile(source, new Random().Next().ToString(), null, items, ViewBag);
            return new Tuple<string, HashSet<string>>(result, rights);
        }

        private void ErrorMessage(Exception ex)
        {
            Send(ex.Message);
        }
    }
}
