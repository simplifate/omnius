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
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.FrontEnd.Utils;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;

namespace FSS.Omnius.Controllers.Tapestry
{
    [PersonaAuthorize]
    public class RunController : Controller
    {
        public static DateTime requestStart;
        public static DateTime startTime;
        public static DateTime prepareEnd;

        [HttpGet]
        public ActionResult Index(string appName, string locale = "", string blockIdentify = null, int modelId = -1, string message = null, string messageType = null, string registry = null)
        {
            Dictionary<string, object> blockDependencies = new Dictionary<string, object>();

            RunController.startTime = DateTime.Now;

            var context = DBEntities.instance;
            // init
            C.CORE core = HttpContext.GetCORE();
            if (!string.IsNullOrEmpty(appName))
                core.Entitron.AppName = appName;
            core.User = User.GetLogged(core);

            // WatchtowerLogger.Instance.LogEvent($"Začátek WF: GET {appName}/{blockIdentify}. ModelId={modelId}.",
            //    core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Entitron.AppId);

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
                    if (user.HasRole(role))
                        userIsAllowed = true;
                }
                if (!userIsAllowed)
                    return new HttpStatusCodeResult(403);
            }

            try
            {
                block = block ?? context.WorkFlows.FirstOrDefault(w => w.ApplicationId == core.Entitron.AppId && w.InitBlockId != null && !w.IsTemp).InitBlock;
            }
            catch (NullReferenceException)
            {
                return new HttpStatusCodeResult(404);
            }

            var crossBlockRegistry = new Dictionary<string, object>();
            var fc = new FormCollection();
            if (!string.IsNullOrEmpty(registry))
                crossBlockRegistry = JsonConvert.DeserializeObject<Dictionary<string, object>>(registry);
            foreach (var pair in crossBlockRegistry)
            {
                fc.Add(pair.Key, pair.Value.ToString());
            }

