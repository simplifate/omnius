﻿using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System.IO;
using FSS.Omnius.Modules.Entitron.Service;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(Roles = "Admin", Module = "Master")]
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
            bool dbSchemeLocked = false;

            using (var context = new DBEntities())
            {
                var app = context.Applications.Find(Id);
                
                // Entitron Generate Database
                if (app.DbSchemeLocked)
                    throw new InvalidOperationException("This application's database scheme is locked because another process is currently working with it.");
                app.DbSchemeLocked = dbSchemeLocked = true;
                context.SaveChanges();
                core.Entitron.AppId = app.Id;
                var dbSchemeCommit = app.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).First();
                DatabaseGenerateService.GenerateDatabase(dbSchemeCommit, core);
                app.DbSchemeLocked = false;
                context.SaveChanges();


                // Mozaic pages
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

                // Tapestry
                var service = new TapestryGeneratorService();
                var blockMapping = service.GenerateTapestry(core);

                // menu layout
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
                menuLayout.ViewContent = GetApplicationMenu(core, blockMapping);

                app.IsPublished = true;
                context.SaveChanges();


                return View();
            }
        }

        private string GetApplicationMenu(Modules.CORE.CORE core, Dictionary<int, Block> blockMapping, int rootId = 0, int level = 0)
        {
            DBEntities e = core.Entitron.GetStaticTables();
            rootId = rootId == 0 ? core.Entitron.Application.TapestryDesignerRootMetablock.Id : rootId;

            List<TapestryDesignerMenuItem> items = new List<TapestryDesignerMenuItem>();
            foreach (TapestryDesignerMetablock m in e.TapestryDesignerMetablocks.Include("ParentMetablock").Where(m => m.ParentMetablock.Id == rootId && m.IsInMenu == true))
            {
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    SubMenu = GetApplicationMenu(core, blockMapping, m.Id, level + 1),
                    IsInitial = m.IsInitial,
                    IsInMenu = m.IsInMenu,
                    MenuOrder = m.MenuOrder,
                    IsMetablock = true
                });
            }

            foreach (TapestryDesignerBlock b in e.TapestryDesignerBlocks.Include("ParentMetablock").Where(b => b.ParentMetablock.Id == rootId && b.IsInMenu == true))
            {
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsInitial = b.IsInitial,
                    IsInMenu = b.IsInMenu,
                    MenuOrder = b.MenuOrder,
                    IsBlock = true,
                    BlockId = blockMapping[b.Id].Id
                });
            }

            ViewData.Model = items;
            ViewData["Level"] = level;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "~/Views/Shared/_ApplicationMenu.cshtml");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
