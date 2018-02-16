//using FSS.Omnius.Modules.Entitron.Entity.Persona;
//using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
//using FSS.Omnius.Modules.Tapestry.Service;
//using System;
//using System.Web.Mvc;
//using System.Linq;
//using System.Text.RegularExpressions;
//using FSS.Omnius.Modules.Entitron;
//using FSS.Omnius.Modules.Entitron.Entity;
//using C = FSS.Omnius.Modules.CORE;
//using System.Collections.Generic;
//using System.Data;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using FSS.Omnius.Modules.Entitron.Entity.Entitron;
//using FSS.Omnius.Modules.Watchtower;
//using System.Data.Entity;
//using FSPOC_WebProject;
//using System.Web;
//using Microsoft.AspNet.Identity.Owin;
//using System.Configuration;
//using FSS.Omnius.Utils;
//using FSS.Omnius.Modules;

//namespace FSS.Omnius.Controllers.Tapestry
//{
//    [PersonaAuthorize]
//    public class RunController : Controller
//    {
//        public static DateTime requestStart;
//        public static DateTime startTime;
//        public static DateTime prepareEnd;

//        [HttpGet]
//        public ActionResult Index(string appName, string blockIdentify = null, int modelId = -1, string locale = "", string message = null, string messageType = null, string registry = null)
//        {
//            Dictionary<string, object> blockDependencies = new Dictionary<string, object>();

//            RunController.startTime = DateTime.Now;

//            var context = DBEntities.instance;
//            // init
//            C.CORE core = HttpContext.GetCORE();
//            if (!string.IsNullOrEmpty(appName))
//                core.Application.Name = appName;
//            core.User = User.GetLogged(core);

//            // WatchtowerLogger.Instance.LogEvent($"Začátek WF: GET {appName}/{blockIdentify}. ModelId={modelId}.",
//            //    core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            Block block = getBlockWithResource(core, context, appName, blockIdentify);
//            if (block == null)
//                return new HttpStatusCodeResult(404);

//            // Check if user has one of allowed roles, otherwise return 403
//            if (!String.IsNullOrEmpty(block.RoleWhitelist))
//            {
//                User user = core.User;
//                bool userIsAllowed = false;
//                foreach (var role in block.RoleWhitelist.Split(',').ToList())
//                {
//                    if (user.HasRole(role))
//                        userIsAllowed = true;
//                }
//                if (!userIsAllowed)
//                    return new HttpStatusCodeResult(403);
//            }

//            try
//            {
//                block = block ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Application.Id && w.InitBlockId != null && !w.IsTemp).InitBlock;
//            }
//            catch (NullReferenceException)
//            {
//                return new HttpStatusCodeResult(404);
//            }

//            var crossBlockRegistry = new Dictionary<string, object>();
//            var fc = new FormCollection();
//            if (!string.IsNullOrEmpty(registry))
//                crossBlockRegistry = JsonConvert.DeserializeObject<Dictionary<string, object>>(registry);
//            foreach (var pair in crossBlockRegistry)
//            {
//                fc.Add(pair.Key, pair.Value.ToString());
//            }

//            blockDependencies.Add("Translator", this.GetTranslator());
//            var result = core.Tapestry.innerRun(HttpContext.GetLoggedUser(), block, "INIT", modelId, fc, -1, blockDependencies);
//            if (result.Item2.Id != block.Id)
//                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = this.PrepareAlert(result.Item1.Message), messageType = result.Item1.Message.Type.ToString() });

//            Dictionary<string, object> tapestryVars = result.Item1.OutputData;

//            foreach (var pair in crossBlockRegistry)
//            {
//                if (!tapestryVars.ContainsKey(pair.Key))
//                    tapestryVars.Add(pair.Key, pair.Value.ToString());
//            }

//            if (tapestryVars.ContainsKey("__ModelId__") && Convert.ToInt32(tapestryVars["__ModelId__"]) != modelId)
//                modelId = Convert.ToInt32(tapestryVars["__ModelId__"]);

