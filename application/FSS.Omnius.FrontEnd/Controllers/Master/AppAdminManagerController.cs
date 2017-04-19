using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Web.Helpers;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Tapestry.Service;
using FSS.Omnius.Modules.CORE;
using Microsoft.Web.WebSockets;
using RazorEngine;
using RazorEngine.Templating;
using System.Configuration;
using FSS.Omnius.Modules.Mozaic.BootstrapEditor;
using FSS.Omnius.Utils.Builder;
using FSS.Utils;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;

namespace FSS.Omnius.Controllers.Master
{
    [PersonaAuthorize(NeedsAdmin = true, Module = "Master")]
    public class AppAdminManagerController : Controller
    {
        public ActionResult Index()
        {
            var context = DBEntities.instance;
            var appList = new List<Application>();

            foreach (var app in context.Applications)
            {
                appList.Add(app);
            }
            ViewData["Apps"] = appList;
            ViewData["Mozaic_CssTemplates"] = context.CssTemplates.ToList();
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

        public void SendMessage(object sender, BasicDispatchArgs args)
        {
            string data;

            if(args.Data.GetType() != typeof(string))
            {
                // If message isn't string encode it into JSON
                data = Json.Encode(args.Data);
            }
            else
            {
                // Otherwise, convert it implicitly
                data = (string)args.Data;
            }

            // Send it
            Send(data);
        }

        public override void OnOpen()
        {
            var core = new Modules.CORE.CORE();
            core.Entitron.AppId = _AppId;
            Application masterApp = core.Entitron.Application;
            DBEntities masterContext = DBEntities.instance;
            DBEntities context = DBEntities.appInstance(core.Entitron.Application);

            if (masterApp.DbSchemeLocked)
            {
                Send(Json.Encode(new { type = "error", message = "This application's database scheme is locked because another process is currently working with it.", abort = true }));
                return;
            }

            // copy application to new DB
            Application app;
            if (context != masterContext)
            {
                app = context.Applications.SingleOrDefault(a => a.Name == core.Entitron.AppName);
                if (app == null)
                {
                    app = new Application
                    {
                        Name = masterApp.Name
                    };
                    context.Applications.Add(app);
                }

                app.DisplayName = masterApp.DisplayName;
                app.Icon = masterApp.Icon;
                app.TitleFontSize = masterApp.TitleFontSize;
                app.Color = masterApp.Color;
                app.InnerHTML = masterApp.InnerHTML;
                app.LaunchCommand = masterApp.LaunchCommand;
                app.TileWidth = masterApp.TileWidth;
                app.TileHeight = masterApp.TileHeight;

                app.IsPublished = masterApp.IsPublished;
                app.IsEnabled = masterApp.IsEnabled;
                app.IsSystem = masterApp.IsSystem;

                app.connectionString_data = masterApp.connectionString_data;
                app.connectionString_schema = masterApp.connectionString_schema;
                
                context.SaveChanges();
            }
            else
                app = masterApp;

            try
            {
                // If building shared tables app
                if (_AppId == SharedTables.AppId)
                {
                    // Create instance of db builder
                    SharedTableBuilder builder = new SharedTableBuilder(core, context, app);

                    // Assign dispatcher 
                    builder.OnDispatch += this.SendMessage;

                    // Build tables
                    builder.Build();

                    // And skip standard building process.
                    return;
                }

                // Entitron Generate Database
                if (masterApp.EntitronChangedSinceLastBuild)
                    Send(Json.Encode(new { id = "entitron", type = "info", message = "proběhne aktualizace databáze" }));
                else
                    Send(Json.Encode(new { id = "entitron", type = "success", message = "databázi není potřeba aktualizovat" }));

                if (masterApp.MozaicChangedSinceLastBuild || _rebuildInAction)
                    Send(Json.Encode(new { id = "mozaic", type = "info", message = "proběhne aktualizace uživatelského rozhraní" }));
                else
                    Send(Json.Encode(new { id = "mozaic", type = "success", message = "uživatelské rozhraní není potřeba aktualizovat" }));
                if (masterApp.TapestryChangedSinceLastBuild || masterApp.MenuChangedSinceLastBuild || _rebuildInAction)
                {
                    Send(Json.Encode(new { id = "tapestry", type = "info", message = "proběhne aktualizace workflow" }));
                    Send(Json.Encode(new { id = "menu", type = "info", message = "proběhne aktualizace menu" }));
                }
                else
                {
                    Send(Json.Encode(new { id = "tapestry", type = "success", message = "workflow není potřeba aktualizovat" }));
                    Send(Json.Encode(new { id = "menu", type = "success", message = "menu není potřeba aktualizovat" }));
                }
                if (!masterApp.EntitronChangedSinceLastBuild && !masterApp.MozaicChangedSinceLastBuild && !masterApp.TapestryChangedSinceLastBuild && !masterApp.MenuChangedSinceLastBuild && !_rebuildInAction)
                {
                    Send(Json.Encode(new { type = "success", message = "od poslední aktualizace neproběhly žádné změny", done = true }));
                    return;
                }


                // clear page cache
                //if (app.MozaicChangedSinceLastBuild || app.MenuChangedSinceLastBuild)
                //    FSS.Omnius.FrontEnd.Views.MyVirtualPathProvider.ClearAllCache();

                if (masterApp.EntitronChangedSinceLastBuild)
                {
                    try
                    {
                        masterApp.DbSchemeLocked = true;
                        masterContext.SaveChanges();

                        Send(Json.Encode(new { id = "entitron", type = "info", message = "probíhá aktualizace databáze" }));
                        var dbSchemeCommit = masterApp.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                        if (dbSchemeCommit == null)
                            dbSchemeCommit = new DbSchemeCommit();
                        if (!dbSchemeCommit.IsComplete)
                            throw new Exception("Pozor! Databázové schéma je špatně uložené, build nemůže pokračovat, protože by způsobil ztrádtu dat!");
                        new DatabaseGenerateService().GenerateDatabase(dbSchemeCommit, core, x => Send(x));

                        masterApp.DbSchemeLocked = false;
                        masterApp.EntitronChangedSinceLastBuild = false;
                        masterContext.SaveChanges();

                        Send(Json.Encode(new { id = "entitron", type = "success", message = "proběhla aktualizace databáze" }));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "entitron", type = "error", message = ex.Message, abort = true }));
                        //context.DiscardChanges();
                    }
                    finally
                    {
                        masterApp.DbSchemeLocked = false;
                        masterContext.SaveChanges();
                    }
                }

