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
        public ActionResult Index(string appName, string blockIdentify = null, int modelId = -1, string message = null, string messageType = null)
        {
            C.CORE core = HttpContext.GetCORE();
            DBEntities context = core.Entitron.GetStaticTables();
            core.Entitron.AppName = appName;
            core.CrossBlockRegistry = Session["CrossBlockRegistry"] == null ? new Dictionary<string, object>() : (Dictionary<string, object>)Session["CrossBlockRegistry"];

            Block block = getBlockWithResource(core, context, appName, blockIdentify);
            if (block == null)
                return new HttpStatusCodeResult(404);

            // Check if user has one of allowed roles, otherwise return 403
            if (!String.IsNullOrEmpty(block.RoleWhitelist)) {
                User user = core.User;
                bool userIsAllowed = false;
                foreach (var role in block.RoleWhitelist.Split(',').ToList()) {
                    if (user.HasRole(role, context))
                        userIsAllowed = true;
                }
                if (!userIsAllowed)
                    return new HttpStatusCodeResult(403);
            }
            
            FormCollection fc = new FormCollection();
            foreach (var pair in core.CrossBlockRegistry) {
                if (pair.Value != null) {
                    fc.Add(pair.Key, pair.Value.ToString());
                }
                else {
                    fc.Add(pair.Key, "");
                }
            }

            var result = core.Tapestry.innerRun(HttpContext.GetLoggedUser(), block, "INIT", modelId, fc);
            if (result.Item2.Id != block.Id)
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId });
            var tapestryVars = result.Item1.OutputData;

            foreach (var pair in result.Item1.OutputData) {
                if (pair.Key.StartsWith("_uic_")) {
                    ViewData[pair.Key.Substring(5)] = pair.Value;
                }
                if (pair.Key.StartsWith("_uictable_")) {
                    var entitronRowList = (List<DBItem>)pair.Value;
                    bool columnsCreated = false;
                    DataTable dataSource = new DataTable();
                    dataSource.Columns.Add("hiddenId", typeof(int));
                    foreach (var entitronRow in entitronRowList) {
                        var newRow = dataSource.NewRow();
                        var columnNames = entitronRow.getColumnNames();
                        newRow["hiddenId"] = columnNames.Contains("id") ? entitronRow["id"] : -1;
                        foreach (var columnName in columnNames) {
                            if (!columnsCreated)
                                dataSource.Columns.Add(columnName);
                            if (entitronRow[columnName] is bool) {
                                if ((bool)entitronRow[columnName] == true)
                                    newRow[columnName] = "Ano";
                                else
                                    newRow[columnName] = "Ne";
                            }
                            else if (entitronRow[columnName] is DateTime) {
                                newRow[columnName] = ((DateTime)entitronRow[columnName]).ToString("d. M. yyyy H:mm:ss");
                            }
                            else
                                newRow[columnName] = entitronRow[columnName];
                        }
                        columnsCreated = true;
                        dataSource.Rows.Add(newRow);
                    }
                    ViewData[pair.Key.Substring(10)] = dataSource;
                }
            }

            // fill data
            ViewData["appName"] = core.Entitron.Application.DisplayName;
            ViewData["appIcon"] = core.Entitron.Application.Icon;
            ViewData["pageName"] = block.DisplayName;
            ViewData["blockName"] = block.Name;
            ViewData["userName"] = core.User.DisplayName;
            ViewData["crossBlockRegistry"] = ""; /// !!!

            DBItem modelRow = null;
            if (modelId != -1 && !string.IsNullOrEmpty(block.ModelName)) {
                modelRow = core.Entitron.GetDynamicTable(block.ModelName).GetById(modelId);
            }
            foreach (var resourceMappingPair in context.ResourceMappingPairs.Where(mp => mp.BlockId == block.Id).ToList()) {
                DataTable dataSource = new DataTable();
                List<TapestryDesignerConditionSet> conditionSets = context.TapestryDesignerConditionSets.Where(cs => cs.ResourceMappingPair_Id == resourceMappingPair.Id).ToList();
                var columnDisplayNameDictionary = new Dictionary<string, string>();

                string targetType = resourceMappingPair.TargetType;
                string targetName = resourceMappingPair.TargetName;
                string sourceColumnName = resourceMappingPair.SourceColumnName;
                string sourceTableName = resourceMappingPair.SourceTableName;


                if (!string.IsNullOrEmpty(resourceMappingPair.SourceTableName)
                    && string.IsNullOrEmpty(resourceMappingPair.SourceColumnName)) {
                    if (resourceMappingPair.relationType.StartsWith("V:")) {
                        dataSource = new DataTable();
                        columnDisplayNameDictionary = new Dictionary<string, string>();
                        var entitronView = core.Entitron.GetDynamicView(sourceTableName);
                        dataSource.Columns.Add("hiddenId", typeof(int));
                        var entitronRowList = entitronView.Select().ToList();
                        bool noConditions = conditionSets.Count == 0;
                        bool idAvailable = false;
                        List<string> columnNameList = null;

                        if (entitronRowList.Count > 0) {
                            columnNameList = entitronRowList[0].getColumnNames();
                            idAvailable = columnNameList.Contains("id");
                        }
                        else {
                            continue;
                        }
                        foreach (var columnName in columnNameList) {
                            dataSource.Columns.Add(columnName);
                            columnDisplayNameDictionary.Add(columnName, columnName);
                        }
                        foreach (var entitronRow in entitronRowList) {
                            if (noConditions || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars)) {
                                var newRow = dataSource.NewRow();
                                newRow["hiddenId"] = idAvailable ? entitronRow["id"] : 0;
                                foreach (var columnName in columnNameList) {
                                    var currentCell = entitronRow[columnName];
                                    if (currentCell is bool) {
                                        if ((bool)currentCell == true)
                                            newRow[columnName] = "Ano";
                                        else
                                            newRow[columnName] = "Ne";
                                    }
                                    else if (currentCell is DateTime) {
                                        newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
                                    }
                                    else
                                        newRow[columnName] = currentCell;
                                }
                                dataSource.Rows.Add(newRow);
                            }
                        }
                    }
                    else {
                        bool disableColumnAlias = (resourceMappingPair.TargetType == "form|select" || resourceMappingPair.TargetType == "dropdown-select" || resourceMappingPair.TargetType == "multiple-select");
                        var entitronTable = core.Entitron.GetDynamicTable(resourceMappingPair.SourceTableName);
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter)) {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            if (columnFilter.Count > 0)
                                getAllColumns = false;
                        }
                        var entitronColumnList = entitronTable.columns.OrderBy(c => c.ColumnId).ToList();
                        dataSource.Columns.Add("hiddenId", typeof(int));
                        if (disableColumnAlias) {
                            foreach (var entitronColumn in entitronColumnList) {
                                dataSource.Columns.Add(entitronColumn.Name);
                            }
                            var entitronRowList = entitronTable.Select().ToList();
                            foreach (var entitronRow in entitronRowList) {
                                if (conditionSets.Count == 0
                                    || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars)) {
                                    var newRow = dataSource.NewRow();
                                    newRow["hiddenId"] = (int)entitronRow["id"];
                                    foreach (var entitronColumn in entitronColumnList) {
                                        if (getAllColumns || columnFilter.Contains(entitronColumn.Name)) {
                                            if (entitronColumn.type == "bit") {
                                                if ((bool)entitronRow[entitronColumn.Name] == true)
                                                    newRow[entitronColumn.Name] = "Ano";
                                                else
                                                    newRow[entitronColumn.Name] = "Ne";
                                            }
                                            else if (entitronRow[entitronColumn.Name] is DateTime) {
                                                newRow[entitronColumn.Name] = ((DateTime)entitronRow[entitronColumn.Name]).ToString("d. M. yyyy H:mm:ss");
                                            }
                                            else
                                                newRow[entitronColumn.Name] = entitronRow[entitronColumn.Name];
                                        }
                                    }
                                    dataSource.Rows.Add(newRow);
                                }
                            }
                        }
                        else {
                            foreach (var entitronColumn in entitronColumnList) {
                                if (getAllColumns || columnFilter.Contains(entitronColumn.Name)) {
                                    var columnMetadata = core.Entitron.Application.ColumnMetadata.FirstOrDefault(c => c.TableName == entitronTable.tableName
                                        && c.ColumnName == entitronColumn.Name);
                                    if (columnMetadata != null && columnMetadata.ColumnDisplayName != null) {
                                        dataSource.Columns.Add(columnMetadata.ColumnDisplayName);
                                        columnDisplayNameDictionary.Add(entitronColumn.Name, columnMetadata.ColumnDisplayName);
                                    }
                                    else {
                                        dataSource.Columns.Add(entitronColumn.Name);
                                        columnDisplayNameDictionary.Add(entitronColumn.Name, entitronColumn.Name);
                                    }
                                }
                            }
                            var entitronRowList = entitronTable.Select().ToList();
                            foreach (var entitronRow in entitronRowList) {
                                if (conditionSets.Count == 0
                                    || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars)) {
                                    var newRow = dataSource.NewRow();
                                    newRow["hiddenId"] = (int)entitronRow["id"];
                                    foreach (var entitronColumn in entitronColumnList) {
                                        if (getAllColumns || columnFilter.Contains(entitronColumn.Name)) {
                                            if (entitronColumn.type == "bit") {
                                                if ((bool)entitronRow[entitronColumn.Name] == true)
                                                    newRow[columnDisplayNameDictionary[entitronColumn.Name]] = "Ano";
                                                else
                                                    newRow[columnDisplayNameDictionary[entitronColumn.Name]] = "Ne";
                                            }
                                            else if (entitronRow[entitronColumn.Name] is DateTime) {
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
                }

                if (ResourceMappingPairsTarget.IsTable(targetType)) {
                    ViewData["tableData_" + targetName] = dataSource;
                }
                else if (ResourceMappingPairsTarget.IsSelect(targetType) && string.IsNullOrEmpty(sourceColumnName)) {
                    var dropdownDictionary = new Dictionary<int, string>();
                    if (dataSource.Rows.Count > 0 && dataSource.Columns.Contains("name")) {
                        foreach (DataRow datarow in dataSource.Rows) {
                            if (datarow["name"] != DBNull.Value) {
                                dropdownDictionary.Add((int)datarow["hiddenId"], columnDisplayNameDictionary.ContainsKey("name")
                                    ? (string)datarow[columnDisplayNameDictionary["name"]] : (string)datarow["name"]);
                            }
                        }
                        ViewData["dropdownData_" + targetName] = dropdownDictionary;
                    }
                    else
                        ViewData["dropdownData_" + targetName] = null;
                }

                // Máme model a mapujeme data do inputů
                if (modelRow != null && !string.IsNullOrEmpty(resourceMappingPair.SourceColumnName)) {
                    switch (ResourceMappingPairsTarget.GetType(targetType)) {
                        case ResourceMappingPairsTarget.CHECKBOX: {
                                ViewData["checkboxData_" + targetName] = modelRow[sourceColumnName];
                                break;
                            }
                        case ResourceMappingPairsTarget.RADIO: {
                                ViewData["radioData_" + targetName] = modelRow[sourceColumnName];
                                break;
                            }
                        case ResourceMappingPairsTarget.INPUT: {
                                ViewData["inputData_" + targetName] = modelRow[sourceColumnName];
                                break;
                            }
                        case ResourceMappingPairsTarget.SELECT: {
                                ViewData["dropdownSelection_" + targetName] = modelRow[sourceColumnName];
                                break;
                            }

                    }
                }

                if (!string.IsNullOrEmpty(sourceColumnName) && resourceMappingPair.DataSourceParams == "currentUser"
                    && ResourceMappingPairsTarget.IsInput(targetType)) {
                    if (sourceTableName == "Omnius::Users") {
                        switch (sourceColumnName) {
                            case "DisplayName":
                                ViewData["inputData_" + targetName] = core.User.DisplayName;
                                break;
                            case "Company":
                                var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                    .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                                if (epkUserRowList.Count > 0) {
                                    var abkrsRowList = core.Entitron.GetDynamicTable("ABKRS").Select()
                                    .where(c => c.column("abkrs").Equal(epkUserRowList[0]["abkrs"])).ToList();
                                    if (abkrsRowList.Count > 0) {
                                        var companyRowList = core.Entitron.GetDynamicTable("Companies").Select()
                                                            .where(c => c.column("id_company").Equal(abkrsRowList[0]["id_company"])).ToList();
                                        if (companyRowList.Count > 0) {
                                            ViewData["inputData_" + targetName] = companyRowList[0]["name"];
                                        }
                                    }
                                }
                                break;
                            case "Job":
                                ViewData["inputData_" + targetName] = core.User.Job;
                                break;
                            case "Email":
                                ViewData["inputData_" + targetName] = core.User.Email;
                                break;
                            case "Address":
                                ViewData["inputData_" + targetName] = core.User.Address;
                                break;
                        }
                    }
                    else if (sourceTableName == "Users") {
                        var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                    .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                        if (epkUserRowList.Count > 0)
                            ViewData["inputData_" + targetName] = epkUserRowList[0][sourceColumnName];
                    }
                }
                else if (!string.IsNullOrEmpty(sourceColumnName) && resourceMappingPair.DataSourceParams == "superior"
                    && ResourceMappingPairsTarget.IsInput(targetType)) {
                    var tableUsers = core.Entitron.GetDynamicTable("Users");
                    if (resourceMappingPair.SourceTableName == "Users") {
                        var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                        if (epkUserRowList.Count > 0) {
                            int superiorId = (int)epkUserRowList[0]["h_pernr"];
                            var epkSuperiorRowList = tableUsers.Select()
                                    .where(c => c.column("pernr").Equal(superiorId)).ToList();
                            if (epkSuperiorRowList.Count > 0)
                                ViewData["inputData_" + targetName] = epkSuperiorRowList[0][sourceColumnName];
                        }
                    }
                }
            }
            string boostrapPath = block.BootstrapPageId != null ? $"Bootstrap\\" : "";
            var pageId = block.BootstrapPageId ?? block.EditorPageId;
            string viewPath = $"{core.Entitron.Application.Id}\\Page\\{boostrapPath}{pageId}.cshtml";

            Session["CrossBlockRegistry"] = core.CrossBlockRegistry;

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
            core.CrossBlockRegistry = Session["CrossBlockRegistry"] == null ? new Dictionary<string, object>() : (Dictionary<string, object>)Session["CrossBlockRegistry"];

            Block block = getBlockWithWF(core, context, appName, blockIdentify);
            if (block == null)
                return new HttpStatusCodeResult(404);

            foreach (var pair in core.CrossBlockRegistry) {
                if (pair.Value != null) {
                    fc.Add(pair.Key, pair.Value.ToString());
                }
                else {
                    fc.Add(pair.Key, "");
                }
            }

            // run
            var result = core.Tapestry.run(HttpContext.GetLoggedUser(), block, button, modelId, fc);

            Session["CrossBlockRegistry"] = core.CrossBlockRegistry;

            if (Response.StatusCode != 202) {
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = result.Item1.ToUser(), messageType = result.Item1.Type.ToString(), registry = JsonConvert.SerializeObject(result.Item3) });
            }
            return null;
        }

        private Block getBlockWithResource(C.CORE core, DBEntities context, string appName, string blockName)
        {
            return blockName != null
                ? context.Blocks
                    .Include(b => b.SourceTo_ActionRules)
                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.Name == blockName)
                : context.Blocks
                    .Include(b => b.SourceTo_ActionRules)
                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.WorkFlow.InitBlockId == b.Id);
        }
        private Block getBlockWithWF(C.CORE core, DBEntities context, string appName, string blockName)
        {
            return blockName != null
                ? context.Blocks
                    .Include(b => b.SourceTo_ActionRules)
                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.Name == blockName)
                : context.Blocks.Include(b => b.ResourceMappingPairs).Include(b => b.SourceTo_ActionRules).FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Entitron.AppId && b.WorkFlow.InitBlockId == b.Id);
        }
    }

    static class ResourceMappingPairsTarget
    {
        public const string TABLE = "TABLE";
        public static List<string> Table = new List<string>()
        {
            // LEGACY
            "data-table-read-only",
            "data-table-with-actions",
            "name-value-list",              
            // BOOTSTRAP
            "ui|data-table",
            "ui|nv-list"
        };

        public const string SELECT = "select";
        public static List<string> Select = new List<string>()
        {
            // LEGACY
            "dropdown-select",
            "multiple-select",  
            // BOOTSTRAP
            "form|select"
        };

        public const string CHECKBOX = "checkbox";
        public static List<string> Checkbox = new List<string>()
        {
            // LEGACY
            "checkbox",     
            // BOOTSTRAP
            "form|checkbox"
        };

        public const string RADIO = "radio";
        public static List<string> Radio = new List<string>()
        {
            // BOOTSTRAP
            "form|radio"
        };

        public const string INPUT = "input";
        public static List<string> Input = new List<string>()
        {
            // LEGACY
            "input-single-line",
            "input-multiline",      
            // BOOTSTRAP
            "form|input-text",
            "form|input-email",
            "form|input-color",
            "form|input-tel",
            "form|input-date",
            "form|input-number",
            "form|input-range",
            "form|input-hidden",
            "form|input-url",
            "form|input-search",
            "form|input-password",
            "form|textarea"
        };

        public static string GetType(string targetType)
        {
            if (Table.Contains(targetType)) { return TABLE; }
            if (Select.Contains(targetType)) { return SELECT; }
            if (Checkbox.Contains(targetType)) { return CHECKBOX; }
            if (Radio.Contains(targetType)) { return RADIO; }
            if (Input.Contains(targetType)) { return INPUT; }

            return "";
        }

        public static bool IsTable(string targetType)
        {
            return Table.Contains(targetType);
        }

        public static bool IsSelect(string targetType)
        {
            return Select.Contains(targetType);
        }

        public static bool IsCheckbox(string targetType)
        {
            return Checkbox.Contains(targetType);
        }

        public static bool IsRadio(string targetType)
        {
            return Radio.Contains(targetType);
        }

        public static bool IsInput(string targetType)
        {
            return Input.Contains(targetType);
        }
    }
}