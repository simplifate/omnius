using CsvHelper;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Tapestry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Entitron : ActionManager
    {
        [Action(10002099, "Cast variable", "Result", "Error", "IsNull")]
        public static (object, string, bool) Cast(COREobject core, object Object, string CustomTargetType = null, string TargetType = null)
        {
            string error = "";
            
            string targetType = "";
            if (CustomTargetType != null)
            {
                targetType = CustomTargetType;
            }
            else if (TargetType != null)
            {
                targetType = TargetType;
            }

            if (Object == null || Object == DBNull.Value)
            {
                return (null, error, true);
            }

            if (string.IsNullOrEmpty(targetType))
            {
                throw new Exception($"Target type must by specified");
            }

            try
            {
                return (Converting(Object, targetType), error, false);
            }
            catch (Exception e)
            {
                return (null, e.Message, true);
            }
        }
        private static object Converting(object source, string targetType)
        {
            object output = null;
            if (source is DBItem || source is IEnumerable<DBItem>)
            {
                return source is DBItem ? ConvertDbItem((DBItem)source, targetType) : ConvertDbItemList((IEnumerable<DBItem>)source, targetType);
            }

            JToken sourceToken = source is JToken ? (JToken)source : JToken.FromObject(source);

            switch (targetType)
            {
                case "bool": output = As<bool>(sourceToken); break;
                case "string": output = As<string>(sourceToken); break;
                case "char": output = As<char>(sourceToken); break;
                case "int": output = As<int>(sourceToken); break;
                case "float": output = As<float>(sourceToken); break;
                case "decimal": output = As<decimal>(sourceToken); break;
                case "double": output = As<double>(sourceToken); break;
                case "JArray": output = (JArray)sourceToken; break;
                case "JToken": output = sourceToken; break;
                case "JObject": output = (JObject)sourceToken; break;
                case "JValue": output = (JValue)sourceToken; break;
                default: output = ParseTargetTypeAndConvert(sourceToken, targetType); break;
            }

            return output;
        }
        private static object ConvertDbItem(DBItem source, string targetType)
        {
            if (targetType == "JToken" || targetType == "JObject")
            {
                return targetType == "JToken" ? source.ToJson() : (JObject)source.ToJson();
            }
            throw new Exception($"Unsupported conversion of DBItem to {targetType}");
        }
        private static object ConvertDbItemList(IEnumerable<DBItem> source, string targetType)
        {
            if (targetType == "JToken" || targetType == "JArray")
            {
                JArray o = new JArray();
                foreach (DBItem item in source)
                {
                    o.Add(item.ToJson());
                }
                return targetType == "JArray" ? o : (JToken)o;
            }
            throw new Exception($"Unsupported conversion of IEnumerable<DBItem> to {targetType}");
        }
        private static object ParseTargetTypeAndConvert(JToken source, string targetType)
        {
            bool isSimpleType = !targetType.Contains("<");

            Type T = isSimpleType ? ParseSimpleType(targetType) : ParseComplexType(targetType);
            return typeof(Entitron).GetMethod("As").MakeGenericMethod(T).Invoke(null, new object[] { source });
        }
        private static Type ParseComplexType(string type)
        {
            string baseType = type.Substring(0, type.IndexOf("<"));
            string typeFullName = "";
            string[] args = type.Substring(type.IndexOf("<") + 1, type.LastIndexOf(">") - type.IndexOf("<") - 1).Split(',');

            Type tBaseType = ParseSimpleType(baseType + "`" + args.Length);
            typeFullName = tBaseType.FullName;

            List<string> argsFullNames = new List<string>();
            foreach (string arg in args)
            {
                Type tArgType = arg.Contains("<") ? ParseComplexType(arg) : ParseSimpleType(arg);
                argsFullNames.Add(tArgType.FullName);
            }

            typeFullName = string.Format("{0}[{1}]", typeFullName, string.Join(",", argsFullNames));

            return Type.GetType(typeFullName);
        }
        private static Type ParseSimpleType(string type)
        {
            bool isNullable = false;
            bool isArray = false;

            type = type.Trim(' ');
            if (type.IndexOf("[]") != -1)
            {
                isArray = true;
                type = type.Remove(type.IndexOf("[]"), 2);
            }
            if (type.IndexOf("?") != -1)
            {
                isNullable = true;
                type = type.Remove(type.IndexOf("?"), 1);
            }

            string typeLower = type.ToLower();

            string fullTypeName = null;
            switch (type)
            {
                case "bool": case "boolean": fullTypeName = "System.Boolean"; break;
                case "byte": fullTypeName = "System.Byte"; break;
                case "char": fullTypeName = "System.Char"; break;
                case "datetime": fullTypeName = "System.DateTime"; break;
                case "datetimeoffset": fullTypeName = "System.DateTimeOffset"; break;
                case "decimal": fullTypeName = "System.Decimal"; break;
                case "double": fullTypeName = "System.Double"; break;
                case "float": fullTypeName = "System.Single"; break;
                case "int16": case "short": fullTypeName = "System.Int16"; break;
                case "int32": case "int": fullTypeName = "System.Int32"; break;
                case "int64": case "long": fullTypeName = "System.Int64"; break;
                case "object": fullTypeName = "System.Object"; break;
                case "sbyte": fullTypeName = "System.SByte"; break;
                case "string": fullTypeName = "System.String"; break;
                case "timespan": fullTypeName = "System.TimeSpan"; break;
                case "uint16": case "ushort": fullTypeName = "System.UInt16"; break;
                case "uint32": case "uint": fullTypeName = "System.UInt32"; break;
                case "uint64": case "ulong": fullTypeName = "System.UInt64"; break;
            }

            if (fullTypeName == null)
            {
                Type T = classList.FirstOrDefault(t => t.Name == type);
                fullTypeName = T != null ? T.FullName : type;
            }

            if (isArray) { fullTypeName += "[]"; }
            if (isNullable) { fullTypeName = string.Format("System.Nullable`1[{0}]", fullTypeName); }

            return Type.GetType(fullTypeName);
        }
        private static T As<T>(JToken source)
        {
            return source.ToObject<T>();
        }
        private static IEnumerable<Type> classList = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsClass);

        [Action(198, "Column to list", "Result")]
        public static List<object> ColumnToList(COREobject core, List<DBItem> TableRows, string ColumnName)
        {
            List<object> result = new List<object>();

            foreach (DBItem row in TableRows)
            {
                if (!row.HasProperty(ColumnName))
                {
                    throw new Exception($"Tapestry action Column to List can't found column {ColumnName} in supplied data");
                }
                result.Add(row[ColumnName]);
            }

            return result;
        }

        [Action(1001, "Compare", "Result")]
        public static bool Compare(COREobject core, DBItem model, string parameter, object value)
        {
            return model[parameter] == value;
        }

        [Action(1002, "Get Model Data", "Data")]
        public static object GetModelData(COREobject core, string ColumnId = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            DBItem model = db.Table(core.BlockAttribute.ModelTableName, SearchInShared).SelectById(core.ModelId);
            if (ColumnId != null)
                return model[ColumnId];

            return model;
        }

        [Action(1003, "Get Table", "Data", "columnNames")]
        public static (List<DBItem>, List<string>) GetTable(COREobject core, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);

            return (table.Select().ToList(), table.Columns.Select(c => c.Name).ToList());
        }

        [Action(1004, "Create DB Item", "AssignedId")]
        public static int CreateDbItem(COREobject core, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            string tableName = TableName ?? core.BlockAttribute.ModelTableName;
            DBTable table = db.Table(tableName, SearchInShared);

            DBItem item = new DBItem(db, table);
            foreach (DBColumn column in table.Columns)
            {
                if (column.Type == DbType.Boolean)
                    item[column.Name] = core.Data.ContainsKey($"__Model.{table.Name}.{column.Name}");
                else if (core.Data.ContainsKey($"__Model.{table.Name}.{column.Name}"))
                {
                    if (column.Type == DbType.DateTime)
                    {
                        if (core.Data[$"__Model.{table.Name}.{column.Name}"] is DateTime)
                            item[column.Name] = core.Data[$"__Model.{table.Name}.{column.Name}"];
                        else
                        {
                            DateTime parsedDateTime = new DateTime();
                            bool parseSuccessful = DateTime.TryParseExact((string)core.Data[$"__Model.{table.Name}.{column.Name}"],
                                new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-ddTHH:mm", "dd.MM.yyyy HH:mm", },
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                            if (parseSuccessful)
                                item[column.Name] = parsedDateTime;
                        }
                    }
                    else if (core.Data[$"__Model.{table.Name}.{column.Name}"] is JValue)
                    {
                        if (column.Type == DbType.Int32)
                            item[column.Name] = ((JValue)core.Data[$"__Model.{table.Name}.{column.Name}"]).ToObject<int>();
                        if (column.Type == DbType.Double)
                            item[column.Name] = ((JValue)core.Data[$"__Model.{table.Name}.{column.Name}"]).ToObject<double>();
                    }
                    else
                        item[column.Name] = core.Data[$"__Model.{table.Name}.{column.Name}"];
                }
            }
            DateTime timestamp = DateTime.Now;
            if (table.Columns.Any(c => c.Name == "ID_USER_VLOZIL"))
            {
                int userId = core.User.Id;
                item["ID_USER_VLOZIL"] = userId;
                item["ID_USER_EDITOVAL"] = userId;
                item["DATUM_VLOZENI"] = timestamp;
                item["DATUM_EDITACE"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date"))
            {
                item["date"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date_purchase"))
            {
                item["date_purchase"] = timestamp;
            }
            table.AddGetId(item);

            return (int)item["id"];
        }

        [Action(1007, "Update DB Item")]
        public static void UpdateDBItem(COREobject core, string TableName = null, int? Id = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            TableName = TableName ?? core.BlockAttribute.ModelTableName;
            int itemId = Id ?? core.ModelId;

            DBTable table = db.Table(TableName, SearchInShared);
            DBItem row = table.SelectById(itemId);
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {TableName}, Id: {itemId}, Akce: Update DB Item (1007))");

            foreach (DBColumn column in table.Columns)
            {
                if (column.Type == DbType.Boolean)
                    row[column.Name] = core.Data.ContainsKey($"__Model.{TableName}.{column.Name}");
                else if (core.Data.ContainsKey($"__Model.{TableName}.{column.Name}"))
                {
                    var inputValue = core.Data[$"__Model.{TableName}.{column.Name}"];
                    if (column.Type == DbType.DateTime && inputValue is string)
                    {
                        DateTime parsedDateTime = new DateTime();
                        bool parseSuccessful = DateTime.TryParseExact((string)inputValue,
                            new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss" },
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                        if (parseSuccessful)
                            row[column.Name] = parsedDateTime;
                    }
                    else
                    {
                        row[column.Name] = inputValue;
                    }
                }
            }

            if (table.Columns.Any(c => c.Name == "ID_USER_EDITOVAL"))
            {
                row["ID_USER_EDITOVAL"] = core.User.Id;
                row["DATUM_EDITACE"] = DateTime.Now;
            }

            table.Update(row, itemId);
            db.SaveChanges();
        }

        [Action(1008, "Update DBItem Without Form")]
        public static void UpdateDBItemWithoutForm(COREobject core, string TableName = null, int? Id = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            TableName = TableName ?? core.BlockAttribute.ModelTableName;
            int itemId = Id ?? core.ModelId;
            DBTable table = db.Table(TableName, SearchInShared);
            DBItem row = table.Select().Where(c => c.Column(DBCommandSet.PrimaryKey).Equal(itemId)).First();
            if (row == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {TableName}, Id: {itemId}, Akce: Update DBItem Without Form (1008))");

            foreach (DBColumn column in table.Columns.Where(c => core.Data.ContainsKey($"__Model.{TableName}.{c.Name}")))
            {
                var inputValue = core.Data[$"__Model.{TableName}.{column.Name}"];
                if (column.Type == System.Data.DbType.DateTime && inputValue is string)
                {
                    DateTime parsedDateTime = new DateTime();
                    bool parseSuccessful = DateTime.TryParseExact((string)inputValue,
                        new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss" },
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                    if (parseSuccessful)
                        row[column.Name] = parsedDateTime;
                }
                else
                {
                    row[column.Name] = inputValue;
                }
            }
            if (table.Columns.Any(c => c.Name == "id_user_change"))
            {
                row["id_user_change"] = core.User.Id;
                row["datetime_change"] = DateTime.Now;
            }
            table.Update(row, itemId);
            db.SaveChanges();
        }

        [Action(1010, "Delete Item")]
        public static void DeleteDBItem(COREobject core, string TableName = null, int? ItemId = null, int? deleteId = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            TableName = TableName ?? core.BlockAttribute.ModelTableName;
            ItemId = ItemId ?? deleteId ?? core.ModelId;

            DBTable table = db.Table(TableName, SearchInShared);

            table.Delete(ItemId.Value);
            db.SaveChanges();
        }

        [Action(1019, "Create dictionary", "Result")]
        public static Dictionary<string, string> CreateDictionary(COREobject core, string StaticKey, string StaticValue)
        {
            return new Dictionary<string, string> { { StaticKey, StaticValue } };
        }
        
        [Action(1020, "Select (filter)", "Data")]
        public static ListJson<DBItem> Select(COREobject core, string[] CondColumn, object[] CondValue, string[] CondOperator, string TableName = null, bool SearchInShared = false, int? Top = null, string OrderBy = null, bool? Descending = null, int? MaxRows = null, string GroupBy = null, string GroupByFunction = null)
        {
            // init
            DBConnection db = core.Entitron;
            
            AscDesc isDescending = Descending == true ? AscDesc.Desc : AscDesc.Asc;

            //
            Tabloid tabloid = db.Tabloid(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);
            var select = tabloid.Select();

            // setConditions
            for (int i = 0; i < CondColumn.Length; i++)
            {
                string condOperator = CondOperator.Length > i && CondOperator[i] != null ? CondOperator[i] : "Equal";
                string condColumn = CondColumn[i];
                object condValue = CondValue[i];

                DBColumn column = tabloid.Columns.Single(c => c.Name == condColumn);
                object value = condOperator != "IsIn"
                    ? DataType.ConvertTo(column.Type, condValue)
                    : condValue;

                switch (condOperator)
                {
                    case "Less":
                        select.Where(c => c.Column(condColumn).Less(value));
                        break;
                    case "LessOrEqual":
                        select.Where(c => c.Column(condColumn).LessOrEqual(value));
                        break;
                    case "Greater":
                        select.Where(c => c.Column(condColumn).Greater(value));
                        break;
                    case "GreaterOrEqual":
                        select.Where(c => c.Column(condColumn).GreaterOrEqual(value));
                        break;
                    case "Equal":
                        select.Where(c => c.Column(condColumn).Equal(value));
                        break;
                    case "IsIn":
                        // string, multiple values
                        if ((condValue is string) && ((string)condValue).Contains(","))
                            select.Where(c => c.Column(condColumn).In((condValue as string).Split(',')));
                        // Enumerable
                        else
                            select.Where(c => c.Column(condColumn).In((IEnumerable<object>)condValue));
                        break;
                    default: // ==
                        select.Where(c => c.Column(condColumn).Equal(value));
                        break;
                }
            }

            // top
            if (Top != null)
                select = select.Limit(Top.Value);

            // order
            if (OrderBy != null)
            {
                select = select.Order(isDescending, OrderBy);

                if (MaxRows != null)
                    select.DropStep(MaxRows.Value, ESqlFunction.LAST, isDescending, OrderBy);
            }

            // Group
            if (GroupBy != null)
            {
                ESqlFunction function = ESqlFunction.SUM;
                if (GroupByFunction != null)
                {
                    switch (GroupByFunction)
                    {
                        case "none":
                            function = ESqlFunction.none;
                            break;
                        case "MAX":
                            function = ESqlFunction.MAX;
                            break;
                        case "MIN":
                            function = ESqlFunction.MIN;
                            break;
                        case "AVG":
                            function = ESqlFunction.AVG;
                            break;
                        case "COUNT":
                            function = ESqlFunction.COUNT;
                            break;
                        case "SUM":
                            function = ESqlFunction.SUM;
                            break;
                        case "FIRST":
                            function = ESqlFunction.FIRST;
                            break;
                        case "LAST":
                            function = ESqlFunction.LAST;
                            break;
                    }
                }
                select.Group(function, columns: GroupBy);
            }

            // return
            return select.ToList();
        }
        
        [Action(1021, "Select (Any)", "Result")]
        public static bool AnyItem(COREobject core, string TableName, bool SearchInShared = false, string[] CondColumn = null, object[] CondValue = null, string[] CondOperation = null)
        {
            // init
            DBConnection db = core.Entitron;

            DBTable table = db.Table(TableName, SearchInShared);

            //
            var select = table.Select();
            int CondCount = CondColumn?.Count() ?? 0;

            // setConditions
            for (int i = 0; i < CondCount; i++)
            {
                // none -> ==
                if (CondOperation.Length < i || CondOperation[i] == null)
                    select.Where(c => c.Column(CondColumn[i]).Equal(CondValue[i]));
                else
                    switch (CondOperation[i])
                    {
                        case "Less":
                            select.Where(c => c.Column(CondColumn[i]).Less(CondValue[i]));
                            break;
                        case "LessOrEqual":
                            select.Where(c => c.Column(CondColumn[i]).LessOrEqual(CondValue[i]));
                            break;
                        case "Greater":
                            select.Where(c => c.Column(CondColumn[i]).Greater(CondValue[i]));
                            break;
                        case "GreaterOrEqual":
                            select.Where(c => c.Column(CondColumn[i]).GreaterOrEqual(CondValue[i]));
                            break;
                        default: // ==
                            select.Where(c => c.Column(CondColumn[i]).Equal(CondValue[i]));
                            break;
                    }
            }

            // return
            return select.Count() > 0;
        }

        [Action(1022, "Select Item (by Id)", "Data")]
        public static DBItem SelectById(COREobject core, string TableName, int Id, bool SearchInShared = false)
        {
            // init
            DBConnection db = core.Entitron;

            var result = db.Table(TableName, SearchInShared).SelectById(Id);
            if (result == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {TableName}, Id: {Id}, Akce: 'Select Item (by Id)' ({1022}))");

            return result;
        }
        
        [Action(1025, "Create DB Item for each")]
        public static void CreateDbItemForEach(COREobject core, string TableName = null, bool SearchInShared = false, string ParentProperty = "", int ParentId = -1)
        {
            TableName = TableName ?? core.BlockAttribute.ModelTableName;

            DBConnection db = core.Entitron;
            DBTable table = db.Table(TableName);

            bool addParentRelation = false;
            if (ParentProperty != "" && ParentId != -1 && table.Columns.Any(c => c.Name == ParentProperty))
                addParentRelation = true;

            DBItem item = new DBItem(db, table);
            foreach (DBColumn column in table.Columns)
            {
                if (column.Type == DbType.Boolean)
                    item[column.Name] = core.Data.ContainsKey($"__Model.{table.Name}.{column.Name}");
                else if (core.Data.ContainsKey($"__Model.{table.Name}.{column.Name}"))
                {
                    if (column.Type == DbType.DateTime)
                    {
                        try
                        {
                            item[column.Name] = DateTime.ParseExact((string)core.Data[$"__Model.{table.Name}.{column.Name}"], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (FormatException)
                        {
                            // skip empty date instead of crashing
                        }
                    }
                    else
                        item[column.Name] = core.Data[$"__Model.{table.Name}.{column.Name}"];
                }
            }
            if (addParentRelation)
            {
                item[ParentProperty] = ParentId;
            }
            table.Add(item);
            for (int panelIndex = 1; core.Data.ContainsKey($"panelCopy{panelIndex}Marker"); panelIndex++)
            {
                item = new DBItem(db, table);
                foreach (DBColumn column in table.Columns)
                {
                    if (column.Type == DbType.Boolean)
                        item[column.Name] = core.Data.ContainsKey($"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}");
                    else if (core.Data.ContainsKey($"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}"))
                    {
                        if (column.Type == DbType.DateTime)
                        {
                            try
                            {
                                item[column.Name] = DateTime.ParseExact((string)core.Data[$"__Model.{table.Name}.{column.Name}"], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            }
                            catch (FormatException)
                            {
                                // skip empty date instead of crashing
                            }
                        }
                        else
                            item[column.Name] = core.Data[$"__Model.panelCopy{panelIndex}.{table.Name}.{column.Name}"];
                    }
                }
                if (addParentRelation)
                {
                    item[ParentProperty] = ParentId;
                }
                table.Add(item);
            }
            db.SaveChanges();
        }

        [Action(1026, "Search in table", "Data")]
        public static ListJson<DBItem> SearchInTable(COREobject core, string TableName, string ColumnName, string Query, string SearchMode = null, bool SearchInShared = false)
        {
            // init
            DBConnection db = core.Entitron;

            string query = "";
            if (SearchMode != null)
            {
                switch (SearchMode)
                {
                    case "start":
                    default:
                        query = Query + "%";
                        break;
                    case "end":
                        query = "%" + Query;
                        break;
                    case "anywhere":
                        query = "%" + Query + "%";
                        break;
                }
            }
            else
                query = Query + "%";

            return db.Tabloid(TableName, SearchInShared).Select()
                .Where(c => c.Column(ColumnName).LikeCaseInsensitive(query)).ToList();
        }

        [Action(1027, "Get cell from row", "CellValue")]
        public static object GetCellFromRow(COREobject core, string ColumnName, DBItem RowData)
        {
            return RowData[ColumnName];
        }
        
        [Action(1028, "Get dictionary from table", "Result")]
        public static Dictionary<string, string> GetDictionaryFromTable(COREobject core, object TableData = null, string KeyColumn = null, string ValueColumn = null, string StaticKey = null, string StaticValue = null)
        {
            var result = new Dictionary<string, string>();
            if (StaticKey != null && StaticValue != null)
                result[StaticKey] = StaticValue;

            else if (TableData is DBItem)
            {
                var row = (DBItem)TableData;
                string key = (string)row[KeyColumn];
                string value = (string)row[ValueColumn];
                if (result.ContainsKey(key))
                    result[key] = value;
                else
                    result.Add(key, value);
            }
            else
            {
                foreach (var row in (List<DBItem>)TableData)
                {
                    string key = (string)row[KeyColumn];
                    string value = (string)row[ValueColumn];
                    if (result.ContainsKey(key))
                        result[key] = value;
                    else
                        result.Add(key, value);
                }
            }
            return result;
        }

        [Action(1029, "Set state")]
        public static void SetState(COREobject core, string ColumnName, int StateId, string TableName = null, int? RowId = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            
            DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);
            DBItem change = new DBItem(db, table);
            change[ColumnName] = StateId;
            table.Update(change, RowId ?? core.ModelId);

            db.SaveChanges();
        }

        [Action(1030, "Add to dictionary", "Result")]
        public static Dictionary<string, object> AddToDictionary(COREobject core, object RowData = null, bool AutoMapping = false, string KeyMapping = null, Dictionary<string, object> Dictionary = null)
        {
            // init
            DBConnection db = core.Entitron;

            Dictionary = Dictionary ?? new Dictionary<string, object>();

            DBItem rowData = null;
            bool useRowData = true;
            if (RowData == null)
                useRowData = false;
            else if (RowData is string)
                rowData = (DBItem)core.Data[(string)RowData];
            else if (RowData is DBItem)
                rowData = (DBItem)RowData;
            else if (RowData is IEnumerable<DBItem>)
                rowData = (RowData as IEnumerable<DBItem>).FirstOrDefault();
            
            List<string> mappingStringList = AutoMapping || KeyMapping == null
                ? rowData.getColumnNames().Select(a => $"{a}:{a}").ToList()
                : KeyMapping.Split(',').ToList();

            foreach (string mappingString in mappingStringList)
            {
                List<string> tokens = mappingString.Split(':').ToList();
                if (tokens.Count != 2)
                    continue;
                string columnName = tokens[1];
                object value;
                if (useRowData)
                    value = rowData[columnName];
                else
                    value = KeyValueString.ParseValue(columnName, core.Data);
                if (Dictionary.ContainsKey(tokens[0]))
                {
                    if (value == null)
                        Dictionary[tokens[0]] = "";
                    else if (value is List<object>)
                        Dictionary[tokens[0]] = value;
                    else
                        Dictionary[tokens[0]] = value.ToString();
                }
                else
                {
                    if (value == null)
                        Dictionary.Add(tokens[0], "");
                    else if (value is List<object>)
                        Dictionary.Add(tokens[0], value);
                    else
                        Dictionary.Add(tokens[0], value.ToString());
                }
            }
            return Dictionary;
        }

        [Action(1031, "Get Model URL", "Result")]
        public static string GetModelUrl(COREobject core, string BlockName, int Id)
        {
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            return $"{hostname}/{appName}/{BlockName}?modelId={Id}";
        }

        [Action(1033, "Get dictionaries from rows", "Result")]
        public static List<Dictionary<string, string>> GetDictionariesFromRows(COREobject core, object InputData)
        {
            var result = new List<Dictionary<string, string>>();

            if (InputData is DBItem)
                InputData = new DBItem[] { (DBItem)InputData };
            if (!(InputData is IEnumerable<DBItem>))
                throw new Exception("Invalid input data!");

            foreach (DBItem inputRow in (IEnumerable<DBItem>)InputData)
            {
                var jObject = (JObject)inputRow.ToJson();
                var rowDictionary = new Dictionary<string, string>();
                foreach (var pair in jObject)
                {
                    rowDictionary.Add(pair.Key, pair.Value.ToString());
                }
                result.Add(rowDictionary);
            }

            return result;
        }

        [Action(1034, "Select from view", "Result")]
        public static ListJson<DBItem> SelectFromView(COREobject core, string ViewName, string OrderBy = null, bool Descending = false, bool SearchInShared = false, string[] ColumnName = null, object[] Value = null, int? MaxRows = null, string GroupBy = null, string GroupByFunction = null)
        {
            // init
            DBConnection db = core.Entitron;

            // get view
            AscDesc isDescending = Descending ? AscDesc.Desc : AscDesc.Asc;

            Select select = db.Select(ViewName, SearchInShared);
            int conditionIndex = 0;
            /// each condition
            while (ColumnName.Length > conditionIndex)
            {
                string columnName = ColumnName[conditionIndex];
                object value = Value[conditionIndex];

                if (columnName == null && value == null)
                {
                    conditionIndex++;
                    continue;
                }

                // condition is list
                if (!(value is string) && value is IEnumerable)
                {
                    // condition list is empty -> return empty list
                    if (((IEnumerable<object>)value).Count() == 0)
                    {
                        return new ListJson<DBItem>();
                    }

                    select.Where(c => c.Column(columnName).In((IEnumerable<object>)value));
                }
                // condition is list of strings
                else if ((value is string) && ((string)value).Contains(","))
                {
                    string[] list = (value as string).Split(',');
                    select.Where(c => c.Column(columnName).In(list));
                }
                // condition is object
                else
                    select.Where(c => c.Column(columnName).Equal(value));

                conditionIndex++;
            }

            // MaxRows
            if (MaxRows != null && OrderBy != null)
                select.DropStep(MaxRows.Value, ESqlFunction.LAST, isDescending, OrderBy);

            // order
            select.Order(isDescending, OrderBy);

            // group
            if (GroupBy != null)
            {
                ESqlFunction function = ESqlFunction.SUM;
                if (GroupByFunction != null)
                {
                    switch (GroupByFunction)
                    {
                        case "none":
                            function = ESqlFunction.none;
                            break;
                        case "MAX":
                            function = ESqlFunction.MAX;
                            break;
                        case "MIN":
                            function = ESqlFunction.MIN;
                            break;
                        case "AVG":
                            function = ESqlFunction.AVG;
                            break;
                        case "COUNT":
                            function = ESqlFunction.COUNT;
                            break;
                        case "SUM":
                            function = ESqlFunction.SUM;
                            break;
                        case "FIRST":
                            function = ESqlFunction.FIRST;
                            break;
                        case "LAST":
                            function = ESqlFunction.LAST;
                            break;
                    }
                }
                select.Group(function, columns: GroupBy);
            }

            return select.ToList();
        }
        
        [Action(1035, "Mass edit")]
        public static void MassEdit(COREobject core, string TableName, string ColumnName, object Value, object TableData = null, string IdList = null, string ValueType = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            
            DBTable table = db.Table(TableName, SearchInShared);
            if (ValueType != null)
            {
                switch (ValueType)
                {
                    case "string":
                        Value = Convert.ToString(Value);
                        break;
                    case "datetime":
                        {
                            if (Value is string)
                                Value = DateTime.ParseExact((string)Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            else
                                Value = Convert.ToDateTime(Value);
                        }
                        break;
                    case "realdatetime":
                        Value = Convert.ToDateTime(Value);
                        break;
                    case "bool":
                        Value = Convert.ToBoolean(Value);
                        break;
                    case "int":
                        Value = Convert.ToInt32(Value);
                        break;
                }
            }

            var idList = new List<object>();
            List<DBItem> results = null;
            if (TableData != null)
            {
                if (TableData is DBItem)
                {
                    var rowList = new List<DBItem>();
                    rowList.Add((DBItem)TableData);
                    results = rowList;
                }
                else
                {
                    results = (List<DBItem>)TableData;
                }
            }
            else if (IdList != null)
            {
                idList = IdList.Split(',').Select(int.Parse).Cast<object>().ToList();
                results = table.Select().Where(c => c.Column("id").In(idList)).ToList();
            }
            else
            {
                //results = table.Select().ToList();
            }
            foreach (var row in results)
            {
                row[ColumnName] = Value;
                table.Update(row, (int)row["id"]);
            }
            db.SaveChanges();
        }

        [Action(1036, "Select top", "Result")]
        public static DBItem SelectTop(COREobject core, List<DBItem> TableData, string SortingColumn)
        {
            return TableData.OrderByDescending(c => c[SortingColumn]).First();
        }

        [Action(1037, "Create DB Items for for rows")]
        public static void CreateDBItemsForRows(COREobject core, string TableName, List<DBItem> InputData, string KeyMapping, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            var keyMappingDictionary = new Dictionary<string, string>();

            foreach (var segment in KeyMapping.Split(','))
            {
                var pair = segment.Split(':');
                if (pair.Length == 2)
                {
                    keyMappingDictionary.Add(pair[0], pair[1]);
                }
            }

            DBTable table = db.Table(TableName, SearchInShared);
            DateTime timestamp = DateTime.Now;
            foreach (var inputRow in InputData)
            {
                var newRowItem = new DBItem(db, table);
                foreach (DBColumn column in table.Columns)
                {
                    if (keyMappingDictionary.ContainsKey(column.Name))
                    {
                        string dictionaryValue = keyMappingDictionary[column.Name];
                        if (dictionaryValue.Substring(0, 2) == "$$")
                            newRowItem[column.Name] = inputRow[dictionaryValue.Substring(2)];
                        else
                            newRowItem[column.Name] = KeyValueString.ParseValue(dictionaryValue, core.Data);
                    }
                }
                if (table.Columns.Any(c => c.Name == "id_user_insert"))
                {
                    int userId = core.User.Id;
                    newRowItem["id_user_insert"] = userId;
                    newRowItem["id_user_change"] = userId;
                    newRowItem["datetime_insert"] = timestamp;
                    newRowItem["datetime_change"] = timestamp;
                }
                table.Add(newRowItem);
            }
            db.SaveChanges();
        }

        [Action(1039, "Order by column", "Result")]
        public static List<DBItem> OrderByColumn(COREobject core, List<DBItem> TableData, string Column1, string Column2 = null)
        {
            List<DBItem> sortedTableData;
            if (Column2 != null)
                sortedTableData = TableData.OrderBy(x => x[Column1]).ThenBy(x => x[Column2]).ToList();
            else
                sortedTableData = TableData.OrderBy(x => x[Column1]).ToList();

            return sortedTableData;
        }

        [Action(1040, "Select count", "Result")]
        public static int SelectCount(COREobject core, List<DBItem> TableData)
        {
            return TableData.Count;
        }

        [Action(1041, "Get App URL", "Result")]
        public static string GetAppUrl(COREobject core)
        {
            string hostname = TapestryUtils.GetServerHostName();
            string appName = core.Application.Name;
            return $"{hostname}/{appName}";
        }

        [Action(1042, "Get Appsettings", "Result")]
        public static DBItem GetAppSettings(COREobject core)
        {
            // init
            DBConnection db = core.Entitron;
            return db.Table("AppConfig").Select().First();
        }

        [Action(1043, "Import CSV", "Success", "Message", "CountAdded")]
        public static (bool, string, int) ImportCSV(COREobject core, string TableName, string CsvSource = null, string Delimiter = ";", string DateTimeFormat = "yyyy-MM-dd", string UniqueColumns = null, bool HasFieldsInQuotes = false, bool isServerFile = false)
        {
            // init
            DBConnection db = core.Entitron;

            int countAdded = 0;
            //jméno form inputu, nebo cesta k souboru
            List<string> uniqueColumns = UniqueColumns != null ? UniqueColumns.Split(',').ToList() : new List<string>();
            CultureInfo czechCulture = new CultureInfo("cs-CZ");
            var columnMetadataList = db.Application.ColumnMetadata.Where(c => c.TableName == TableName).ToList();

            DBTable table = db.Table(TableName, false);
            Dictionary<int, DBColumn> columnsMap = new Dictionary<int, DBColumn>();

            IEnumerable<byte[]> files = isServerFile
                ? new List<byte[]> { File.ReadAllBytes(CsvSource) }
                : core.GetRequestFiles().Select(pair => pair.Value);
            
            List<string> messages = new List<string>();
            Encoding cp1250 = Encoding.GetEncoding(1250);
            
            foreach (byte[] file in files)
            {
                using (CsvReader reader = new CsvReader(new StreamReader(new MemoryStream(file), cp1250)))
                {
                    reader.Configuration.Delimiter = Delimiter;
                    reader.Configuration.TrimFields = true;
                    reader.Configuration.TrimHeaders = true;
                    reader.Configuration.Encoding = cp1250;

                    if (reader.ReadHeader())
                    {
                        string[] fields = reader.FieldHeaders;
                        int i = 0;
                        foreach (string field in fields)
                        {
                            var colMetadata = columnMetadataList.SingleOrDefault(c => c.ColumnDisplayName == field);
                            string colName = colMetadata == null ? field : colMetadata.ColumnName;
                            if (table.Columns.Where(c => c.Name == colName).Count() > 0)
                            {
                                columnsMap.Add(i, table.Columns.Where(c => c.Name == colName).First());
                            }
                            i++;
                        }
                    }
                    else
                    {
                        messages.Add("Nepodařilo se načíst názvy sloupců. Nelze pokračovat.");
                        continue;
                    }


                    while (reader.Read())
                    {
                        long line = reader.Row;
                        string[] fields = reader.CurrentRecord;

                        int i = 0;
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        bool isValid = true;

                        foreach (string value in fields)
                        {
                            // Neznámé sloupce ignorujeme
                            if (!columnsMap.ContainsKey(i))
                            {
                                i++;
                                continue;
                            }

                            // Prázdné hodnoty vynecháme
                            if (string.IsNullOrEmpty(value))
                            {
                                i++;
                                continue;
                            }

                            DBColumn col = columnsMap[i];

                            switch (col.Type)
                            {
                                case DbType.String:
                                    data.Add(col.Name, value);
                                    break;
                                case DbType.Boolean:
                                    bool parsedBool;
                                    if (bool.TryParse(value, out parsedBool))
                                    {
                                        data.Add(col.Name, parsedBool);
                                    }
                                    else
                                    {
                                        if (value == "0")
                                        {
                                            data.Add(col.Name, false);
                                        }
                                        else if (value == "1")
                                        {
                                            data.Add(col.Name, true);
                                        }
                                        else
                                        {
                                            isValid = false;
                                            messages.Add($"Řádek {line}, sloupec {col.Name}: hodnota musí být: logická hodnota. Řádek vynechán.");
                                        }
                                    }
                                    break;
                                case DbType.Int32:
                                    int parsedInt;
                                    if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedInt))
                                    {
                                        data.Add(col.Name, parsedInt);
                                    }
                                    else
                                    {
                                        isValid = false;
                                        messages.Add($"Řádek {line}, sloupec {col.Name}: hodnota musí být: celé číslo. Řádek vynechán.");
                                    }
                                    break;
                                case DbType.Double:
                                    double parsedDouble;
                                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble))
                                    {
                                        data.Add(col.Name, parsedDouble);
                                    }
                                    else
                                    {
                                        isValid = false;
                                        messages.Add($"Řádek {line}, sloupec {col.Name}: hodnota musí být: celé nebo desetinní číslo. Řádek vynechán.");
                                    }
                                    break;
                                case DbType.DateTime:
                                    try
                                    {
                                        DateTime parsedDateTime = DateTime.Parse(value, czechCulture);
                                        data.Add(col.Name, parsedDateTime);
                                    }
                                    catch (FormatException)
                                    {
                                        isValid = false;
                                        messages.Add($"Řádek {line}, sloupec {col.Name}: hodnota musí být: platné datum. Řádek vynechán.");
                                    }
                                    break;
                            }
                            i++;
                        }

                        if (!isValid)
                        {
                            continue;
                        }

                        if (uniqueColumns.Count > 0)
                        {
                            try
                            {
                                var select = table.Select();

                                // setConditions
                                foreach (string colName in uniqueColumns)
                                {
                                    string colName2 = columnMetadataList.SingleOrDefault(c => c.ColumnDisplayName == colName).ColumnName;
                                    object condValue = data[colName2].ToString();
                                    if (colName == "IČO")
                                        condValue = data[colName2].ToString().PadLeft(8, '0');
                                    DBColumn column = table.Columns.Single(c => c.Name == colName2);

                                    select.Where(c => c.Column(colName2).Equal(condValue));
                                }

                                // check if row already exists
                                if (select.ToList().Count > 0)
                                {
                                    isValid = false;
                                    messages.Add($"Řádek {line}: Tabulka již obsahuje takovýto řádek. Řádek vynechán.");
                                }
                            }
                            catch (KeyNotFoundException)
                            {
                                isValid = false;
                                messages.Add($"Řádek {line}: Unikátní sloupec není vyplněn. Řádek vynechán.");
                            }
                        }

                        if (isValid)
                        {
                            data.Add("Datum_vlozeni", DateTime.Now);
                            data.Add("Cas_editace", DateTime.Now);
                            data.Add("Editoval", core.User.Id);
                            DBItem item = new DBItem(db, table);
                            foreach (KeyValuePair<string, object> kv in data)
                            {
                                item[kv.Key] = kv.Value;
                            }
                            table.Add(item);
                            countAdded++;
                        }
                    }
                }
            }

            db.SaveChanges();

            return (messages.Count() == 0, string.Join("<br>", messages), countAdded);
        }

        [Action(1045, "EXEC SP", "Result")]
        public static bool Exec(COREobject core, string ProcedureName, string[] paramName, string[] paramValue)
        {
            // init
            DBConnection db = core.Entitron;
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            // set params
            for (int i = 0; i < paramName.Length; i++)
            {
                parameters.Add(paramName[i], paramValue[i]);
            }

            // return
            return db.ExecSP(ProcedureName, parameters);
        }

        [Action(1046, "Truncate table", "Result")]
        public static void Truncate(COREobject core, string TableName)
        {
            core.Entitron.TableTruncate(TableName);
        }

        [Action(1047, "JSON to DbItem list", "Result")]
        public static bool JSON2DbItemList(COREobject core, string TableName, string BaseName, JToken Data, string ItemName = "item", bool SearchInShared = false)
        {
            try
            {
                // init
                DBConnection db = core.Entitron;

                /****************************************************************************************
                ** MOCKUP DATA                                                                         **
                *****************************************************************************************
                string jsonText;
                try {
                    XmlDocument xml = new XmlDocument();
                    xml.Load("c:/users/mnvk8/Downloads/response.xml");
                    jsonText = JsonConvert.SerializeXmlNode(xml);
                }
                catch (Exception e) {
                    if (e is ArgumentNullException || e is XmlException) {
                        jsonText = "";// JsonConvert.SerializeObject(response);
                    }
                    else {
                        throw e;
                    }
                }
                JToken data = JToken.Parse(jsonText);
                ****************************************************************************************/

                DBTable table = db.Table(TableName, SearchInShared);

                Dictionary<string, DBColumn> columnExists = new Dictionary<string, DBColumn>();
                Dictionary<string, DbType> columnType = new Dictionary<string, DbType>();

                var items = Data.SelectToken($"$..{BaseName}.{ItemName}");
                foreach (JToken item in items)
                {
                    DBItem entity = new DBItem(db, table);
                    foreach (JProperty pair in item)
                    {
                        // Zjistíme, jestli ten slupec v tabulce vůbec existuje
                        string columnName = pair.Name.ToLowerInvariant();
                        if (!columnExists.ContainsKey(columnName))
                        {
                            DBColumn column = table.Columns.Where(c => c.Name.ToLowerInvariant() == columnName).FirstOrDefault();

                            columnExists.Add(columnName, column);
                            if (column != null)
                            {
                                columnType.Add(columnName, column.Type);
                            }
                        }

                        if (columnExists[columnName] != null)
                        {
                            var columnInfo = columnExists[columnName];
                            entity[columnInfo.Name] = DataType.ConvertTo(columnType[columnName], pair);
                        }
                    }
                    table.Add(entity);
                }

                db.SaveChanges();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        [Action(1048, "Generate unique ID", "Result")]
        public static int GenerateUniqueId(COREobject core, string TableName, string ColumnName, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;
            
            var results = db.Select(TableName, SearchInShared).ToList();
            int prevoiusId = results.Count > 0 ? results.Select(c => (int)c[ColumnName]).Max() : 0;
            return prevoiusId + 1;
        }
        
        [Action(1049, "Get Lists Element By Index", "Result", "Error")]
        public static (object, bool) GetListsElementByIndex(COREobject core, IEnumerable<object> List, int Index)
        {
            try
            {
                if (List.Count() > Index)
                    return (List.ElementAt(Index), false);
                else
                    return (null, false);
            }
            catch (Exception)
            {
                return (null, true);
            }
        }

        [Action(1050, "Group table rows", "Result")]
        public static List<DBItem> GroupTableRows(COREobject core, List<DBItem> TableData, string GroupingColumn, int DateTimeSensitivity = 3)
        {
            List<DBItem> result = new List<DBItem>();

            // Sort tableData at first
            List<DBItem> sortedTableData = TableData.OrderBy(c => c[GroupingColumn]).ToList();
            DBItem element = sortedTableData[0];
            if (sortedTableData.Count == 1)
            {
                result.Add(element);
            }
            for (int i = 1; i < sortedTableData.Count; i++)
            {
                var currentRow = sortedTableData[i];
                bool isDateTime = currentRow[GroupingColumn] is DateTime; ;
                if ((!isDateTime && currentRow[GroupingColumn].ToString() == element[GroupingColumn].ToString()) || (isDateTime && (Convert.ToDateTime(currentRow[GroupingColumn]) - Convert.ToDateTime(element[GroupingColumn])).TotalMinutes <= DateTimeSensitivity))
                {
                    foreach (string columnName in element.getColumnNames())
                    {
                        if ((element[columnName] is int || element[columnName] is double) && columnName.ToUpper() != "id".ToUpper())
                        {
                            double current = currentRow[columnName] != DBNull.Value ? Convert.ToDouble(currentRow[columnName]) : 0;
                            current += element[columnName] != DBNull.Value ? Convert.ToDouble(element[columnName]) : 0;
                            element[columnName] = current;
                            if (i == sortedTableData.Count - 1)
                            {
                                element[columnName] = current;
                                result.Add(element);
                            }
                        }
                    }
                }
                else
                {
                    result.Add(element);

                    element = currentRow;
                    if (i == sortedTableData.Count - 1)
                        result.Add(currentRow);
                }
            }


            return result;
        }

        [Action(1051, "Select from view 2", "Data")]
        public static List<DBItem> SelectFromView2(COREobject core, string ViewName, string[] CondColumn, object[] CondValue, string[] CondOperator, bool SearchInShared = false, string OrderBy = null, bool Descending = false, int? MaxRows = null, string GroupBy = null, string GroupByFunction = null)
        {
            return Select(core, TableName: ViewName, CondColumn: CondColumn, CondValue: CondValue, CondOperator: CondOperator, SearchInShared: SearchInShared, OrderBy: OrderBy, Descending: Descending, MaxRows: MaxRows, GroupBy: GroupBy, GroupByFunction: GroupByFunction);
        }

        [Action(1053, "Select First(filter)", "Data")]
        public static DBItem SelectFirst(COREobject core, string TableName, string[] CondColumn, object[] CondValue, string[] CondOperator, string OrderBy = null, bool Descending = false, bool SearchInShared = false, int? MaxRows = null, string GroupBy = null, string GroupByFunction = null)
        {
            return Select(core, TableName: TableName, SearchInShared: SearchInShared, Top: 1, OrderBy: OrderBy, Descending: Descending, CondColumn: CondColumn, CondValue: CondValue, CondOperator: CondOperator, MaxRows: MaxRows, GroupBy: GroupBy, GroupByFunction: GroupByFunction).FirstOrDefault();
        }

        [Action(1052, "Filter Table By Column", "Result")]
        public static List<DBItem> FilterTableByColumn(COREobject core, List<DBItem> TableData, string ColumnName)
        {
            if (TableData.Count <= 0)
                return TableData;

            if (!TableData[0].HasProperty(ColumnName))
                throw new Exception("Column name not found!");

            List<DBItem> outputTable = new List<DBItem>();
            var uniques = new HashSet<object>();

            foreach (DBItem tableRow in TableData)
            {
                if (!uniques.Contains(tableRow[ColumnName]))
                {
                    uniques.Add(tableRow[ColumnName]);
                    outputTable.Add(tableRow);
                }
            }

            return outputTable;
        }

        [Action(1060, "Search in view", "Data")]
        public static ListJson<DBItem> SearchInView(COREobject core, string TableName, string ColumnName, string Query, string SearchMode = null, bool SearchInShared = false)
        {
            return SearchInTable(core, TableName, ColumnName, Query, SearchMode, SearchInShared);
        }

        [Action(1067, "Clear old pool action")]
        public static void ClearOldPool(COREobject core)
        {
            DBConnection db = core.Entitron;
            IDbCommand cmd = db.CommandSet.Command;

            cmd.CommandText =
                "DECLARE @v_spid INT " +
                "DECLARE c_Users CURSOR " +
                "   FAST_FORWARD FOR " +
                "   SELECT SPID " +
                "   FROM master..sysprocesses (NOLOCK) " +
                "   WHERE spid>50  " +
                "   AND status='sleeping'  " +
                "   AND (program_name = 'EntityFramework' OR program_name = 'Entitron') " +
                "   AND DATEDIFF(mi,last_batch,GETDATE())>=60 " +
                "   AND spid<>@@spid " +

                "OPEN c_Users " +
                "FETCH NEXT FROM c_Users INTO @v_spid " +
                "WHILE (@@FETCH_STATUS=0) " +
                "BEGIN " +
                "  PRINT 'KILLing '+CONVERT(VARCHAR,@v_spid)+'...' " +
                "  EXEC('KILL '+@v_spid) " +
                "  FETCH NEXT FROM c_Users INTO @v_spid " +
                "END " +

                "CLOSE c_Users " +
                "DEALLOCATE c_Users";

            db.ExecuteNonQuery(cmd);
        }

        [Action(1106, "Save list")]
        public static void SaveList(COREobject core, string tableName, IEnumerable<DBItem> list)
        {
            DBConnection db = core.Entitron;

            DBTable targetTable = db.Table(tableName);

            targetTable.AddRange(list);
            db.SaveChanges();
        }

        [Action(1524, "Select Last Item", "Result")]
        public static DBItem SelectLastItem(COREobject core, string TableName, string SortingColumn, string[] CondColumn, object[] CondValue, string[] CondOperator, bool Descending = false, bool SearchInShared = false)
        {
            return Select(core, TableName: TableName, SearchInShared: SearchInShared, Top: 1, OrderBy: SortingColumn, Descending: !Descending, MaxRows: null, CondColumn: CondColumn, CondValue: CondValue, CondOperator: CondOperator, GroupBy: null, GroupByFunction: null).FirstOrDefault();
        }

        [Action(1938, "Delete in table", "Data")]
        public static void DeleteInTable(COREobject core, string[] CondColumn, string[] CondOperator, object[] CondValue, string TableName = null, bool SearchInShared = false, string OrderBy = null, bool Descending = false)
        {
            // init
            DBConnection db = core.Entitron;
            TableName = TableName ?? core.BlockAttribute.ModelTableName;

            ListJson<DBItem> columns = db.ExecuteRead(db.CommandSet.LIST_column(db, TableName), new DBTable(db) { Name = TableName });

            //
            DBTable table = db.Table(TableName, SearchInShared);
            var delete = table.Delete();

            // setConditions
            for (int i = 0; i < CondColumn.Length; i++)
            {
                string condOperator = CondOperator.Length > i && CondOperator[i] != null ? CondOperator[i] : "Equal";
                string condColumn = CondColumn[i];
                object condValue = CondValue[i];

                DBItem column = columns.Single(c => (string)c["name"] == condColumn);
                object value = condOperator != "IsIn"
                    ? DataType.ConvertTo(DataType.FromDBName((string)column["typeName"], db.Type), condValue)
                    : column["typeName"];

                switch (condOperator)
                {
                    case "Less":
                        delete.Where(c => c.Column(condColumn).Less(value));
                        break;
                    case "LessOrEqual":
                        delete.Where(c => c.Column(condColumn).LessOrEqual(value));
                        break;
                    case "Greater":
                        delete.Where(c => c.Column(condColumn).Greater(value));
                        break;
                    case "GreaterOrEqual":
                        delete.Where(c => c.Column(condColumn).GreaterOrEqual(value));
                        break;
                    case "Equal":
                        delete.Where(c => c.Column(condColumn).Equal(value));
                        break;
                    case "IsIn":
                        // string, multiple values
                        if ((condValue is string) && ((string)condValue).Contains(","))
                            delete.Where(c => c.Column(condColumn).In((condValue as string).Split(',')));
                        // Enumerable
                        else
                            delete.Where(c => c.Column(condColumn).In((IEnumerable<object>)condValue));
                        break;
                    default: // ==
                        delete.Where(c => c.Column(condColumn).Equal(value));
                        break;
                }
            }

            // order
            delete.Run();
        }

        [Action(5021, "Convert Json String To JToken", "Result")]
        public static JToken ConvertJsonStringToJToken(COREobject core, string JsonString)
        {
            return JToken.Parse(JsonString);
        }

        [Action(5022, "JToken to String", "Result")]
        public static string JTokenToString(COREobject core, JValue value)
        {
            return value.ToObject<string>();
        }

        [Action(5222, "JArray: Insert into DB", "Result")]
        public static void JArrayInsertIntoDB(COREobject core, JArray JArray, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);

            foreach (JObject jo in JArray)
            {
                Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                TapestryUtils.ParseJObject(jo, parsedColumns);

                DBItem parsedRow = new DBItem(db, null);
                foreach (var parsedCol in parsedColumns)
                    parsedRow[parsedCol.Key] = parsedCol.Value;

                DBItem item = new DBItem(db, table);
                foreach (DBColumn col in table.Columns)
                {
                    if (col.Name == DBCommandSet.PrimaryKey)
                        continue;
                    string parsedColName = (col.Name == "ext_id") ? DBCommandSet.PrimaryKey : col.Name;
                    item[col.Name] = parsedRow[parsedColName];
                }

                table.Add(item);
            }
            db.SaveChanges();
        }

        [Action(5223, "JArray: Update DB", "Result")]
        public static bool JArrayUpdateDB(COREobject core, JArray JArray, string UniqueCol = null, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            if (JArray.HasValues)
            {
                DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);

                string UniqueExtCol; //basicly foreign key
                if (UniqueCol != null)
                {
                    UniqueExtCol = UniqueCol;
                }
                else
                {
                    UniqueCol = "ext_id";
                    UniqueExtCol = DBCommandSet.PrimaryKey;
                }
                if (!table.Columns.Any(c => c.Name == UniqueCol))
                    throw new Exception($"Table column named '{UniqueCol}' not found!");
                foreach (JObject jo in JArray)
                {
                    Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                    TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);
                    DBItem parsedRow = new DBItem(db, table, parsedColumns);

                    DBItem updatedRow = table.Select().Where(c => c.Column(UniqueCol).Equal(parsedRow[UniqueExtCol])).FirstOrDefault();

                    if (updatedRow != null) //update
                    {
                        foreach (var col in parsedRow.getColumnNames())
                        {
                            if (updatedRow.getColumnNames().Contains(col) && col != DBCommandSet.PrimaryKey && col != UniqueCol)
                            {
                                updatedRow[col] = parsedRow[col];
                            }
                        }
                        table.Update(updatedRow, (int)updatedRow[DBCommandSet.PrimaryKey]);
                    }
                    else // insert row if it does not exist 
                    {
                        DBItem item = new DBItem(db, table);
                        foreach (DBColumn col in table.Columns)
                        {
                            if (col.Name == DBCommandSet.PrimaryKey)
                                continue;
                            string parsedColName = (col.Name == "ext_id") ? DBCommandSet.PrimaryKey : col.Name;
                            item[col.Name] = parsedRow[parsedColName];
                        }

                        table.Add(item);
                    }
                }
                db.SaveChanges();
                return true;
            }
            else
            {
                Watchtower.OmniusLog.Log($"Input JArray has no values! Action aborted", Watchtower.OmniusLogLevel.Warning, Watchtower.OmniusLogSource.Tapestry, core.Application, core.User);
                return false;
            }
        }

        [Action(5224, "JArray: Insert Unique into DB", "Result")]
        public static bool JArrayInsertUniqueIntoDB(COREobject core, JArray JArray, string UniqueCol = null, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = core.Entitron;

            if (JArray.HasValues)
            {
                DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName, SearchInShared);

                string UniqueExtCol; //basicly foreign key
                if (UniqueCol != null)
                {
                    UniqueExtCol = UniqueCol;
                }
                else
                {
                    UniqueCol = "ext_id";
                    UniqueExtCol = DBCommandSet.PrimaryKey;
                }
                if (!table.Columns.Any(c => c.Name == UniqueExtCol))
                    throw new Exception($"Table column named '{UniqueExtCol}' not found!");

                HashSet<string> tableUniques = new HashSet<string>(table.Select(UniqueCol).ToList().Select(x => x[UniqueCol].ToString()).ToList());

                HashSet<DBItem> parsedItems = new HashSet<DBItem>();
                foreach (JObject jo in JArray)
                {
                    Dictionary<string, object> parsedColumns = new Dictionary<string, object>();
                    TapestryUtils.ParseJObjectRecursively(jo, parsedColumns);

                    parsedItems.Add(new DBItem(db, table, parsedColumns));
                }
                HashSet<string> parsedExtIds = new HashSet<string>(); //ids of items from input jarray
                foreach (var item in parsedItems)
                    parsedExtIds.Add(item[UniqueExtCol].ToString());
                HashSet<string> newUniqueIDs = new HashSet<string>(parsedExtIds.Except(tableUniques));
                parsedItems.RemoveWhere(x => !newUniqueIDs.Contains(x[UniqueExtCol]));

                foreach (DBItem parsedItem in parsedItems)
                {
                    DBItem item = new DBItem(db, table);
                    foreach (DBColumn col in table.Columns)
                    {
                        if (col.Name == DBCommandSet.PrimaryKey)
                            continue;
                        string parsedColName = (col.Name == "ext_id") ? DBCommandSet.PrimaryKey : col.Name;
                        if (parsedItem[parsedColName] != null)//
                            item[col.Name] = parsedItem[parsedColName];
                    }

                    table.Add(item);
                }
                db.SaveChanges();

                return true;
            }
            else
            {
                Watchtower.OmniusLog.Log($"Input JArray has no values! Action aborted", Watchtower.OmniusLogLevel.Warning, Watchtower.OmniusLogSource.Tapestry, core.Application, core.User);
                return false;
            }
        }

        [Action(9957, "Create DB Item", "AssignedId")]
        public static int InsertItemToTable(COREobject core, DBItem Item, string TableName = null)
        {
            DBConnection db = core.Entitron;

            DBTable table = db.Table(TableName ?? core.BlockAttribute.ModelTableName);

            foreach (DBColumn column in table.Columns)
            {
                string modelColumnName = $"__Model.{table.Name}.{column.Name}";

                if (column.Type == DbType.Boolean)
                    Item[column.Name] = core.Data.ContainsKey(modelColumnName);
                else if (core.Data.ContainsKey(modelColumnName))
                {
                    if (column.Type == DbType.DateTime && core.Data[modelColumnName] is string)
                    {
                        DateTime parsedDateTime = new DateTime();
                        bool parseSuccessful = DateTime.TryParseExact((string)core.Data[modelColumnName],
                            new string[] { "d.M.yyyy H:mm:ss", "d.M.yyyy", "H:mm:ss", "yyyy-MM-dd H:mm:ss", "yyyy-MM-dd" },
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime);

                        if (parseSuccessful)
                            Item[column.Name] = parsedDateTime;
                    }
                    else
                        Item[column.Name] = core.Data[modelColumnName];
                }
            }
            DateTime timestamp = DateTime.Now;
            if (table.Columns.Any(c => c.Name == "ID_USER_VLOZIL"))
            {
                int userId = core.User.Id;
                Item["ID_USER_VLOZIL"] = userId;
                Item["ID_USER_EDITOVAL"] = userId;
                Item["DATUM_VLOZENI"] = timestamp;
                Item["DATUM_EDITACE"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date"))
            {
                Item["date"] = timestamp;
            }
            else if (table.Columns.Any(c => c.Name == "date_purchase"))
            {
                Item["date_purchase"] = timestamp;
            }

            return table.AddGetId(Item);
        }

        [Action(10036, "Select top X", "Result")]
        public static List<DBItem> SelectTopX(COREobject core, int NumOfItems, List<DBItem> TableData, string SortingColumn)
        {
            return TableData.OrderByDescending(c => c[SortingColumn]).Take(NumOfItems).ToList();
        }

        [Action(10147, "Dictionary To Jtoken", "Result")]
        public static JToken Dictionary2Jtoken(COREobject core, object Dictionary)
        {
            return JToken.FromObject(Dictionary);
        }

        [Action(10300, "Add to list", "Result")]
        public static List<object> AddToList(COREobject core, List<dynamic> List = null, object Value = null)
        {
            List = List ?? new List<object>();
            if (Value != null)
                List.Add(Value);

            return List;
        }

        [Action(10301, "Add to JArray", "Result")]
        public static JArray AddToJarray(COREobject core, object Object = null, JArray Array = null)
        {
            Array = Array ?? new JArray();
            if (Object != null)
            {
                Array.Add(Object);
            }

            return Array;
        }

        [Action(21399, "TableData Group Average", "TableData")]
        public static List<DBItem> TableDataAverages(COREobject core, List<DBItem> TableData, string DataColName, int Range = 100)
        {
            int dataCount = TableData.Count;
            int divisor = dataCount / Range;
            List<DBItem> group = new List<DBItem>();
            List<DBItem> result = new List<DBItem>();
            for (int i = 0; i < dataCount; i++)
            {
                group.Add(TableData[i]);

                if (i % divisor == 0)
                {
                    //calculate average
                    int sum = 0;
                    foreach (var dbi in group)
                    {
                        sum += Convert.ToInt32(dbi[DataColName].ToString());
                    }
                    TableData[i][DataColName] = sum / divisor;
                    result.Add(TableData[i]);
                    group.Clear();
                }
            }

            return result;
        }

        [Action(30104, "Table Column To CSV", "Result")]
        public static string TableColumnToCSV(COREobject core, List<DBItem> TableData, string LabelColumn, string[] ValueColumn)
        {
            if (!TableData.Any())
                return "";

            string[] columns = new string[] { LabelColumn }.Concat(ValueColumn).Where(c => c != null).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Join(",", columns));
            sb.Append("\\n");
            foreach(DBItem item in TableData)
            {
                sb.Append(string.Join(",", columns.Select(c => checkNull(item[c]).Replace(",", "."))));
                sb.Append("\\n");
            }
            
            return sb.ToString(); //retur comma seperated string  to result,which can be used by the graph later
        }
        private static string checkNull(object Input)
        {
            if (Input == DBNull.Value || Input == null)
                return "";
            else
                return Input.ToString();
        }

        [Action(1010121, "Mass Delete DBItem")]
        public static void MassDeleteDBItem(COREobject core, IEnumerable<int> ItemIDList, string TableName = null, bool SearchInShared = false)
        {
            DBConnection db = COREobject.i.Entitron;

            string tableName = TableName ?? core.BlockAttribute.ModelTableName;
            DBTable table = db.Table(tableName, SearchInShared);
            
            foreach (int id in ItemIDList)
            {
                table.Delete(id);
            }

            db.SaveChanges();
        }
    }
}