//            // fill data
//            ViewData["appName"] = core.Application.DisplayName;
//            ViewData["appIcon"] = core.Application.Icon;
//            ViewData["pageName"] = block.DisplayName;
//            ViewData["UserEmail"] = core.User.Email;
//            ViewData["blockName"] = block.Name;
//            ViewData["crossBlockRegistry"] = registry;
//            ViewData["userRoleArray"] = JsonConvert.SerializeObject(core.User.GetAppRoles(core.Application.Id));
//            ViewData["HttpContext"] = HttpContext;
//            string lastLocale = this.GetLocaleNameById(core.User.LocaleId);
//            if (locale == "")
//                locale = lastLocale;
//            ViewData["locale"] = locale;
//            if (locale == "cs")
//                core.User.LocaleId = 1;
//            else
//                core.User.LocaleId = 2;

//            context.SaveChanges();
//            DBItem modelRow = null;
//            bool modelLoaded = false;
//            if (modelId != -1 && !string.IsNullOrEmpty(block.ModelName))
//            {
//                modelRow = core.Entitron.GetDynamicTable(block.ModelName).GetById(modelId);
//                modelLoaded = true;
//            }
//            var columnMetadataResultCache = new Dictionary<string, List<ColumnMetadata>>();
//            var tableQueryResultCache = new Dictionary<string, List<DBItem>>();
//            var viewQueryResultCache = new Dictionary<string, List<DBItem>>();
//            foreach (var resourceMappingPair in block.ResourceMappingPairs)
//            {
//                DataTable dataSource = null;
//                List<string> columnNameList = null;
//                Dictionary<string, string> columnDisplayNameDictionary = null;
//                List<DBItem> entitronRowList = null;
//                var source = resourceMappingPair.Source;
//                var target = resourceMappingPair.Target;
//                string sourceTableName = source.TableName;
//                bool sourceIsColumnAttribute = false;
//                bool noConditions = true;
//                bool idAvailable = true;
//                switch (source.TypeClass)
//                {
//                    case "viewAttribute":
//                        dataSource = new DataTable();
//                        columnDisplayNameDictionary = new Dictionary<string, string>();
//                        var entitronView = core.Entitron.GetDynamicView(sourceTableName);
//                        dataSource.Columns.Add("hiddenId", typeof(int));
//                        if (viewQueryResultCache.ContainsKey(sourceTableName))
//                            entitronRowList = viewQueryResultCache[sourceTableName];
//                        else {
//                            entitronRowList = entitronView.Select().ToList();
//                            viewQueryResultCache.Add(sourceTableName, entitronRowList);
//                        }
//                        noConditions = source.ConditionSets.Count == 0;
//                        if (entitronRowList.Count > 0)
//                        {
//                            columnNameList = entitronRowList[0].getColumnNames();
//                            idAvailable = columnNameList.Contains("id");
//                        }
//                        else {
//                            continue;
//                        }
//                        foreach (var columnName in columnNameList)
//                        {
//                            dataSource.Columns.Add(columnName);
//                            columnDisplayNameDictionary.Add(columnName, columnName);
//                        }
//                        foreach (var entitronRow in entitronRowList)
//                        {
//                            if (noConditions || core.Entitron.filteringService.MatchConditionSets(source.ConditionSets, entitronRow, tapestryVars))
//                            {
//                                var newRow = dataSource.NewRow();
//                                newRow["hiddenId"] = idAvailable ? entitronRow["id"] : 0;
//                                foreach (var columnName in columnNameList)
//                                {
//                                    var currentCell = entitronRow[columnName];
//                                    if (currentCell is bool)
//                                    {
//                                        if ((bool)currentCell == true)
//                                            newRow[columnName] = "Ano";
//                                        else
//                                            newRow[columnName] = "Ne";
//                                    }
//                                    else if (currentCell is DateTime)
//                                    {
//                                        newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
//                                    }
//                                    else
//                                        newRow[columnName] = currentCell;
//                                }
//                                dataSource.Rows.Add(newRow);
//                            }
//                        }
//                        break;
//                    case "tableAttribute":
//                        if (sourceTableName.StartsWith("Omnius::"))
//                        {
//                            dataSource = new DataTable();
//                            columnDisplayNameDictionary = new Dictionary<string, string>();
//                            var systemTable = core.Entitron.GetSystemTable(sourceTableName);
//                            columnNameList = systemTable.Item1;
//                            var rowList = systemTable.Item2;
//                            List<string> columnFilter = null;
//                            bool getAllColumns = true;
//                            noConditions = source.ConditionSets.Count == 0;
//                            if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
//                            {
//                                columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
//                                if (columnFilter.Count > 0)
//                                    getAllColumns = false;
//                            }
//                            dataSource.Columns.Add("hiddenId", typeof(int));
//                            foreach (string columnName in columnNameList)
//                            {
//                                dataSource.Columns.Add(columnName);
//                                columnDisplayNameDictionary.Add(columnName, columnName);
//                            }
//                            idAvailable = false;
//                            string idProperty = "";
//                            if (columnNameList.Contains("id"))
//                            {
//                                idProperty = "id";
//                                idAvailable = true;
//                            }
//                            else if (columnNameList.Contains("Id"))
//                            {
//                                idProperty = "Id";
//                                idAvailable = true;
//                            }
//                            foreach (var entitronRow in rowList)
//                            {
//                                if (noConditions || core.Entitron.filteringService.MatchConditionSets(source.ConditionSets, entitronRow, tapestryVars))
//                                {
//                                    var newRow = dataSource.NewRow();
//                                    newRow["hiddenId"] = idAvailable ? entitronRow[idProperty] : 0;
//                                    foreach (string columnName in columnNameList)
//                                    {
//                                        if (getAllColumns || columnFilter.Contains(columnName))
//                                        {
//                                            var currentCell = entitronRow[columnName];
//                                            if (currentCell is bool)
//                                            {
//                                                if ((bool)currentCell == true)
//                                                    newRow[columnName] = "Ano";
//                                                else
//                                                    newRow[columnName] = "Ne";
//                                            }
//                                            else if (currentCell is DateTime)
//                                            {
//                                                newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
//                                            }
//                                            else
//                                                newRow[columnName] = currentCell;
//                                        }
//                                    }
//                                    dataSource.Rows.Add(newRow);
//                                }
//                            }
//                        }
//                        else {
//                            dataSource = new DataTable();
//                            columnDisplayNameDictionary = new Dictionary<string, string>();
//                            var entitronTable = core.Entitron.GetDynamicTable(sourceTableName);
//                            List<string> columnFilter = null;
//                            bool getAllColumns = true;
//                            if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
//                            {
//                                columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
//                                if (columnFilter.Count > 0)
//                                    getAllColumns = false;
//                            }
//                            noConditions = source.ConditionSets.Count == 0;
//                            if (tableQueryResultCache.ContainsKey(sourceTableName))
//                                entitronRowList = tableQueryResultCache[sourceTableName];
//                            else {
//                                entitronRowList = entitronTable.Select().ToList();
//                                tableQueryResultCache.Add(sourceTableName, entitronRowList);
//                            }
//                            idAvailable = false;
//                            string idProperty = "";
//                            if (entitronRowList.Count > 0)
//                            {
//                                columnNameList = entitronRowList[0].getColumnNames();
//                                if (columnNameList.Contains("id"))
//                                {
//                                    idProperty = "id";
//                                    idAvailable = true;
//                                }
//                                else if (columnNameList.Contains("Id"))
//                                {
//                                    idProperty = "Id";
//                                    idAvailable = true;
//                                }
//                                dataSource.Columns.Add("hiddenId", typeof(int));
//                                List<ColumnMetadata> columnMetadataList = null;
//                                if (columnMetadataResultCache.ContainsKey(sourceTableName))
//                                    columnMetadataList = columnMetadataResultCache[sourceTableName];
//                                else {
//                                    columnMetadataList = core.Application.ColumnMetadata.Where(c => c.TableName == sourceTableName).ToList();
//                                    columnMetadataResultCache.Add(sourceTableName, columnMetadataList);
//                                }
//                                foreach (string columnName in columnNameList)
//                                {
//                                    if (getAllColumns || columnFilter.Contains(columnName))
//                                    {
//                                        var columnMetadata = columnMetadataList.FirstOrDefault(c => c.ColumnName == columnName);
//                                        if (columnMetadata != null && !string.IsNullOrEmpty(columnMetadata.ColumnDisplayName))
//                                        {
//                                            var newColumn = new DataColumn(columnName);
//                                            newColumn.Caption = columnMetadata.ColumnDisplayName;
//                                            dataSource.Columns.Add(newColumn);
//                                            columnDisplayNameDictionary.Add(columnName, columnMetadata.ColumnDisplayName);
//                                        }
//                                        else {
//                                            dataSource.Columns.Add(columnName);
//                                            columnDisplayNameDictionary.Add(columnName, columnName);
//                                        }
//                                    }
//                                }
//                                foreach (var entitronRow in entitronRowList)
//                                {
//                                    if (noConditions || core.Entitron.filteringService.MatchConditionSets(source.ConditionSets, entitronRow, tapestryVars))
//                                    {
//                                        var newRow = dataSource.NewRow();
//                                        newRow["hiddenId"] = idAvailable ? entitronRow[idProperty] : 0;
//                                        foreach (string columnName in columnNameList)
//                                        {
//                                            if (getAllColumns || columnFilter.Contains(columnName))
//                                            {
//                                                var currentCell = entitronRow[columnName];
//                                                if (currentCell is bool)
//                                                {
//                                                    if ((bool)currentCell == true)
//                                                        newRow[columnName] = "Ano";
//                                                    else
//                                                        newRow[columnName] = "Ne";
//                                                }
//                                                else if (currentCell is DateTime)
//                                                {
//                                                    newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
//                                                }
//                                                else
//                                                    newRow[columnName] = currentCell;
//                                            }
//                                        }
//                                        dataSource.Rows.Add(newRow);
//                                    }
//                                }
//                            }
//                        }
//                        break;
//                    case "columnAttribute":
//                        sourceIsColumnAttribute = true;
//                        break;
//                    case "uiItem":
//                        break;
//                }
//                switch (target.TypeClass)
//                {
//                    case "uiItem":
//                        switch (resourceMappingPair.TargetType)
//                        {
//                            case "data-table-read-only":
//                            case "data-table-with-actions":
//                            case "name-value-list":
//                                ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
//                                break;
//                            case "dropdown-select":
//                                if (sourceIsColumnAttribute && modelLoaded)
//                                {
//                                    ViewData["dropdownSelection_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
//                                }
//                                else {
//                                    var dropdownDictionary = new Dictionary<int, string>();
//                                    if (columnDisplayNameDictionary.ContainsKey("name"))
//                                    {
//                                        foreach (DataRow datarow in dataSource.Rows)
//                                        {
//                                            int id = (int)datarow["hiddenId"];
//                                            if (!dropdownDictionary.ContainsKey(id))
//                                                dropdownDictionary.Add(id, (string)datarow["name"]);
//                                        }
//                                    }
//                                    else if (columnDisplayNameDictionary.ContainsKey("code"))
//                                    {
//                                        foreach (DataRow datarow in dataSource.Rows)
//                                        {
//                                            int id = (int)datarow["hiddenId"];
//                                            if (!dropdownDictionary.ContainsKey(id))
//                                                dropdownDictionary.Add(id, (string)datarow["code"]);
//                                        }
//                                    }
//                                    ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
//                                }
//                                break;
//                            case "checkbox":
//                                if (modelLoaded)
//                                    ViewData["checkboxData_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
//                                break;
//                            case "input-single-line":
//                            case "input-multiline":
//                            case "color-picker":
//                            case "label":
//                                if (modelLoaded)
//                                {
//                                    if (resourceMappingPair.DataSourceParams == "currentUser")
//                                    {
//                                        if (sourceTableName == "Omnius::Users")
//                                        {
//                                            switch (source.ColumnName)
//                                            {
//                                                case "DisplayName":
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.DisplayName;
//                                                    break;
//                                                case "Company":
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Company;
//                                                    break;
//                                                case "Job":
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Job;
//                                                    break;
//                                                case "Email":
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Email;
//                                                    break;
//                                                case "Address":
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Address;
//                                                    break;
//                                            }
//                                        }
//                                        else if (sourceTableName == "Users")
//                                        {
//                                            var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
//                                                        .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
//                                            if (epkUserRowList.Count > 0)
//                                                ViewData["inputData_" + resourceMappingPair.TargetName] = epkUserRowList[0][source.ColumnName];
//                                        }
//                                    }
//                                    else if (resourceMappingPair.DataSourceParams == "superior")
//                                    {
//                                        var tableUsers = core.Entitron.GetDynamicTable("Users");
//                                        if (sourceTableName == "Users")
//                                        {
//                                            var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
//                                            if (epkUserRowList.Count > 0)
//                                            {
//                                                int superiorId = (int)epkUserRowList[0]["h_pernr"];
//                                                var epkSuperiorRowList = tableUsers.Select()
//                                                        .where(c => c.column("pernr").Equal(superiorId)).ToList();
//                                                if (epkSuperiorRowList.Count > 0)
//                                                    ViewData["inputData_" + resourceMappingPair.TargetName] = epkSuperiorRowList[0][source.ColumnName];
//                                            }
//                                        }
//                                    }
//                                    else if (modelRow[source.ColumnName] is DateTime)
//                                    {
//                                        if (!string.IsNullOrEmpty(resourceMappingPair.TargetClasses))
//                                        {
//                                            var sourceDateTime = (DateTime)modelRow[source.ColumnName];
//                                            var classList = resourceMappingPair.TargetClasses.Split(' ').ToList();
//                                            if (classList.Contains("input-with-datepicker"))
//                                                ViewData["inputData_" + resourceMappingPair.TargetName] = sourceDateTime.ToString("d.M.yyyy");
//                                            else if (classList.Contains("input-with-timepicker"))
//                                                ViewData["inputData_" + resourceMappingPair.TargetName] = sourceDateTime.ToString("H:mm:ss");
//                                            else
//                                                ViewData["inputData_" + resourceMappingPair.TargetName] = sourceDateTime.ToString("d.M.yyyy H:mm:ss");
//                                        }
//                                        else
//                                            ViewData["inputData_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
//                                    }
//                                    else
//                                        ViewData["inputData_" + resourceMappingPair.TargetName] = modelRow[source.ColumnName];
//                                }
//                                break;
//                            case "countdown":
//                                if (modelLoaded)
//                                    if (modelRow[source.ColumnName] == DBNull.Value)
//                                        ViewData["countdownTargetData_" + resourceMappingPair.TargetName] = null;
//                                    else {
//                                        ViewData["countdownTargetData_" + resourceMappingPair.TargetName] = TimeZoneInfo.ConvertTimeToUtc(
//                                                DateTime.SpecifyKind((DateTime)modelRow[source.ColumnName], DateTimeKind.Unspecified),
//                                                TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")
//                                            ).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'Z'");
//                                    }
//                                break;
//                        }
//                        break;
//                }
//            }
//            foreach (var pair in tapestryVars)
//            {
//                if (pair.Key.StartsWith("_uic_"))
//                {
//                    if (pair.Value is DateTime && pair.Key.Contains("#"))
//                    {
//                        string key = pair.Key;
//                        string keyWithoutSuffix = key.Substring(0, key.LastIndexOf('#'));

