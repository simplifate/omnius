using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using TapestryModule = FSS.Omnius.Modules.Tapestry;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        // Run Tapestry & show page
        [HttpPost]
        public ActionResult Index(string appName, int blockId, string buttonId, FormCollection fc, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;

            core._form = fc;

            User user = User.GetLogged(core);
            Block targetBlock = core.Tapestry.run(user, appName, blockId, buttonId, modelId, fc);

            return ShowPage(core, appName, targetBlock, modelId);
        }

        // Only show page
        [HttpGet]
        public ActionResult Index(string appName, int blockId = -1, int modelId = -1)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = appName;
            DBEntities e = core.Entitron.GetStaticTables();
            Block block = blockId > 0
                ? e.Blocks.SingleOrDefault(b => b.Id == blockId)
                : e.WorkFlows.SingleOrDefault(wf => wf.ApplicationId == core.Entitron.AppId && wf.Type.Name == "Init").InitBlock;
            
            if(!string.IsNullOrEmpty(appName)) {
                ViewData["appMenu"] = GetApplicationMenu(e, appName);
            }
            
            return ShowPage(core, appName, block, modelId);
        }

        private ActionResult ShowPage(Modules.CORE.CORE core, string appName, Block block, int modelId)
        {
            // init
            core.User = User.GetLogged(core);
            DBItem model = modelId > 0 ? core.Entitron.GetDynamicItem(block.ModelName, modelId) : null; // can be null

            // preRun
            TapestryModule.ActionResultCollection results = new TapestryModule.ActionResultCollection();
            results.outputData.Add("__CORE__", core);
            results.outputData.Add("__MODEL__", model);
            block.Run(results);

            //
            foreach(var pair in results.outputData)
            {
                ViewData.Add(pair);
            }

            using (var context = new DBEntities())
            {
                foreach (var resourceMappingPair in block.ResourceMappingPairs)
                {
                    DataTable dataSource = new DataTable();
                    if (resourceMappingPair.Source.TableId != null)
                    {
                        string tableName = context.DbTables.Find(resourceMappingPair.Source.TableId).Name;
                        core.Entitron.AppId = block.WorkFlow.ApplicationId;
                        var entitronTable = core.Entitron.GetDynamicTable(tableName);
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
                        {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            getAllColumns = false;
                        }

                        var entitronColumnList = entitronTable.columns.OrderBy(c => c.ColumnId).ToList();
                        dataSource.Columns.Add("hiddenId");
                        foreach (var entitronColumn in entitronColumnList)
                        {
                            if(getAllColumns || columnFilter.Contains(entitronColumn.Name))
                                dataSource.Columns.Add(entitronColumn.Name);
                        }
                        var entitronRowList = entitronTable.Select().ToList();
                        foreach (var entitronRow in entitronRowList)
                        {
                            var newRow = dataSource.NewRow();
                            newRow["hiddenId"] = entitronRow["id"];
                            foreach (var entitronColumn in entitronColumnList)
                            {
                                if (getAllColumns || columnFilter.Contains(entitronColumn.Name))
                                {
                                    if(entitronColumn.type == "bit")
                                    {
                                        if ((bool)entitronRow[entitronColumn.Name] == true)
                                            newRow[entitronColumn.Name] = "Ano";
                                        else
                                            newRow[entitronColumn.Name] = "Ne";
                                    }
                                    else
                                        newRow[entitronColumn.Name] = entitronRow[entitronColumn.Name];
                                }
                            }
                            if(!dataSource.Columns.Contains("IsActive") || (string)newRow["IsActive"] == "Ano")
                                dataSource.Rows.Add(newRow);
                        }

                    }
                    if (resourceMappingPair.TargetType == "data-table-read-only" || resourceMappingPair.TargetType == "data-table-with-actions")
                    {
                        ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
                    }
                    else if (resourceMappingPair.TargetType == "dropdown-select")
                    {
                        var dropdownDictionary = new Dictionary<int, string>();
                        foreach(DataRow datarow in dataSource.Rows)
                        {
                            dropdownDictionary.Add(int.Parse((string)datarow["id"]), (string)datarow["Name"]);
                        }
                        ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
                    }
                }
            }

            // show
            return View(block.MozaicPage.ViewPath);
        }

        public ActionResult Generate(string AppName)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            core.Entitron.AppName = AppName;

            TapestryGeneratorService service = new TapestryGeneratorService();
            service.GenerateTapestry(core);

            return null;
        }

        private string GetApplicationMenu(DBEntities e, string appName, int rootId = 0, int level = 0)
        {
            Modules.CORE.CORE core = HttpContext.GetCORE();
            rootId = rootId == 0 ? core.Entitron.Application.TapestryDesignerRootMetablock.Id : rootId;

            List<TapestryDesignerMenuItem> items = new List<TapestryDesignerMenuItem>();
            foreach (TapestryDesignerMetablock m in e.TapestryDesignerMetablocks.Include("ParentMetablock").Where(m => m.ParentMetablock.Id == rootId && m.IsInMenu == true)) {
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = m.Id,
                    Name = m.Name,
                    SubMenu = GetApplicationMenu(e, appName, m.Id, level + 1),
                    IsInitial = m.IsInitial,
                    IsInMenu = m.IsInMenu,
                    MenuOrder = m.MenuOrder,
                    IsMetablock = true
                });
            }

            foreach(TapestryDesignerBlock b in e.TapestryDesignerBlocks.Include("ParentMetablock").Where(b => b.ParentMetablock.Id == rootId && b.IsInMenu == true)) {
                items.Add(new TapestryDesignerMenuItem()
                {
                    Id = b.Id,
                    Name = b.Name,
                    IsInitial = b.IsInitial,
                    IsInMenu = b.IsInMenu,
                    MenuOrder = b.MenuOrder,
                    IsBlock = true,
                });
            }

            ViewData.Model = items;
            ViewData["Level"] = level;
            using (var sw = new StringWriter()) {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, "~/Views/Shared/_ApplicationMenu.cshtml");
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
