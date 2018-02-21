using System;
using System.Collections.Generic;
using System.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AddToDictionaryAction : Action
    {
        public override int Id => 1030;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "RowData", "KeyMapping", "?Dictionary", "?AutoMapping" };

        public override string Name => "Add to dictionary";

        public override string[] OutputVar => new string[] { "Result" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            var dictionary = new Dictionary<string, object>();
            if (vars.ContainsKey("Dictionary"))
                dictionary = (Dictionary<string, object>)vars["Dictionary"];
            
            DBItem rowData = null;
            bool useRowData = true;
            bool skipMapping = false;
            if (!vars.ContainsKey("RowData"))
                useRowData = false;
            else if (vars["RowData"] is string)
                rowData = (DBItem)vars[(string)vars["RowData"]];
            else if (vars["RowData"] is DBItem)
                rowData = (DBItem)vars["RowData"];
            else if (vars["RowData"] is IEnumerable<DBItem>)
                rowData = (vars["RowData"] as IEnumerable<DBItem>).FirstOrDefault();

            var autoMapping = (vars.ContainsKey("AutoMapping") && Convert.ToBoolean(vars["AutoMapping"]));

            List <string> mappingStringList = null;
            if (autoMapping)
            {
                mappingStringList = rowData.getColumnNames().Select(a => $"{a}:{a}").ToList();
            }
            else if (!vars.ContainsKey("KeyMapping"))
            {
                skipMapping = true;
            }
            else
            { 
                mappingStringList = ((string)vars["KeyMapping"]).Split(',').ToList();
            }

            if (!skipMapping)
            {
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
                        value = KeyValueString.ParseValue(columnName, vars);
                    if (dictionary.ContainsKey(tokens[0]))
                    {
                        if (value == null)
                            dictionary[tokens[0]] = "";
                        else
                            dictionary[tokens[0]] = value;
                    }
                    else
                    {
                        if (value == null)
                            dictionary.Add(tokens[0], "");
                        else
                            dictionary.Add(tokens[0], value);
                    }
                }
            }
            outputVars["Result"] = dictionary;
        }
    }
}