//                        if (key.EndsWith("#date"))
//                            ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("d.M.yyyy");
//                        else if (key.EndsWith("#time"))
//                            ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("H:mm:ss");
//                        else
//                            ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("d.M.yyyy H:mm:ss");
//                    }
//                    else
//                        ViewData[pair.Key.Substring(5)] = pair.Value;
//                }
//                else if (pair.Key.StartsWith("_uictable_"))
//                {
//                    List<DBItem> entitronRowList;
//                    if (pair.Value is DBItem)
//                    {
//                        entitronRowList = new List<DBItem>();
//                        entitronRowList.Add((DBItem)pair.Value);
//                    }
//                    else
//                    {
//                        entitronRowList = (List<DBItem>)pair.Value;
//                    }
//                    bool columnsCreated = false;
//                    DataTable dataSource = new DataTable();
//                    dataSource.Columns.Add("hiddenId", typeof(int));
//                    foreach (var entitronRow in entitronRowList)
//                    {
//                        var newRow = dataSource.NewRow();
//                        var columnNames = entitronRow.getColumnNames();
//                        newRow["hiddenId"] = columnNames.Contains("id") ? entitronRow["id"] : -1;
//                        foreach (var columnName in columnNames)
//                        {
//                            if (!columnsCreated)
//                                dataSource.Columns.Add(columnName);
//                            if (entitronRow[columnName] is bool)
//                            {
//                                if ((bool)entitronRow[columnName] == true)
//                                    newRow[columnName] = "Ano";
//                                else
//                                    newRow[columnName] = "Ne";
//                            }
//                            else if (entitronRow[columnName] is DateTime)
//                            {
//                                newRow[columnName] = ((DateTime)entitronRow[columnName]).ToString("d. M. yyyy H:mm:ss");
//                            }
//                            else
//                                newRow[columnName] = entitronRow[columnName];
//                        }
//                        columnsCreated = true;
//                        dataSource.Rows.Add(newRow);
//                    }
//                    ViewData[pair.Key.Substring(10)] = dataSource;
//                }
//            }
//            string viewPath = $"{core.Application.Id}\\Page\\{block.EditorPageId}.cshtml";

