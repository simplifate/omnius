using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.PIA
{
    public class LogChangesAction : Action
    {
        public override int Id => 6001;

        public override string[] InputVar => new string[] { "TableName", "ModelId", "OldData", "?IgnoreColumns" };

        public override string[] OutputVar => new string[] { };

        public override string Name => "PIA: Log changes";

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            string tableName = (string)vars["TableName"];
            int modelId = (int)vars["ModelId"];
            DBItem oldData = (DBItem)vars["OldData"];
            List<string> ignoreColumns = vars.ContainsKey("IgnoreColumns") ? ((string)vars["IgnoreColumns"]).Split(',').ToList() : new List<string>();

            List<string> changes = new List<string>();

            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron entitron = core.Entitron;

            DBTable table = db.Table(tableName);
            if (table == null)
            {
                throw new Exception($"Požadovaná tabulka nebyla nalezena (Tabulka: {tableName}, Akce: {Name} ({Id}))");
            }

            foreach (DBColumn column in table.Columns)
            {
                if (ignoreColumns.Contains(column.Name) || column.Name == "id")
                    continue;

                var oldValue = oldData[column.Name];
                var newValue = GetNewValue(vars, tableName, column);
                var columnMetadata = column.Metadata;

                bool isEqual = true;
                if (newValue != null && newValue != DBNull.Value && oldValue != DBNull.Value)
                {
                    switch (column.Type)
                    {
                        case DbType.String: isEqual = (string)oldValue == (string)newValue; break;
                        case DbType.Boolean: isEqual = (bool)oldValue == (bool)newValue; break;
                        case DbType.Int32: isEqual = (int)oldValue == (int)newValue; break;
                        case DbType.Double: isEqual = (float)oldValue == (float)newValue; break;
                        case DbType.DateTime: isEqual = (DateTime?)oldValue == (DateTime?)newValue; break;
                    }
                }
                else
                {
                    if (newValue == DBNull.Value)
                    {
                        isEqual = true;
                    }
                    else if (!string.IsNullOrEmpty((string)DataType.ConvertTo(DbType.String, newValue)))
                    {
                        if (oldValue == DBNull.Value)
                        {
                            isEqual = false;
                        }
                    }
                    else
                    {
                        if (oldValue != DBNull.Value)
                        {
                            isEqual = false;
                        }
                    }
                }

                if (!isEqual)
                {
                    changes.Add(string.Format("{0}: změna na \"{1}\"",
                            columnMetadata.ColumnDisplayName ?? columnMetadata.ColumnName,
                            DataType.StripTags((string)DataType.ConvertTo(DbType.String, newValue))
                        )
                    );
                }
            }

            if (changes.Count > 0)
            {
                DBTable log = db.Table("RWE_CHANGE_LOG");
                DBItem entry = new DBItem(db, log);
                entry["MODEL_NAME"] = tableName;
                entry["MODEL_ID"] = modelId;
                entry["DATUM_ZMENY"] = DateTime.Now;
                entry["USER_ID"] = core.User.Id;
                entry["CHANGES"] = string.Join("<br>", changes);

                log.Add(entry);
            }

            db.SaveChanges();
        }

        private object GetNewValue(Dictionary<string, object> vars, string tableName, DBColumn column)
        {
            if (column.Type == DbType.Boolean)
            {
                return vars.ContainsKey($"__Model.{tableName}.{column.Name}");
            }
            else if (vars.ContainsKey($"__Model.{tableName}.{column.Name}"))
            {
                var val = vars[$"__Model.{tableName}.{column.Name}"];
                if (column.Type == DbType.DateTime && val != null)
                {
                    return Convert.ToDateTime(val);
                }
                else
                {
                    return val;
                }
            }
            else
            {
                return DBNull.Value;
            }
        }
    }
}
