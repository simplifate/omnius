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
        public ActionResult Index(string appName, int blockId = -1, int modelId = -1, string message = null, string messageType = null)
        {
            using (var context = new DBEntities())
            {
                // init
                C.CORE core = HttpContext.GetCORE();
                if(!string.IsNullOrEmpty(appName))
                    core.Entitron.AppName = appName;
                core.User = User.GetLogged(core);

                Block block = null;
                try
                {
                    block = context.Blocks.SingleOrDefault(b => b.Id == blockId) ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Entitron.AppId && w.InitBlockId != null).InitBlock;
                    core.Entitron.AppId = block.WorkFlow.ApplicationId;
                }
                catch (NullReferenceException)
                {
                    return new HttpStatusCodeResult(404);
                }

                ViewData["appName"] = core.Entitron.Application.DisplayName;
                ViewData["appIcon"] = core.Entitron.Application.Icon;
                ViewData["pageName"] = block.Name;

                foreach (var resourceMappingPair in block.ResourceMappingPairs)
                {
                    DataTable dataSource = new DataTable();
                    var columnDisplayNameDictionary = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(resourceMappingPair.Source.TableName))
                    {
                        string tableName = resourceMappingPair.Source.TableName;
                        var entitronTable = core.Entitron.GetDynamicTable(tableName);
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
                        {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            getAllColumns = false;
                        }
                        var entitronColumnList = entitronTable.columns.OrderBy(c => c.ColumnId).ToList();
                        dataSource.Columns.Add("hiddenId", typeof(int));
                        foreach (var entitronColumn in entitronColumnList)
                        {
                            if (getAllColumns || columnFilter.Contains(entitronColumn.Name))
                            {
                                var columnMetadata = core.Entitron.Application.ColumnMetadata.FirstOrDefault(c => c.TableName == entitronTable.tableName
                                    && c.ColumnName == entitronColumn.Name);
                                if (columnMetadata != null && columnMetadata.ColumnDisplayName != null)
                                {
                                    dataSource.Columns.Add(columnMetadata.ColumnDisplayName);
                                    columnDisplayNameDictionary.Add(entitronColumn.Name, columnMetadata.ColumnDisplayName);
                                }
                                else
                                {
                                    dataSource.Columns.Add(entitronColumn.Name);
                                    columnDisplayNameDictionary.Add(entitronColumn.Name, entitronColumn.Name);
                                }
                            }
                        }
                        var entitronRowList = entitronTable.Select().ToList();
                        foreach (var entitronRow in entitronRowList)
                        {
                            var newRow = dataSource.NewRow();
                            newRow["hiddenId"] = (int)entitronRow["id"];
                            foreach (var entitronColumn in entitronColumnList)
                            {
                                if (getAllColumns || columnFilter.Contains(entitronColumn.Name))
                                {
                                    if (entitronColumn.type == "bit")
                                    {
                                        if ((bool)entitronRow[entitronColumn.Name] == true)
                                            newRow[columnDisplayNameDictionary[entitronColumn.Name]] = "Ano";
                                        else
                                            newRow[columnDisplayNameDictionary[entitronColumn.Name]] = "Ne";
                                    }
                                    else
                                        newRow[columnDisplayNameDictionary[entitronColumn.Name]] = entitronRow[entitronColumn.Name];
                                }
                            }
                            if (!dataSource.Columns.Contains("IsActive") || (string)newRow["IsActive"] == "Ano")
                                dataSource.Rows.Add(newRow);
                        }

                    }
                    if (resourceMappingPair.TargetType == "data-table-read-only" || resourceMappingPair.TargetType == "data-table-with-actions"
                        || resourceMappingPair.TargetType == "name-value-list")
                    {
                        ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
                    }
                    else if (resourceMappingPair.TargetType == "dropdown-select")
                    {
                        var dropdownDictionary = new Dictionary<int, string>();
                        foreach (DataRow datarow in dataSource.Rows)
                        {
                            dropdownDictionary.Add((int)datarow["hiddenId"], (string)datarow[columnDisplayNameDictionary["Name"]]);
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
                var result = core.Tapestry.run(HttpContext.GetLoggedUser(), appName, block, button, modelId, fc);

                return RedirectToRoute("Run", new { appName = appName, blockId = result.Item2.Id, message = result.Item1.ToUser(), messageType = result.Item1.Type.ToString() });
            }
        }
    }
}