//            prepareEnd = DateTime.Now;
//            // show
//            //return View(block.MozaicPage.ViewPath);

//            // WatchtowerLogger.Instance.LogEvent($"Konec WF: GET {appName}/{blockIdentify}. ModelId={modelId}.",
//            //     core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            return View(viewPath);
//        }
//        [HttpPost]
//        public ActionResult Index(string appName, string button, FormCollection fc, string blockIdentify = null, int modelId = -1, int deleteId = -1)
//        {
//            C.CORE core = HttpContext.GetCORE();
//            core.Application = DBEntities.instance.Applications.SingleOrDefault(a => a.Name == appName && a.IsEnabled && a.IsPublished && !a.IsSystem);

//            // WatchtowerLogger.Instance.LogEvent($"Začátek WF: POST {appName}/{blockIdentify}. ModelId={modelId}, Button={button}.",
//            //     core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            DBEntities context = DBEntities.instance;
//            // get block
//            Block block = null;
//            try
//            {
//                int blockId = Convert.ToInt32(blockIdentify);
//                block = context.Blocks.FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.Id == blockId && !b.WorkFlow.IsTemp);
//            }
//            catch (FormatException)
//            {
//                block = context.Blocks.FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.Name == blockIdentify && !b.WorkFlow.IsTemp);
//            }