                string applicationViewPath = AppDomain.CurrentDomain.BaseDirectory + $"\\Views\\App\\{_AppId}";
                string applicationPageViewPath = applicationViewPath + "\\Page";
                if (!Directory.Exists(applicationViewPath))
                {
                    Directory.CreateDirectory(applicationViewPath);
                }
                if (!Directory.Exists(applicationPageViewPath))
                {
                    Directory.CreateDirectory(applicationPageViewPath);
                }

                // Mozaic pages
                if (masterApp.MozaicChangedSinceLastBuild || _rebuildInAction)
                {
                    try
                    {
                        Send(Json.Encode(new { id = "mozaic", type = "info", message = "probíhá aktualizace uživatelského rozhraní" }));

                        foreach (var editorPage in masterApp.MozaicEditorPages)
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
                                    ViewContent = editorPage.CompiledPartialView,
                                    IsBootstrap = false
                                };
                                context.Pages.Add(newPage);
                                context.SaveChanges();
                                editorPage.CompiledPageId = newPage.Id;
                            }
                            else
                            {
                                oldPage.ViewName = editorPage.Name;
                                oldPage.ViewContent = editorPage.CompiledPartialView;
                                oldPage.IsBootstrap = false;
                                editorPage.CompiledPageId = oldPage.Id;
                            }
                            string fileName = applicationPageViewPath + $"\\{editorPage.Id}.cshtml";

                            File.WriteAllText(fileName, editorPage.CompiledPartialView);
                        }

                        Builder bootstrapBuilder = new Builder(context, applicationPageViewPath);
                        foreach (var bootstrapPage in masterApp.MozaicBootstrapPages) {
                            bootstrapBuilder.BuildPage(bootstrapPage);
                        }


                        masterApp.MozaicChangedSinceLastBuild = false;
                        masterContext.SaveChanges();

