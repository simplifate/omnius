using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Tapestry.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using C = FSS.Omnius.Modules.CORE;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        public static DateTime requestStart;
        public static DateTime startTime;
        public static DateTime prepareEnd;

        [HttpGet]
        public ActionResult Index(string appName, string blockIdentify = null, int modelId = -1, string message = null, string messageType = null, string registry = null)
        {
            C.CORE core = HttpContext.GetCORE();
            DBEntities context = core.Entitron.GetStaticTables();
            core.Entitron.AppName = appName;

            Block block = getBlockWithResource(core, context, appName, blockIdentify);
            if (block == null)
                return new HttpStatusCodeResult(404);

            // Check if user has one of allowed roles, otherwise return 403
            if (!String.IsNullOrEmpty(block.RoleWhitelist))
            {
                User user = core.User;
                bool userIsAllowed = false;
                foreach (var role in block.RoleWhitelist.Split(',').ToList())
                {
                    if (user.HasRole(role, context))
                        userIsAllowed = true;
                }
                if (!userIsAllowed)
                    return new HttpStatusCodeResult(403);
            }
            
            var result = core.Tapestry.innerRun(HttpContext.GetLoggedUser(), block, "INIT", modelId, null);
            if (result.Item2.Id != block.Id)
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId });
            ViewData["tapestry"] = result.Item1.OutputData;

            // fill data
            ViewData["appName"] = core.Entitron.Application.DisplayName;
            ViewData["appIcon"] = core.Entitron.Application.Icon;
            ViewData["pageName"] = block.DisplayName;
            ViewData["blockName"] = block.Name;
            ViewData["userName"] = core.User.DisplayName;
            ViewData["crossBlockRegistry"] = registry;

            DBItem modelRow = null;
            if (modelId != -1 && !string.IsNullOrEmpty(block.ModelName))
            {
                modelRow = core.Entitron.GetDynamicTable(block.ModelName).GetById(modelId);
            }
            foreach (var resourceMappingPair in context.ResourceMappingPairs.Include("Source").Where(mp => mp.BlockId == block.Id).ToList())
            {
                DataTable dataSource = new DataTable();
                var columnDisplayNameDictionary = new Dictionary<string, string>();
                var source = resourceMappingPair.Source;
                if (!string.IsNullOrEmpty(source.TableName)
                    && string.IsNullOrEmpty(source.ColumnName))
                {
                    string tableName = source.TableName;
                    if (source.Label.StartsWith("View:"))
                    {
                        var entitronView = core.Entitron.GetDynamicView(tableName);
                        dataSource.Columns.Add("hiddenId", typeof(int));
                        var entitronRowList = entitronView.Select().ToList();
                        bool columnsCreated = false;
                        foreach (var entitronRow in entitronRowList)
                        {
                            var newRow = dataSource.NewRow();
                            var columnNames = entitronRow.getColumnNames();
                            newRow["hiddenId"] = columnNames.Contains("id") ? entitronRow["id"] : -1;
                            foreach (var columnName in columnNames)
                            {
                                if (!columnsCreated)
                                    dataSource.Columns.Add(columnName);
                                if (entitronRow[columnName] is bool)
                                {
                                    if ((bool)entitronRow[columnName] == true)
                                        newRow[columnName] = "Ano";
                                    else
                                        newRow[columnName] = "Ne";
                                }
                                else if (entitronRow[columnName] is DateTime)
                                {
                                    newRow[columnName] = ((DateTime)entitronRow[columnName]).ToString("d. M. yyyy H:mm:ss");
                                }
                                else
                                    newRow[columnName] = entitronRow[columnName];
                            }
                            columnsCreated = true;
                            dataSource.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        var entitronTable = core.Entitron.GetDynamicTable(tableName);
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
                        {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            if (columnFilter.Count > 0)
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
                            if (source.ConditionSets.Count == 0
                                || core.Entitron.filteringService.MatchConditionSets(source.ConditionSets, entitronRow))
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
                                        else if (entitronRow[entitronColumn.Name] is DateTime)
                                        {
                                            newRow[columnDisplayNameDictionary[entitronColumn.Name]] = ((DateTime)entitronRow[entitronColumn.Name]).ToString("d. M. yyyy H:mm:ss");
                                        }
                                        else
                                            newRow[columnDisplayNameDictionary[entitronColumn.Name]] = entitronRow[entitronColumn.Name];
                                    }
                                }
                                dataSource.Rows.Add(newRow);
                            }
                        }
                    }
                }
                if (resourceMappingPair.TargetType == "data-table-read-only" || resourceMappingPair.TargetType == "data-table-with-actions"
                    || resourceMappingPair.TargetType == "name-value-list")
                {
                    ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
                }
                else if ((resourceMappingPair.TargetType == "dropdown-select" || resourceMappingPair.TargetType == "multiple-select") && string.IsNullOrEmpty(source.ColumnName))
                {
                    var dropdownDictionary = new Dictionary<int, string>();
                    foreach (DataRow datarow in dataSource.Rows)
                    {
                        dropdownDictionary.Add((int)datarow["hiddenId"], columnDisplayNameDictionary.ContainsKey("name")
                            ? (string)datarow[columnDisplayNameDictionary["name"]] : (string)datarow["name"]);
                    }
                    ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
                }
                if (modelRow != null && !string.IsNullOrEmpty(source.ColumnName)
                    && resourceMappingPair.TargetType == "checkbox")
                {
                    ViewData["checkboxData_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
                }
                else if (modelRow != null && !string.IsNullOrEmpty(source.ColumnName)
                    && (resourceMappingPair.TargetType == "input-single-line" || resourceMappingPair.TargetType == "input-multiline"))
                {
                    ViewData["inputData_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
                }
                else if (modelRow != null && !string.IsNullOrEmpty(source.ColumnName)
                    && (resourceMappingPair.TargetType == "dropdown-select" || resourceMappingPair.TargetType == "multiple-select"))
                {
                    ViewData["dropdownSelection_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
                }
                if (!string.IsNullOrEmpty(source.ColumnName) && resourceMappingPair.DataSourceParams == "currentUser"
                    && (resourceMappingPair.TargetType == "input-single-line" || resourceMappingPair.TargetType == "input-multiline"))
                {
                    if (source.TableName == "Omnius::Users")
                        switch (source.ColumnName)
                        {
                            case "DisplayName":
                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.DisplayName;
                                break;
                            case "Company":
                                var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                    .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                                if (epkUserRowList.Count > 0)
                                {
                                    var abkrsRowList = core.Entitron.GetDynamicTable("ABKRS").Select()
                                    .where(c => c.column("abkrs").Equal(epkUserRowList[0]["abkrs"])).ToList();
                                    if (abkrsRowList.Count > 0)
                                    {
                                        var companyRowList = core.Entitron.GetDynamicTable("Companies").Select()
                                                            .where(c => c.column("id_company").Equal(abkrsRowList[0]["id_company"])).ToList();
                                        if (companyRowList.Count > 0)
                                        {
                                            ViewData["inputData_" + resourceMappingPair.TargetName] = companyRowList[0]["name"];
                                        }
                                    }
                                }
                                break;
                            case "Job":
                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Job;
                                break;
                            case "Email":
                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Email;
                                break;
                            case "Address":
                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Address;
                                break;
                        }
                    else if (source.TableName == "Users")
                    {
                        var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                    .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                        if (epkUserRowList.Count > 0)
                            ViewData["inputData_" + resourceMappingPair.TargetName] = epkUserRowList[0][source.ColumnName];
                    }
                }
                else if (!string.IsNullOrEmpty(source.ColumnName) && resourceMappingPair.DataSourceParams == "superior"
                    && (resourceMappingPair.TargetType == "input-single-line" || resourceMappingPair.TargetType == "input-multiline"))
                {
                    var tableUsers = core.Entitron.GetDynamicTable("Users");
                    if (source.TableName == "Users")
                    {
                        var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                        if (epkUserRowList.Count > 0)
                        {
                            int superiorId = (int)epkUserRowList[0]["h_pernr"];
                            var epkSuperiorRowList = tableUsers.Select()
                                    .where(c => c.column("pernr").Equal(superiorId)).ToList();
                            if (epkSuperiorRowList.Count > 0)
                                ViewData["inputData_" + resourceMappingPair.TargetName] = epkSuperiorRowList[0][source.ColumnName];
                        }
                    }
                }
            }
            string viewPath = $"{core.Entitron.Application.Id}\\Page\\{block.EditorPageId}.cshtml";

            // show
            //return View(block.MozaicPage.ViewPath);
            return View(viewPath);
        }
        [HttpPost]
        public ActionResult Index(string appName, string button, FormCollection fc, string blockIdentify = null, int modelId = -1)
        {
            C.CORE core = HttpContext.GetCORE();
            DBEntities context = core.Entitron.GetStaticTables();
            core.Entitron.AppName = appName;

            Block block = getBlockWithWF(core, context, appName, blockIdentify);
            if (block == null)
                return new HttpStatusCodeResult(404);

            var crossBlockRegistry = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(fc["registry"]))
                crossBlockRegistry = JsonConvert.DeserializeObject<Dictionary<string, object>>(fc["registry"]);
            foreach (var pair in crossBlockRegistry)
            {
                fc.Add(pair.Key, pair.Value.ToString());
            }

            // run
            var result = core.Tapestry.run(HttpContext.GetLoggedUser(), block, button, modelId, fc);

            if (Response.StatusCode != 202) {
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = result.Item1.ToUser(), messageType = result.Item1.Type.ToString(), registry = JsonConvert.SerializeObject(result.Item3) });
            }
            return null;
        }

        private Block getBlockWithResource(C.CORE core, DBEntities context, string appName, string blockName)
        {
            return blockName != null
                ? context.Blocks
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
                    .Include(b => b.SourceTo_ActionRules)
                    .SingleOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.Name == blockName)
                : context.Blocks
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
                    .Include(b => b.SourceTo_ActionRules)
                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.WorkFlow.InitBlockId == b.Id);
        }
        private Block getBlockWithWF(C.CORE core, DBEntities context, string appName, string blockName)
        {
            return blockName != null
                ? context.Blocks
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
                    .Include(b => b.SourceTo_ActionRules)
                    .SingleOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.Name == blockName)
                : context.Blocks.Include(b => b.ResourceMappingPairs).Include(b => b.SourceTo_ActionRules).FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.WorkFlow.InitBlockId == b.Id);
        }
    }
}