//            try
//            {
//                block = block ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Application.Id && w.InitBlockId != null && !w.IsTemp).InitBlock;
//            }
//            catch (NullReferenceException)
//            {
//                return new HttpStatusCodeResult(404);
//            }
//            var crossBlockRegistry = new Dictionary<string, object>();
//            if (!string.IsNullOrEmpty(fc["registry"]))
//                crossBlockRegistry = JsonConvert.DeserializeObject<Dictionary<string, object>>(fc["registry"]);
//            foreach (var pair in crossBlockRegistry)
//            {
//                fc.Add(pair.Key, pair.Value.ToString());
//            }
//            // run
//            var result = core.Tapestry.run(HttpContext.GetLoggedUser(), block, button, modelId, fc, deleteId);

//            // WatchtowerLogger.Instance.LogEvent($"Konec WF: POST {appName}/{blockIdentify}. ModelId={modelId}, Button={button}.",
//            //     core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            // redirect
//            if (Response.ContentType.IndexOf("application/") == -1)
//            {
//                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = this.PrepareAlert(result.Item1), messageType = result.Item1.Type.ToString(), registry = JsonConvert.SerializeObject(result.Item3) });
//            }
//            return null;
//        }
//        private Block getBlockWithResource(C.CORE core, DBEntities context, string appName, string blockName)
//        {
//            return blockName != null
//                ? context.Blocks
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
//                    .Include(b => b.SourceTo_ActionRules)
//                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.Name == blockName && !b.WorkFlow.IsTemp)
//                : context.Blocks
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
//                    .Include(b => b.SourceTo_ActionRules)
//                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.WorkFlow.InitBlockId == b.Id && !b.WorkFlow.IsTemp);
//        }
//        private Block getBlockWithWF(C.CORE core, DBEntities context, string appName, string blockName)
//        {
//            return blockName != null
//                ? context.Blocks
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
//                    .Include(b => b.SourceTo_ActionRules)
//                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.Name == blockName && !b.WorkFlow.IsTemp)
//                : context.Blocks
//                    .Include(b => b.ResourceMappingPairs)
//                    .Include(b => b.SourceTo_ActionRules)
//                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.WorkFlow.InitBlockId == b.Id && b.WorkFlow.IsTemp);
//        }

