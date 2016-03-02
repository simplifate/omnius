using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using T = FSS.Omnius.Modules.Tapestry;
using C = FSS.Omnius.Modules.CORE;
using E = FSS.Omnius.Modules.Entitron;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        [HttpGet]
        public ActionResult Index(string appName, int blockId = -1, int modelId = -1)
        {
            using (var context = new DBEntities())
            {
                // init
                C.CORE core = HttpContext.GetCORE();
                core.Entitron.AppName = appName;
                core.User = User.GetLogged(core);

                Block block = context.Blocks.SingleOrDefault(b => b.Id == blockId) ?? context.WorkFlows.FirstOrDefault(w => w.Application.Name == appName && w.Type.Name == "Init").InitBlock;
            
                ViewData["appName"] = core.Entitron.Application.DisplayName;
                ViewData["appIcon"] = core.Entitron.Application.Icon;
                ViewData["pageName"] = block.Name;

                foreach (var resourceMappingPair in block.ResourceMappingPairs)
                {
                    DataTable dataSource = new DataTable();
                    if (resourceMappingPair.Source.TableId != null)
                    {
                        string tableName = context.DbTables.Find(resourceMappingPair.Source.TableId).Name;
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
                            if (getAllColumns || columnFilter.Contains(entitronColumn.Name))
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
                                    if (entitronColumn.type == "bit")
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
                            if (!dataSource.Columns.Contains("IsActive") || (string)newRow["IsActive"] == "Ano")
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
                        foreach (DataRow datarow in dataSource.Rows)
                        {
                            dropdownDictionary.Add(int.Parse((string)datarow["id"]), (string)datarow["Name"]);
                        }
                        ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
                    }
                }

                // show
                return View(block.MozaicPage.ViewPath);
            }
        }
        [HttpPost]
        public ActionResult Index(string appName, string button, FormCollection fc, int blockId = -1, int modelId = -1)
        {
            C.CORE core = HttpContext.GetCORE();
            using (DBEntities context = new DBEntities())
            {
                Block block = context.Blocks.SingleOrDefault(b => b.Id == blockId) ?? context.WorkFlows.FirstOrDefault(w => w.Application.Name == appName && w.Type.Name == "Init").InitBlock;
                Block targetBlock = core.Tapestry.run(HttpContext.GetLoggedUser(), appName, block, button, modelId, fc);

                return RedirectToRoute("Run", new { appName = appName, blockId = targetBlock.Id });
            }
        }
        //// Run Tapestry & show page
        //[HttpPost]
        //public ActionResult Index(string appName, int blockId, string buttonId, FormCollection fc, int modelId = -1)
        //{
        //    Modules.CORE.CORE core = HttpContext.GetCORE();
        //    core.Entitron.AppName = appName;

        //    core._form = fc;

        //    User user = User.GetLogged(core);
        //    Block targetBlock = core.Tapestry.run(user, appName, blockId, buttonId, modelId, fc);

        //    return ShowPage(core, appName, targetBlock, modelId);
        //}

        //// Only show page
        //[HttpGet]
        //public ActionResult Index(string appName, int blockId = -1, int modelId = -1)
        //{
        //    Modules.CORE.CORE core = HttpContext.GetCORE();
        //    core.Entitron.AppName = appName;
        //    DBEntities e = core.Entitron.GetStaticTables();
        //    Block block = blockId > 0
        //        ? e.Blocks.SingleOrDefault(b => b.Id == blockId)
        //        : e.WorkFlows.SingleOrDefault(wf => wf.ApplicationId == core.Entitron.AppId && wf.Type.Name == "Init").InitBlock;
            
        //    return ShowPage(core, appName, block, modelId);
        //}

        //private ActionResult ShowPage(Modules.CORE.CORE core, string appName, Block block, int modelId)
        //{
        //    if (string.IsNullOrEmpty(appName) && block == null) // Requested block Id not found
        //        return new HttpStatusCodeResult(404);

        //    // init
        //    core.User = User.GetLogged(core);
        //    core.Entitron.AppId = block.WorkFlow.ApplicationId;
        //    DBItem model = modelId > 0 ? core.Entitron.GetDynamicItem(block.ModelName, modelId) : null; // can be null

        //    // preRun
        //    TapestryModule.ActionResultCollection results = new TapestryModule.ActionResultCollection();
        //    results.outputData.Add("__CORE__", core);
        //    results.outputData.Add("__MODEL__", model);
        //    block.Run(results);

        //    //
        //    foreach(var pair in results.outputData)
        //    {
        //        ViewData.Add(pair);
        //    }

        //    using (var context = new DBEntities())
        //    {
        //        ViewData["appName"] = core.Entitron.Application.DisplayName;
        //        ViewData["appIcon"] = core.Entitron.Application.Icon;
        //        ViewData["pageName"] = block.Name;

        //        foreach (var resourceMappingPair in block.ResourceMappingPairs)
        //        {
        //            DataTable dataSource = new DataTable();
        //            if (resourceMappingPair.Source.TableId != null)
        //            {
        //                string tableName = context.DbTables.Find(resourceMappingPair.Source.TableId).Name;
        //                var entitronTable = core.Entitron.GetDynamicTable(tableName);
        //                List<string> columnFilter = null;
        //                bool getAllColumns = true;
        //                if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
        //                {
        //                    columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
        //                    getAllColumns = false;
        //                }

        //                var entitronColumnList = entitronTable.columns.OrderBy(c => c.ColumnId).ToList();
        //                dataSource.Columns.Add("hiddenId");
        //                foreach (var entitronColumn in entitronColumnList)
        //                {
        //                    if(getAllColumns || columnFilter.Contains(entitronColumn.Name))
        //                        dataSource.Columns.Add(entitronColumn.Name);
        //                }
        //                var entitronRowList = entitronTable.Select().ToList();
        //                foreach (var entitronRow in entitronRowList)
        //                {
        //                    var newRow = dataSource.NewRow();
        //                    newRow["hiddenId"] = entitronRow["id"];
        //                    foreach (var entitronColumn in entitronColumnList)
        //                    {
        //                        if (getAllColumns || columnFilter.Contains(entitronColumn.Name))
        //                        {
        //                            if(entitronColumn.type == "bit")
        //                            {
        //                                if ((bool)entitronRow[entitronColumn.Name] == true)
        //                                    newRow[entitronColumn.Name] = "Ano";
        //                                else
        //                                    newRow[entitronColumn.Name] = "Ne";
        //                            }
        //                            else
        //                                newRow[entitronColumn.Name] = entitronRow[entitronColumn.Name];
        //                        }
        //                    }
        //                    if(!dataSource.Columns.Contains("IsActive") || (string)newRow["IsActive"] == "Ano")
        //                        dataSource.Rows.Add(newRow);
        //                }

        //            }
        //            if (resourceMappingPair.TargetType == "data-table-read-only" || resourceMappingPair.TargetType == "data-table-with-actions")
        //            {
        //                ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
        //            }
        //            else if (resourceMappingPair.TargetType == "dropdown-select")
        //            {
        //                var dropdownDictionary = new Dictionary<int, string>();
        //                foreach(DataRow datarow in dataSource.Rows)
        //                {
        //                    dropdownDictionary.Add(int.Parse((string)datarow["id"]), (string)datarow["Name"]);
        //                }
        //                ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
        //            }
        //        }
        //    }

        //    // show
        //    return View(block.MozaicPage.ViewPath);
        //}

        //public ActionResult Generate(string AppName)
        //{
        //    Modules.CORE.CORE core = HttpContext.GetCORE();
        //    core.Entitron.AppName = AppName;

        //    TapestryGeneratorService service = new TapestryGeneratorService();
        //    service.GenerateTapestry(core);

        //    return null;
        //}
    }
}