                        Send(Json.Encode(new { id = "mozaic", type = "success", message = "proběhla aktualizace uživatelského rozhraní" }));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Json.Encode(new { id = "mozaic", type = "error", message = ex.Message, abort = true }));
                    }
                }

                if (masterApp.TapestryChangedSinceLastBuild || masterApp.MenuChangedSinceLastBuild || _rebuildInAction)
                {
                    // Tapestry
                    Dictionary<TapestryDesignerBlock, Block> blockMapping = null;
                    try
                    {
                        Send(Json.Encode(new { id = "tapestry", type = "info", message = "probíhá aktualizace workflow" }));
                        var service = new TapestryGeneratorService(masterContext, context, _rebuildInAction);
                        blockMapping = service.GenerateTapestry(core, x => Send(x));

                        masterApp.TapestryChangedSinceLastBuild = false;
                        masterContext.SaveChanges();

                        Send(Json.Encode(new { id = "tapestry", type = "success", message = "proběhla aktualizace workflow" }));
                    }
                    catch (Exception ex)
                    {
                        while (ex.Message.Contains("An error occurred while updating the entries. See the inner exception for details."))
                            ex = ex.InnerException;

                        throw new Exception(Json.Encode(new { id = "tapestry", type = "error", message = ex.Message, abort = true }));
                    }

                    // TODO Repair building menu - RazorEngine with $ problem
                    // menu layout
                    try
                    {
                        Send(Json.Encode(new { id = "menu", type = "info", message = "probíhá aktualizace menu" }));
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

                        masterApp.IsPublished = true;
                        masterApp.MenuChangedSinceLastBuild = false;
                        masterContext.SaveChanges();

                        string fileName = applicationViewPath + $"\\menuLayout.cshtml";
                        File.WriteAllText(fileName, menuLayout.ViewContent);

                        Send(Json.Encode(new { id = "menu", type = "success", message = "proběhla aktualizace menu" }));
                    }
                    catch (Exception ex)
                    {
                        while (ex.Message.Contains("An error occurred while updating the entries. See the inner exception for details."))
                            ex = ex.InnerException;

                        throw new Exception(Json.Encode(new { id = "menu", type = "error", message = ex.Message, abort = true }));
                    }
                }

                // Hermes
                if (context != masterContext)
                {
                    // remove old
                    context.EmailPlaceholders.RemoveRange(context.EmailPlaceholders.Where(ep => ep.Hermes_Email_Template.AppId == app.Id));
                    context.EmailContents.RemoveRange(context.EmailContents.Where(ec => ec.Hermes_Email_Template.AppId == app.Id));
                    context.EmailTemplates.RemoveRange(app.EmailTemplates);

                    // add new
                    foreach (EmailTemplate template in masterApp.EmailTemplates)
                    {
                        EmailTemplate newTemplate = new EmailTemplate
                        {
                            Application = app,
                            Is_HTML = template.Is_HTML,
                            Name = template.Name
                        };

                        foreach (EmailPlaceholder placeholder in template.PlaceholderList)
                        {
                            EmailPlaceholder newPlaceholder = new EmailPlaceholder
                            {
                                Description = placeholder.Description,
                                Num_Order = placeholder.Num_Order,
                                Prop_Name = placeholder.Prop_Name
                            };
                            newTemplate.PlaceholderList.Add(newPlaceholder);
                        }
                        foreach (EmailTemplateContent content in template.ContentList)
                        {
                            EmailTemplateContent newContent = new EmailTemplateContent
                            {
                                Content = content.Content,
                                Content_Plain = content.Content_Plain,
                                From_Email = content.From_Email,
                                From_Name = content.From_Name,
                                LanguageId = content.LanguageId,
                                Subject = content.Subject
                            };
                            newTemplate.ContentList.Add(newContent);
                        }

                        context.EmailTemplates.Add(newTemplate);
                    }
                }

                // DONE
                context.SaveChanges();

                Send(Json.Encode(new { message = "aktualizace proběhla úspěšně", type = "success", done = true }));
                masterApp.DbSchemeLocked = false;
                masterContext.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorMessage(ex);
            }
        }


        private Tuple<string, HashSet<string>> GetApplicationMenu(Modules.CORE.CORE core, Dictionary<TapestryDesignerBlock, Block> blockMapping, int rootId = 0, int level = 0)
        {
            DBEntities e = DBEntities.instance;
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
                else
                    rights.Add("User");
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsInitial = b.IsInitial,
                    IsInMenu = b.IsInMenu,
                    MenuOrder = b.MenuOrder,
                    IsBlock = true,
                    BlockName = b.Name.RemoveDiacritics(),
                    rights = commit != null && !string.IsNullOrEmpty(commit.RoleWhitelist) ? commit.RoleWhitelist : "User"
                });
            }

            /*ViewDataDictionary ViewData = new ViewDataDictionary();   
            ViewData.Model = items;
            ViewData["Level"] = level;
            ViewData["ParentID"] = rootId;
            ViewData["AppId"] = core.Entitron.Application.Id;
            */
            //TempDataDictionary TempData = new TempDataDictionary();
            //ControllerContext ControllerContext = new ControllerContext();

            //using (var sw = new StringWriter())
            //{
            //    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "~/Views/Shared/_ApplicationMenu.cshtml");
            //    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
            //    viewResult.View.Render(viewContext, sw);
            //    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
            //    return new Tuple<string, HashSet<string>>(sw.GetStringBuilder().ToString(), rights);
            //}


            DynamicViewBag ViewBag = new DynamicViewBag();
            ViewBag.AddValue("Level", level);
            ViewBag.AddValue("AppId", core.Entitron.AppId);
            ViewBag.AddValue("AppName", core.Entitron.AppName);
            ViewBag.AddValue("ParentId", rootId);

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