//        public static string GetRunTime()
//        {
//            double dbTime = 0;
//            int dbCount = 0;
//            Regex rx = new Regex("Completed in ([0-9]+) ms");

//            foreach (string m in DBEntities.messages)
//            {
//                if (rx.IsMatch(m))
//                {
//                    Match result = rx.Match(m);
//                    dbTime += float.Parse(result.Groups[1].ToString());
//                    dbCount++;
//                }
//            }

//            double totalRunTime = (DateTime.Now - requestStart).TotalMilliseconds;
//            double dataRunTime = (prepareEnd - startTime).TotalMilliseconds;
//            double viewRunTime = (DateTime.Now - prepareEnd).TotalMilliseconds;

//            string str = string.Format("<p>Total run time: {0}s (Data: {3}s, View: {4}s); SQL time: {1}s ({2}×)</p>",
//                                        Math.Round(totalRunTime / 1000, 3).ToString(),
//                                        Math.Round(dbTime / 1000, 3).ToString(),
//                                        dbCount,
//                                        Math.Round(dataRunTime / 1000, 3).ToString(),
//                                        Math.Round(viewRunTime / 1000, 3).ToString()
//                                      );
//            str += "<div id=\"SQLLog\">" + string.Join("<br>", DBEntities.messages) + "</div>";
//            return str;
//        }