            blockDependencies.Add("Translator", this.GetTranslator());
            var result = core.Tapestry.innerRun(HttpContext.GetLoggedUser(), block, "INIT", modelId, fc, -1, blockDependencies);
            if (result.Item2.Id != block.Id)
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = this.PrepareAlert(result.Item1.Message), messageType = result.Item1.Message.Type.ToString() });

            Dictionary<string, object> tapestryVars = result.Item1.OutputData;

            foreach (var pair in crossBlockRegistry)
            {
                if (!tapestryVars.ContainsKey(pair.Key))
                    tapestryVars.Add(pair.Key, pair.Value.ToString());
                else
                    tapestryVars[pair.Key] = pair.Value.ToString();
            }

            if (tapestryVars.ContainsKey("__ModelId__") && Convert.ToInt32(tapestryVars["__ModelId__"]) != modelId)
                modelId = Convert.ToInt32(tapestryVars["__ModelId__"]);

            // fill data
            ViewData["appName"] = core.Entitron.Application.DisplayName;
            ViewData["appIcon"] = core.Entitron.Application.Icon;
            ViewData["pageName"] = block.DisplayName;
            ViewData["UserEmail"] = core.User.Email;
            ViewData["blockName"] = block.Name;
            ViewData["crossBlockRegistry"] = registry;
            ViewData["userRoleArray"] = JsonConvert.SerializeObject(core.User.GetAppRoles(core.Entitron.AppId));
            ViewData["HttpContext"] = HttpContext;
            string lastLocale = this.GetLocaleNameById(core.User.LocaleId);
            if (locale == "")
                locale = lastLocale;
            ViewData["locale"] = locale;
            if (locale == "cs")
                core.User.LocaleId = 1;
            else
                core.User.LocaleId = 2;

            context.SaveChanges();
            DBItem modelRow = null;
            bool modelLoaded = false;
            if (modelId != -1 && !string.IsNullOrEmpty(block.ModelName))
            {
                modelRow = core.Entitron.GetDynamicTable(block.ModelName).GetById(modelId);
                modelLoaded = true;
            }
            var columnMetadataResultCache = new Dictionary<string, List<ColumnMetadata>>();
            var tableQueryResultCache = new Dictionary<string, List<DBItem>>();
            var viewQueryResultCache = new Dictionary<string, List<DBItem>>();
            foreach (var resourceMappingPair in block.ResourceMappingPairs.Where(r=> r.SourceTableName != null).ToList())
            {
                DataTable dataSource = null;
                List<string> columnNameList = null;
                Dictionary<string, string> columnDisplayNameDictionary = null;
                List<DBItem> entitronRowList = null;


                string sourceTableName = resourceMappingPair.SourceTableName;
                bool sourceIsColumnAttribute = false;
                bool noConditions = true;
                bool idAvailable = true;
                List<TapestryDesignerConditionSet> conditionSets = context.TapestryDesignerConditionSets.Where(cs => cs.ResourceMappingPair_Id == resourceMappingPair.Id).ToList();

                if (resourceMappingPair.relationType.StartsWith("V:"))
                {
                    dataSource = new DataTable();
                    columnDisplayNameDictionary = new Dictionary<string, string>();
                    var entitronView = core.Entitron.GetDynamicView(sourceTableName);
                    dataSource.Columns.Add("hiddenId", typeof(int));
                    if (viewQueryResultCache.ContainsKey(sourceTableName))
                        entitronRowList = viewQueryResultCache[sourceTableName];
                    else {
                        entitronRowList = entitronView.Select().ToList();
                        viewQueryResultCache.Add(sourceTableName, entitronRowList);
                    }
                    noConditions = conditionSets.Count == 0;
                    if (entitronRowList.Count > 0)
                    {
                        columnNameList = entitronRowList[0].getColumnNames();
                        idAvailable = columnNameList.Contains("id");
                    }
                    else {
                        continue;
                    }
                    foreach (var columnName in columnNameList)
                    {
                        dataSource.Columns.Add(columnName);
                        columnDisplayNameDictionary.Add(columnName, columnName);
                    }
                    foreach (var entitronRow in entitronRowList)
                    {
                        if (noConditions || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars))
                        {
                            var newRow = dataSource.NewRow();
                            newRow["hiddenId"] = idAvailable ? entitronRow["id"] : 0;
                            foreach (var columnName in columnNameList)
                            {
                                var currentCell = entitronRow[columnName];
                                if (currentCell is bool)
                                {
                                    if ((bool)currentCell == true)
                                        newRow[columnName] = "Ano";
                                    else
                                        newRow[columnName] = "Ne";
                                }
                                else if (currentCell is DateTime)
                                {
                                    newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
                                }
                                else
                                    newRow[columnName] = currentCell;
                            }
                            dataSource.Rows.Add(newRow);
                        }
                    }
                }
                else
                {
                    if (sourceTableName != null && sourceTableName.StartsWith("Omnius::"))
                    {
                        dataSource = new DataTable();
                        columnDisplayNameDictionary = new Dictionary<string, string>();
                        var systemTable = core.Entitron.GetSystemTable(sourceTableName);
                        columnNameList = systemTable.Item1;
                        var rowList = systemTable.Item2;
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        noConditions = conditionSets.Count == 0;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
                        {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            if (columnFilter.Count > 0)
                                getAllColumns = false;
                        }
                        dataSource.Columns.Add("hiddenId", typeof(int));
                        foreach (string columnName in columnNameList)
                        {
                            dataSource.Columns.Add(columnName);
                            columnDisplayNameDictionary.Add(columnName, columnName);
                        }
                        idAvailable = false;
                        string idProperty = "";
                        if (columnNameList.Contains("id"))
                        {
                            idProperty = "id";
                            idAvailable = true;
                        }
                        else if (columnNameList.Contains("Id"))
                        {
                            idProperty = "Id";
                            idAvailable = true;
                        }
                        foreach (var entitronRow in rowList)
                        {
                            if (noConditions || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars))
                            {
                                var newRow = dataSource.NewRow();
                                newRow["hiddenId"] = idAvailable ? entitronRow[idProperty] : 0;
                                foreach (string columnName in columnNameList)
                                {
                                    if (getAllColumns || columnFilter.Contains(columnName))
                                    {
                                        var currentCell = entitronRow[columnName];
                                        if (currentCell is bool)
                                        {
                                            if ((bool)currentCell == true)
                                                newRow[columnName] = "Ano";
                                            else
                                                newRow[columnName] = "Ne";
                                        }
                                        else if (currentCell is DateTime)
                                        {
                                            newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
                                        }
                                        else
                                            newRow[columnName] = currentCell;
                                    }
                                }
                                dataSource.Rows.Add(newRow);
                            }
                        }
                    }
                    if (resourceMappingPair.DataSourceParams == "currentUser"
                     && ResourceMappingPairsTarget.IsInput(resourceMappingPair.TargetType))
                    {
                        if (sourceTableName == "Omnius::Users")
                        {
                            switch (resourceMappingPair.SourceColumnName)
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
                        }
                        else if (sourceTableName == "Users")
                        {
                            var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                        .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                            if (epkUserRowList.Count > 0)
                                ViewData["inputData_" + resourceMappingPair.TargetName] = epkUserRowList[0][resourceMappingPair.SourceColumnName];
                        }
                    }
                    else if ( resourceMappingPair.DataSourceParams == "superior"
                        && ResourceMappingPairsTarget.IsInput(resourceMappingPair.TargetType))
                    {
                        var tableUsers = core.Entitron.GetDynamicTable("Users");
                        if (resourceMappingPair.SourceTableName == "Users")
                        {
                            var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                            if (epkUserRowList.Count > 0)
                            {
                                int superiorId = (int)epkUserRowList[0]["h_pernr"];
                                var epkSuperiorRowList = tableUsers.Select()
                                        .where(c => c.column("pernr").Equal(superiorId)).ToList();
                                if (epkSuperiorRowList.Count > 0)
                                    ViewData["inputData_" + resourceMappingPair.TargetName] = epkSuperiorRowList[0][resourceMappingPair.SourceColumnName];
                            }
                        }
                    }

                    else {
                        dataSource = new DataTable();
                        columnDisplayNameDictionary = new Dictionary<string, string>();
                        var entitronTable = core.Entitron.GetDynamicTable(sourceTableName);
                        List<string> columnFilter = null;
                        bool getAllColumns = true;
                        if (!string.IsNullOrEmpty(resourceMappingPair.SourceColumnFilter))
                        {
                            columnFilter = resourceMappingPair.SourceColumnFilter.Split(',').ToList();
                            if (columnFilter.Count > 0)
                                getAllColumns = false;
                        }
                        noConditions = conditionSets.Count == 0;
                        if (tableQueryResultCache.ContainsKey(sourceTableName))
                            entitronRowList = tableQueryResultCache[sourceTableName];
                        else {
                            entitronRowList = entitronTable.Select().ToList();
                            tableQueryResultCache.Add(sourceTableName, entitronRowList);
                        }
                        idAvailable = false;
                        string idProperty = "";
                        if (entitronRowList.Count > 0)
                        {
                            columnNameList = entitronRowList[0].getColumnNames();
                            if (columnNameList.Contains("id"))
                            {
                                idProperty = "id";
                                idAvailable = true;
                            }
                            else if (columnNameList.Contains("Id"))
                            {
                                idProperty = "Id";
                                idAvailable = true;
                            }
                            dataSource.Columns.Add("hiddenId", typeof(int));
                            List<ColumnMetadata> columnMetadataList = null;
                            if (columnMetadataResultCache.ContainsKey(sourceTableName))
                                columnMetadataList = columnMetadataResultCache[sourceTableName];
                            else {
                                columnMetadataList = core.Entitron.Application.ColumnMetadata.Where(c => c.TableName == sourceTableName).ToList();
                                columnMetadataResultCache.Add(sourceTableName, columnMetadataList);
                            }
                            foreach (string columnName in columnNameList)
                            {
                                if (getAllColumns || columnFilter.Contains(columnName))
                                {
                                    var columnMetadata = columnMetadataList.FirstOrDefault(c => c.ColumnName == columnName);
                                    if (columnMetadata != null && !string.IsNullOrEmpty(columnMetadata.ColumnDisplayName))
                                    {
                                        var newColumn = new DataColumn(columnName);
                                        newColumn.Caption = columnMetadata.ColumnDisplayName;
                                        dataSource.Columns.Add(newColumn);
                                        columnDisplayNameDictionary.Add(columnName, columnMetadata.ColumnDisplayName);
                                    }
                                    else {
                                        dataSource.Columns.Add(columnName);
                                        columnDisplayNameDictionary.Add(columnName, columnName);
                                    }
                                }
                            }
                            foreach (var entitronRow in entitronRowList)
                            {
                                if (noConditions || core.Entitron.filteringService.MatchConditionSets(conditionSets, entitronRow, tapestryVars))
                                {
                                    var newRow = dataSource.NewRow();
                                    newRow["hiddenId"] = idAvailable ? entitronRow[idProperty] : 0;
                                    foreach (string columnName in columnNameList)
                                    {
                                        if (getAllColumns || columnFilter.Contains(columnName))
                                        {
                                            var currentCell = entitronRow[columnName];
                                            if (currentCell is bool)
                                            {
                                                if ((bool)currentCell == true)
                                                    newRow[columnName] = "Ano";
                                                else
                                                    newRow[columnName] = "Ne";
                                            }
                                            else if (currentCell is DateTime)
                                            {
                                                newRow[columnName] = ((DateTime)currentCell).ToString("d. M. yyyy H:mm:ss");
                                            }
                                            else
                                                newRow[columnName] = currentCell;
                                        }
                                    }
                                    dataSource.Rows.Add(newRow);
                                }
                            }
                        }

                    }
                }






                    switch (resourceMappingPair.TargetType)
                    {
                        case "data-table-read-only":
                        case "data-table-with-actions":
                        case "name-value-list":
                            ViewData["tableData_" + resourceMappingPair.TargetName] = dataSource;
                            break;
                        case "dropdown-select":
                            if (sourceIsColumnAttribute && modelLoaded)
                            {
                                ViewData["dropdownSelection_" + resourceMappingPair.TargetName] = modelRow[resourceMappingPair.SourceColumnName];
                            }
                            else {
                                var dropdownDictionary = new Dictionary<int, string>();
                                if (columnDisplayNameDictionary.ContainsKey("name"))
                                {
                                    foreach (DataRow datarow in dataSource.Rows)
                                    {
                                        int id = (int)datarow["hiddenId"];
                                        if (!dropdownDictionary.ContainsKey(id))
                                            dropdownDictionary.Add(id, (string)datarow["name"]);
                                    }
                                }
                                else if (columnDisplayNameDictionary.ContainsKey("code"))
                                {
                                    foreach (DataRow datarow in dataSource.Rows)
                                    {
                                        int id = (int)datarow["hiddenId"];
                                        if (!dropdownDictionary.ContainsKey(id))
                                            dropdownDictionary.Add(id, (string)datarow["code"]);
                                    }
                                }
                                ViewData["dropdownData_" + resourceMappingPair.TargetName] = dropdownDictionary;
                            }
                            break;
                        case "checkbox":
                            if (modelLoaded)
                                ViewData["checkboxData_" + resourceMappingPair.TargetName] = modelRow[resourceMappingPair.SourceColumnName];
                            break;
                        case "input-single-line":
                        case "input-multiline":
                        case "color-picker":
                        case "label":
                            if (modelLoaded)
                            {
                                if (resourceMappingPair.DataSourceParams == "currentUser")
                                {
                                    if (sourceTableName == "Omnius::Users")
                                    {
                                        switch (resourceMappingPair.SourceColumnName)
                                        {
                                            case "DisplayName":
                                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.DisplayName;
                                                break;
                                            case "Company":
                                                ViewData["inputData_" + resourceMappingPair.TargetName] = core.User.Company;
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
                                    }
                                    else if (sourceTableName == "Users")
                                    {
                                        var epkUserRowList = core.Entitron.GetDynamicTable("Users").Select()
                                                    .where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                                        if (epkUserRowList.Count > 0)
                                            ViewData["inputData_" + resourceMappingPair.TargetName] = epkUserRowList[0][resourceMappingPair.SourceColumnName];
                                    }
                                }
                                else if (resourceMappingPair.DataSourceParams == "superior")
                                {
                                    var tableUsers = core.Entitron.GetDynamicTable("Users");
                                    if (sourceTableName == "Users")
                                    {
                                        var epkUserRowList = tableUsers.Select().where(c => c.column("ad_email").Equal(core.User.Email)).ToList();
                                        if (epkUserRowList.Count > 0)
                                        {
                                            int superiorId = (int)epkUserRowList[0]["h_pernr"];
                                            var epkSuperiorRowList = tableUsers.Select()
                                                    .where(c => c.column("pernr").Equal(superiorId)).ToList();
                                            if (epkSuperiorRowList.Count > 0)
                                                ViewData["inputData_" + resourceMappingPair.TargetName] = epkSuperiorRowList[0][resourceMappingPair.SourceColumnName];
                                        }
                                    }
                                }
                                else if (modelRow[resourceMappingPair.SourceColumnName] is DateTime)
                                {
                             
                                        ViewData["inputData_" + resourceMappingPair.TargetName] = modelRow[resourceMappingPair.SourceColumnName];
                                }
                                else
                                    ViewData["inputData_" + resourceMappingPair.TargetName] = modelRow[resourceMappingPair.SourceColumnName];
                            }
                            break;
                        case "countdown":
                            if (modelLoaded)
                                if (modelRow[resourceMappingPair.SourceColumnName] == DBNull.Value)
                                    ViewData["countdownTargetData_" + resourceMappingPair.TargetName] = null;
                                else {
                                    ViewData["countdownTargetData_" + resourceMappingPair.TargetName] = TimeZoneInfo.ConvertTimeToUtc(
                                            DateTime.SpecifyKind((DateTime)modelRow[resourceMappingPair.SourceColumnName], DateTimeKind.Unspecified),
                                            TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time")
                                        ).ToString("yyyy'-'MM'-'dd HH':'mm':'ss'Z'");
                                }
                            break;
                    }
                }
                foreach (var pair in tapestryVars)
                {
                    if (pair.Key.StartsWith("_uic_"))
                    {
                        if (pair.Value is DateTime && pair.Key.Contains("#"))
                        {
                            string key = pair.Key;
                            string keyWithoutSuffix = key.Substring(0, key.LastIndexOf('#'));

                            if (key.EndsWith("#date"))
                                ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("d.M.yyyy");
                            else if (key.EndsWith("#time"))
                                ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("H:mm:ss");
                            else
                                ViewData[keyWithoutSuffix.Substring(5)] = ((DateTime)pair.Value).ToString("d.M.yyyy H:mm:ss");
                        }
                        else
                            ViewData[pair.Key.Substring(5)] = pair.Value;
                    }
                    else if (pair.Key.StartsWith("_uictable_"))
                    {
                        List<DBItem> entitronRowList;
                        if (pair.Value is DBItem)
                        {
                            entitronRowList = new List<DBItem>();
                            entitronRowList.Add((DBItem)pair.Value);
                        }
                        else
                        {
                            entitronRowList = (List<DBItem>)pair.Value;
                        }
                        bool columnsCreated = false;
                        DataTable dataSource = new DataTable();
                        dataSource.Columns.Add("hiddenId", typeof(int));
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
                        ViewData[pair.Key.Substring(10)] = dataSource;
                    }
                }
                string viewPath = $"{core.Entitron.Application.Id}\\Page\\{block.EditorPageId}.cshtml";

                prepareEnd = DateTime.Now;
                // show
                //return View(block.MozaicPage.ViewPath);

                // WatchtowerLogger.Instance.LogEvent($"Konec WF: GET {appName}/{blockIdentify}. ModelId={modelId}.",
                //     core.User == null ? 0 : core.User.Id, LogEventType.NotSpecified, LogLevel.Info, false, core.Entitron.AppId);

                return View(viewPath);
            
        }

        public string PrepareAlert(C.Message message)
        {
            // Never write same code multiple times!
            List<string> messageCollection;

            // Select most important type - limitation of transfer style
            if (message.Errors.Count > 0)
            {
                messageCollection = message.Errors;
            }
            else if (message.Success.Count > 0)
            {
                messageCollection = message.Success;
            }
            else if (message.Warnings.Count > 0)
            {
                messageCollection = message.Warnings;
            }
            else
            {
                messageCollection = message.Info;
            }

            // Translate messages
            ITranslator t = this.GetTranslator();
            messageCollection = messageCollection.Select(x => t._(x)).ToList();

            // Merge them together
            return string.Join("<br/>", messageCollection);
        }

        public ITranslator GetTranslator()
        {
            return new T(this.GetLocaleNameById(HttpContext.GetCORE().User.LocaleId));
        }
        
        // [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(string appName, string button, FormCollection fc, string blockIdentify = null, int modelId = -1, int deleteId = -1)
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
            var result = core.Tapestry.run(HttpContext.GetLoggedUser(), block, button, modelId, fc, deleteId);
            Session["CrossBlockRegistry"] = core.CrossBlockRegistry;

            if (Response.StatusCode != 202) {
                return RedirectToRoute("Run", new { appName = appName, blockIdentify = result.Item2.Name, modelId = modelId, message = result.Item1.ToUser(), messageType = result.Item1.Type.ToString(), registry = JsonConvert.SerializeObject(result.Item3) });
            }
            return null;
        }

        public string GetLocaleNameById(int? id)
        {
            if (id.HasValue && id == 1)
                return "cs";
            else return "en";

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