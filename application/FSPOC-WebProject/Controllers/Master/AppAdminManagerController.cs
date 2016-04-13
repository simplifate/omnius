using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Tapestry.Service;
using FSS.Omnius.Modules.CORE;

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
            var core = HttpContext.GetCORE();
            core.Entitron.AppId = Id;

            using (var context = new DBEntities())
            {
                var app = context.Applications.Find(Id);
                // Entitron Generate Database
                if (app.DbSchemeLocked)
                    throw new InvalidOperationException("This application's database scheme is locked because another process is currently working with it.");
                try
                {
                    try
                    {
                        app.DbSchemeLocked = true;
                        context.SaveChanges();
                        core.Entitron.AppId = app.Id;
                        var dbSchemeCommit = app.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                        if (dbSchemeCommit == null)
                            dbSchemeCommit = new DbSchemeCommit();
                        DatabaseGenerateService.GenerateDatabase(dbSchemeCommit, core);
                        app.DbSchemeLocked = false;
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error in Entitron: {ex.Message}", ex);
                    }


                    // Mozaic pages
                    try
                    {
                        foreach (var editorPage in app.MozaicEditorPages)
                        {
                            editorPage.Recompile();
                            string requestedPath = $"/Views/App/{Id}/Page/{editorPage.Id}.cshtml";
                            var oldPage = context.Pages.FirstOrDefault(c => c.ViewPath == requestedPath);
                            if (oldPage == null)
                            {
                                var newPage = new Page
                                {
                                    ViewName = editorPage.Name,
                                    ViewPath = $"/Views/App/{Id}/Page/{editorPage.Id}.cshtml",
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
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error in Mozaic: {ex.Message}", ex);
                    }

                    // Tapestry
                    Dictionary<int, Block> blockMapping = null;
                    try
                    {
                        var service = new TapestryGeneratorService();
                        blockMapping = service.GenerateTapestry(core);
                    }
                    catch(Exception e)
                    {
                        throw new Exception($"Error in Tapestry: {e.Message}", e);
                    }

                    // menu layout
                    try
                    {
                        string path = $"/Views/App/{Id}/menuLayout.cshtml";
                        var menuLayout = context.Pages.FirstOrDefault(c => c.ViewPath == path);
                        if (menuLayout == null)
                        {
                            menuLayout = new Page
                            {
                                ViewPath = $"/Views/App/{Id}/menuLayout.cshtml"
                            };
                            context.Pages.Add(menuLayout);
                        }
                        menuLayout.ViewName = $"{app.Name} layout";
                        menuLayout.ViewContent = GetApplicationMenu(core, blockMapping).Item1;

                        app.IsPublished = true;
                        context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        throw new Exception($"Error in menu generate: {ex.Message}", ex);
                    }

                    //pass the message object to view
                    var message = new Message();
                    message.Success.Add("Stránky byly úpěšně zkompilovány.");
                    ViewBag.Message = message;

                    return View();
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(ex, Request);
                    //pass the message object to view
                    var message = new Message();
                    message.Errors.Add(ex.Message);
                    ViewBag.Message = message;

                    return View();
                }
                finally
                {
                    app.DbSchemeLocked = false;
                    context.SaveChanges();
                }
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

            foreach (TapestryDesignerBlock b in e.TapestryDesignerBlocks.Include("ParentMetablock").Where(b => b.ParentMetablock.Id == rootId && b.IsInMenu == true))
            {
                var commit = b.BlockCommits.OrderByDescending(bc => bc.Timestamp).FirstOrDefault();
                if (commit != null)
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

            ViewData.Model = items;
            ViewData["Level"] = level;
            ViewBag.AppId = core.Entitron.Application.Id;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "~/Views/Shared/_ApplicationMenu.cshtml");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return new Tuple<string, HashSet<string>>(sw.GetStringBuilder().ToString(), rights);
            }
        }
    }
}