//        public string PrepareAlert(C.Message message)
//        {
//            // Never write same code multiple times!
//            List<string> messageCollection;

//            // Select most important type - limitation of transfer style
//            if (message.Errors.Count > 0)
//            {
//                messageCollection = message.Errors;
//            }
//            else if (message.Success.Count > 0)
//            {
//                messageCollection = message.Success;
//            }
//            else if (message.Warnings.Count > 0)
//            {
//                messageCollection = message.Warnings;
//            }
//            else
//            {
//                messageCollection = message.Info;
//            }

//            // Translate messages
//            ITranslator t = this.GetTranslator();
//            messageCollection = messageCollection.Select(x => t._(x)).ToList();

//            // Merge them together
//            return string.Join("<br/>", messageCollection);
//        }

//        public string GetLocaleNameById(int? id)
//        {
//            if (id.HasValue && id == 1)
//                return "cs";
//            else return "en";

//        }

//        public ITranslator GetTranslator()
//        {
//            return new T(this.GetLocaleNameById(HttpContext.GetCORE().User.LocaleId));
//        }
//    }

//    // Used for Cortex AzureScheduler testing
//    // Insecure - bypasses Persona authentication for the System account
//    // TODO: Enable proper authentication once the issues with Persona login are resolved
//    [AllowAnonymous]
//    public class UnauthorizedRunController : Controller
//    {
//        private ApplicationUserManager _userManager;
//        private ApplicationSignInManager _signInManager;

