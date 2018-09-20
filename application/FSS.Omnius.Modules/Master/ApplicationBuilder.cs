using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Service;
using FSS.Omnius.Modules.Mozaic.BootstrapEditor;
using FSS.Omnius.Modules.Tapestry.Service;
using FSS.Omnius.Modules.Tapestry2.Services;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FSS.Omnius.Modules.Master
{
    public class ApplicationBuilder
    {
        public ApplicationBuilder(Action<string> sendAction)
        {
            progressHandler = new ModalProgressHandler<EModule>(sendAction);
        }

        private COREobject core;
        private Application masterApp;
        private Application app;
        private DBEntities masterContext;
        private DBEntities context;
        private ModalProgressHandler<EModule> progressHandler;
        private bool _rebuildInAction;
        private string _menuPath;

        private string applicationViewPath;
        private string applicationPageViewPath;
        private Dictionary<TapestryDesignerBlock, Block> blockMapping;

        public void Build(int ApplicationId, string menuPath, bool forceRebuild)
        {
            try
            {
                // INIT
                core = COREobject.i;
                masterContext = core.Context;
                core.Application = masterContext.Applications.Find(ApplicationId);
                masterApp = core.Application;
                context = core.AppContext;
                app = context != masterContext
                    ? context.Applications.SingleOrDefault(a => a.Name == masterApp.Name)
                    : masterApp;
                _rebuildInAction = forceRebuild;
                _menuPath = menuPath;

                applicationViewPath = $"{AppDomain.CurrentDomain.BaseDirectory}Views\\App\\{app.Name}";
                applicationPageViewPath = $"{applicationViewPath}\\Page";

                // Shared tables
                if (masterApp.IsSystem)
                {
                    progressHandler.Section(EModule.Entitron, "Actualize database");
                    BuildEntitron();
                    return;
                }

                // FrontEnd
                progressHandler.Section(EModule.Master, "Copy application");
                progressHandler.Section(EModule.Persona, "Copy user roles");
                progressHandler.Section(EModule.Entitron, "Actualize database");
                progressHandler.Section(EModule.Mozaic, "Generate pages");
                progressHandler.Section(EModule.Tapestry, "Generate workflow");
                progressHandler.Section(EModule.CORE, "Generate menu");
                progressHandler.Section(EModule.Hermes, "Copy email template");

                MoveApp();
                BuildPersona();
                BuildEntitron();
                BuildMozaic();
                BuildTapestry();
                BuildTapestry2();
                BuildMenu();
                BuildHermes();

                progressHandler.Section(EModule.Watchtower, "All DONE");
            }
            catch (OmniusMultipleException ex)
            {
                if (!ex.AllExceptions.Any())
                    progressHandler.Error("Unknown error");

                foreach (Exception e in ex.AllExceptions)
                    progressHandler.Error(e.Message);
            }
            catch (Exception ex)
            {
                progressHandler.Error(ex.Message);
            }
        }

        private void MoveApp()
        {
            progressHandler.SetActiveSection(EModule.Master);

            // copy application to new DB
            if (context != masterContext)
            {
                progressHandler.SetMessage(message: "Copying application to app database", type: MessageType.InProgress);

                app = context.Applications.SingleOrDefault(a => a.Name == core.Application.Name);
                if (app == null)
                {
                    app = new Application();
                    context.Applications.Add(app);
                }

                app.CopyPropertiesFrom(masterApp, skip: new string[] { "Id" });
                context.SaveChanges();

                progressHandler.SetMessage(message: "Application copied", type: MessageType.Success);
            }
            else
            {
                progressHandler.SetMessage(message: "Application is not necessary to copy", type: MessageType.Info);
                app = masterApp;
            }

        }
        private void BuildPersona()
        {
            progressHandler.SetActiveSection(EModule.Persona);

            if (masterContext != context)
            {
                progressHandler.SetMessage(message: "Copying user roles to app database", type: MessageType.InProgress);

                // copy Persona_AppRoles
                foreach (PersonaAppRole role in masterApp.Roles)
                {
                    PersonaAppRole newRole = context.AppRoles.SingleOrDefault(r => r.ApplicationId == app.Id && r.Name == role.Name);
                    if (newRole == null)
                    {
                        newRole = new PersonaAppRole
                        {
                            Name = role.Name,
                            Priority = role.Priority
                        };
                        app.Roles.Add(newRole);
                    }
                    else
                    {
                        newRole.Priority = role.Priority;
                    }
                }
                context.SaveChanges();

                progressHandler.SetMessage(message: "User roles copied", type: MessageType.Success);
            }
            else
                progressHandler.SetMessage(message: "User roles is not necessary to copy", type: MessageType.Info);
        }
        private void BuildEntitron()
        {
            progressHandler.SetActiveSection(EModule.Entitron);

            if (masterApp.EntitronChangedSinceLastBuild || _rebuildInAction)
            {
                if (masterApp.DbSchemeLocked)
                {
                    throw new Exception("Database is locked - another process is currently working with it");
                }

                progressHandler.SetMessage(message: "Actualizing database", type: MessageType.InProgress);

                try
                {
                    masterApp.DbSchemeLocked = true;
                    masterContext.SaveChanges();
                    
                    var dbSchemeCommit = masterApp.DatabaseDesignerSchemeCommits.OrderByDescending(o => o.Timestamp).FirstOrDefault();
                    if (dbSchemeCommit == null)
                        dbSchemeCommit = new DbSchemeCommit();
                    if (!dbSchemeCommit.IsComplete)
                        throw new Exception("Database scheme is not saved correctly!");

                    new DatabaseGenerateService(progressHandler).GenerateDatabase(dbSchemeCommit, core);

                    masterApp.DbSchemeLocked = false;
                    masterApp.EntitronChangedSinceLastBuild = false;
                    masterContext.SaveChanges();

                    progressHandler.SetMessage("final", "Database actualised", MessageType.Success);
                    progressHandler.SetMessage(type: MessageType.Success);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    masterApp.DbSchemeLocked = false;
                    masterContext.SaveChanges();
                }
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Database is not necessary to actualize");
        }
        private void BuildMozaic()
        {
            progressHandler.SetActiveSection(EModule.Mozaic);
            
            if (masterApp.MozaicChangedSinceLastBuild || _rebuildInAction)
            {
                progressHandler.SetMessage("directory", "Create directories", MessageType.Info);
                //progressHandler.SetMessage("css", "Create CSS template", MessageType.Info);
                progressHandler.SetMessage("pages", "Generate legacy pages", MessageType.Info);
                progressHandler.SetMessage("bootstrapPages", "Generate bootstrap pages", MessageType.Info);

                // dirs
                progressHandler.SetMessage("directory", "Creating directories", MessageType.InProgress);
                if (!Directory.Exists(applicationViewPath))
                    Directory.CreateDirectory(applicationViewPath);
                if (!Directory.Exists(applicationPageViewPath))
                    Directory.CreateDirectory(applicationPageViewPath);
                progressHandler.SetMessage("directory", "Directories created", MessageType.Success);

                //// create template
                //if (masterContext != context && masterApp.CssTemplate != null)
                //{
                //    progressHandler.SetMessage("css", "Creating CSS template", MessageType.InProgress);
                //    app.CssTemplate = new MozaicCssTemplate
                //    {
                //        Name = masterApp.CssTemplate.Name,
                //        Url = masterApp.CssTemplate.Url
                //    };
                //    progressHandler.SetMessage("css", "CSS template created", MessageType.Success);
                //}
                //else
                //    progressHandler.SetMessage("css", "CSS template is not necessary to actualize", MessageType.Info);

                // pages
                progressHandler.SetMessage("pages", "Generating legacy pages", MessageType.InProgress, masterApp.MozaicEditorPages.Count);

                foreach (var editorPage in masterApp.MozaicEditorPages.Where(p => !p.IsDeleted))
                {
                    editorPage.Recompile();
                    string requestedPath = $"{applicationPageViewPath}\\{editorPage.Id}.cshtml";
                    var oldPage = context.Pages.FirstOrDefault(c => c.ViewPath == requestedPath);
                    if (oldPage == null)
                    {
                        var newPage = new Page
                        {
                            ViewName = editorPage.Name,
                            ViewPath = requestedPath,
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

                    File.WriteAllText(requestedPath, editorPage.CompiledPartialView);

                    progressHandler.IncrementProgress("pages");
                }
                progressHandler.SetMessage("pages", "Generating legacy pages completed", MessageType.Success);

                progressHandler.SetMessage("bootstrapPages", "Generating bootstrap pages", MessageType.InProgress, masterApp.MozaicBootstrapPages.Count);
                Builder bootstrapBuilder = new Builder(context, applicationPageViewPath);
                foreach (var bootstrapPage in masterApp.MozaicBootstrapPages)
                {
                    bootstrapBuilder.BuildPage(bootstrapPage);
                    progressHandler.IncrementProgress("bootstrapPages");
                }
                progressHandler.SetMessage("bootstrapPages", "Generating bootstrap pages completed", MessageType.Success);

                masterApp.MozaicChangedSinceLastBuild = false;
                masterContext.SaveChanges();

                progressHandler.SetMessage("final", "Pages done", MessageType.Success);
                progressHandler.SetMessage(type: MessageType.Success);
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Pages is not necessary to actualize");
        }
        private void BuildTapestry()
        {
            progressHandler.SetActiveSection(EModule.Tapestry);

            if (masterApp.TapestryChangedSinceLastBuild || masterApp.MenuChangedSinceLastBuild || _rebuildInAction)
            {
                progressHandler.SetMessage("block", "Create metablocks and blocks", MessageType.Info);
                progressHandler.SetMessage("blockContent", "Create block content", MessageType.Info);

                // Tapestry
                blockMapping = null;
                var service = new TapestryGeneratorService(masterContext, context, _rebuildInAction);
                blockMapping = service.GenerateTapestry(core, progressHandler);
                
                progressHandler.SetMessage("final", "Generate Workflow completed", MessageType.Success);
                progressHandler.SetMessage(type: MessageType.Success);
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Workflow is not necessary to generate");
        }
        private void BuildTapestry2()
        {
            progressHandler.SetActiveSection(EModule.Tapestry);

            if (masterApp.TapestryChangedSinceLastBuild || masterApp.MenuChangedSinceLastBuild || _rebuildInAction)
            {
                // Tapestry
                var service = new TapestryGenerateService(masterContext, app, progressHandler, _rebuildInAction);
                service.Generate();

                masterApp.TapestryChangedSinceLastBuild = false;
                masterContext.SaveChanges();

                progressHandler.SetMessage("T2_final", "Generate Workflow completed", MessageType.Success);
                progressHandler.SetMessage(type: MessageType.Success);
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Workflow is not necessary to generate");
        }
        private void BuildMenu()
        {
            progressHandler.SetActiveSection(EModule.CORE);

            if (masterApp.TapestryChangedSinceLastBuild || masterApp.MenuChangedSinceLastBuild || _rebuildInAction)
            {
                // menu layout
                progressHandler.SetMessage(type: MessageType.InProgress, message: "Generating menu");
                string path = $"{applicationViewPath}\\menuLayout.cshtml";
                var menuLayout = context.Pages.FirstOrDefault(c => c.ViewPath == path);
                if (menuLayout == null)
                {
                    menuLayout = new Page
                    {
                        ViewPath = path
                    };
                    context.Pages.Add(menuLayout);
                }
                menuLayout.ViewName = $"{app.Name} layout";
                menuLayout.ViewContent = GetApplicationMenu(app, blockMapping).Item1;

                masterApp.IsPublished = true;
                masterApp.MenuChangedSinceLastBuild = false;
                masterContext.SaveChanges();

                File.WriteAllText(path, menuLayout.ViewContent);

                progressHandler.SetMessage(type: MessageType.Success, message: "Generating menu completed");
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Menu is not necessary to generate");
        }
        private void BuildHermes()
        {
            progressHandler.SetActiveSection(EModule.Hermes);

            if (context != masterContext)
            {
                try
                {
                    progressHandler.SetMessage(type: MessageType.Info, message: "Copying email templates");

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
                    context.SaveChanges();

                    progressHandler.SetMessage(type: MessageType.Success, message: "Copying email templates completed");
                }
                catch (Exception ex)
                {
                    progressHandler.Error(ex.Message);
                }
            }
            else
                progressHandler.SetMessage(type: MessageType.Info, message: "Copying email templates is not necessary to generate");
        }

        private Tuple<string, HashSet<string>> GetApplicationMenu(Application app, Dictionary<TapestryDesignerBlock, Block> blockMapping, int rootMetablockId = 0, int level = 0)
        {
            HashSet<string> rights = new HashSet<string>();
            if (rootMetablockId == 0)
                rootMetablockId = app.TapestryDesignerRootMetablock.Id;

            List<TapestryDesignerMenuItem> items = new List<TapestryDesignerMenuItem>();
            foreach (TapestryDesignerMetablock m in core.Context.TapestryDesignerMetablocks.Include("ParentMetablock").Where(m => m.ParentMetablock.Id == rootMetablockId && m.IsInMenu == true && !m.IsDeleted))
            {
                var menuResult = GetApplicationMenu(app, blockMapping, m.Id, level + 1);
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

            foreach (TapestryDesignerBlock b in core.Context.TapestryDesignerBlocks.Include("ParentMetablock").Where(b => !b.IsDeleted && b.ParentMetablock.Id == rootMetablockId && b.IsInMenu == true))
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
                    BlockName = blockMapping[b].Name,
                    rights = commit != null && !string.IsNullOrEmpty(commit.RoleWhitelist) ? commit.RoleWhitelist : "User"
                });
            }

            /*ViewDataDictionary ViewData = new ViewDataDictionary();   
            ViewData.Model = items;
            ViewData["Level"] = level;
            ViewData["ParentID"] = rootId;
            ViewData["AppId"] = core.Application.Id;
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
            ViewBag.AddValue("AppId", app.Id);
            ViewBag.AddValue("AppName", app.Name);
            ViewBag.AddValue("ParentId", rootMetablockId);

            string source = File.ReadAllText(_menuPath);

            string result = Engine.Razor.RunCompile(source, new Random().Next().ToString(), null, items, ViewBag);
            return new Tuple<string, HashSet<string>>(result, rights);
        }
    }
}
