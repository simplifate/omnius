using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.PIA
{
    public class LogChangesAction : Action
    {
        public override int Id
        {
            get {
                return 6001;
            }
        }

        public override string[] InputVar
        {
            get {
                return new string[] { "TableName", "ModelId", "OldData", "?IgnoreColumns" };
            }
        }

        public override string[] OutputVar
        {
            get {
                return new string[] { };
            }
        }

        public override string Name
        {
            get {
                return "PIA: Log changes";
            }
        }

        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string tableName = (string)vars["TableName"];
            int modelId = (int)vars["ModelId"];
            DBItem oldData = (DBItem)vars["OldData"];
            List<string> ignoreColumns = vars.ContainsKey("IgnoreColumns") ? ((string)vars["IgnoreColumns"]).Split(',').ToList() : new List<string>();
        
            List<string> changes = new List<string>();

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron entitron = core.Entitron;

            DBTable table = entitron.GetDynamicTable(tableName);
            if (table == null) {
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");
            }

            var columnMetadataList = core.Entitron.Application.ColumnMetadata.Where(c => c.TableName == tableName);

            foreach (DBColumn column in table.columns) 
            {
                if (ignoreColumns.Contains(column.Name) || column.Name == "id")
                    continue;

                var oldValue = oldData[column.Name];
                var newValue = GetNewValue(vars, tableName, column);
                var columnMetadata = columnMetadataList.FirstOrDefault(c => c.ColumnName == column.Name);

                bool isEqual = true;
                if (newValue != null && newValue != DBNull.Value && oldValue != DBNull.Value) {
                    switch (column.type) {
                        case "nvarchar": isEqual = (string)oldValue == (string)newValue; break;
                        case "boolean": isEqual = (bool)oldValue == (bool)newValue; break;
                        case "integer": isEqual = (int)oldValue == (int)newValue; break;
                        case "float": isEqual = (float)oldValue == (float)newValue; break;
                        case "datetime": isEqual = (DateTime?)oldValue == (DateTime?)newValue; break;
                    }
                }
                else {
                    if(newValue == DBNull.Value) {
                        isEqual = true;
                    }
                    else if(!string.IsNullOrEmpty((string)Convertor.convert('s', newValue))) {
                        if(oldValue == DBNull.Value) {
                            isEqual = false;
                        }
                    }
                    else {
                        if(oldValue != DBNull.Value) {
                            isEqual = false;
                        }
                    }
                }

                if(!isEqual) {
                    changes.Add(string.Format("{0}: změna na \"{1}\"",
                            columnMetadata.ColumnDisplayName ?? columnMetadata.ColumnName,
                            Convertor.StripTags((string)Convertor.convert('s', newValue))
                        )
                    );
                }
            }

            if(changes.Count > 0) {
                DBItem entry = new DBItem();
                entry.createProperty(1, "MODEL_NAME", tableName);
                entry.createProperty(2, "MODEL_ID", modelId);
                entry.createProperty(3, "DATUM_ZMENY", DateTime.Now);
                entry.createProperty(4, "USER_ID", core.User.Id);
                entry.createProperty(5, "CHANGES", string.Join("<br>", changes));

                DBTable log = entitron.GetDynamicTable("RWE_CHANGE_LOG");
                log.Add(entry);

                entitron.Application.SaveChanges();
            }
        }

        private object GetNewValue(Dictionary<string, object> vars, string tableName, DBColumn column)
        {
            if (column.type == "bit") {
                return vars.ContainsKey($"__Model.{tableName}.{column.Name}");
            }
            else if (vars.ContainsKey($"__Model.{tableName}.{column.Name}")) {
                var val = vars[$"__Model.{tableName}.{column.Name}"];
                if (column.type == "datetime" && val != null) {
                    return Convert.ToDateTime(val);
                }
                else {
                    return val;
                }
            }
            else {
                return DBNull.Value;
            }
        }
    }
}