//        public ApplicationUserManager UserManager
//        {
//            get
//            {
//                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            }
//            private set
//            {
//                _userManager = value;
//            }
//        }

//        public ApplicationSignInManager SignInManager
//        {
//            get
//            {
//                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
//            }
//            private set
//            {
//                _signInManager = value;
//            }
//        }

//        private string userName;

//        public ActionResult Get(string appName, string button, string blockIdentify = null, int modelId = -1)
//        {
//            Dictionary<string, object> blockDependencies = new Dictionary<string, object>();

//            bool isAuthorized = Authorize();
//            if (!isAuthorized)
//                return new Http403Result();

//            C.CORE core = HttpContext.GetCORE();

//            if (core.User == null)
//            {
//                core.User = core.Persona.AuthenticateUser(userName);
//            }

//            DBEntities context = DBEntities.instance;
//            core.Application = context.Applications.SingleOrDefault(a => a.Name == appName && a.IsEnabled && a.IsPublished && !a.IsSystem);

//            WatchtowerLogger.Instance.LogEvent($"Začátek WF: Cortex Get {appName}/{blockIdentify}. ModelId={modelId}, Button={button}.",
//                core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            // get block
//            Block block = getBlockWithWF(core, context, appName, blockIdentify);
//            if (block == null)
//                return new HttpStatusCodeResult(404);
//            var crossBlockRegistry = new Dictionary<string, object>();

//            blockDependencies.Add("Translator", this.GetTranslator());
//            // run
//            var result = core.Tapestry.run(core.User, block, button, modelId, new FormCollection(), -1, blockDependencies);

//            WatchtowerLogger.Instance.LogEvent($"Konec WF: Cortex Get {appName}/{blockIdentify}. ModelId={modelId}, Button={button}.",
//                core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Application.Id);

//            // redirect
//            return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = result.Item1.ToUser(), messageType = result.Item1.Type.ToString(), registry = JsonConvert.SerializeObject(result.Item3) });
//        }

//        private bool Authorize()
//        {
//            userName = HttpContext.Request.QueryString["User"];
//            string password = HttpContext.Request.QueryString["Password"];

//            var result = SignInManager.PasswordSignIn(userName, password, false, shouldLockout: false);
//            if (result == SignInStatus.Success)
//            {
//                return true;
//            }
//            return false;
//        }

//        private Block getBlockWithWF(C.CORE core, DBEntities context, string appName, string blockName)
//        {
//            return blockName != null
//                ? context.Blocks
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Source))
//                    .Include(b => b.ResourceMappingPairs.Select(mp => mp.Target))
//                    .Include(b => b.SourceTo_ActionRules)
//                    .FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.Name == blockName)
//                : context.Blocks.Include(b => b.ResourceMappingPairs).Include(b => b.SourceTo_ActionRules).FirstOrDefault(b => b.WorkFlow.ApplicationId == core.Application.Id && b.WorkFlow.InitBlockId == b.Id);
//        }

//        public string GetLocaleNameById(int? id)
//        {
//            if (id.HasValue && id == 1)
//                return "cs";
//            else return "en";

//        }

//        public ITranslator GetTranslator()
//        {
//            return new T(this.GetLocaleNameById(HttpContext.GetCORE().User.LocaleId));
//        }
//    }
//}