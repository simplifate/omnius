using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class AddToDictionaryAction : Action
    {
        public override int Id
        {
            get
            {
                return 1030;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "RowData", "KeyMapping", "?Dictionary" };
            }
        }

        public override string Name
        {
            get
            {
                return "Add to dictionary";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            if (core.Entitron.Application == null)
                core.Entitron.AppName = "EvidencePeriodik";

            var dictionary = new Dictionary<string, string>();
            if (vars.ContainsKey("Dictionary"))
                dictionary = (Dictionary<string, string>)vars["Dictionary"];
            var mappingStringList = ((string)vars["KeyMapping"]).Split(',').ToList();
            DBItem rowData = new DBItem();
            bool useRowData = true;
            if (!vars.ContainsKey("RowData"))
                useRowData = false;
            else if (vars["RowData"] is string)
                rowData = (DBItem)vars[(string)vars["RowData"]];
            else
                rowData = (DBItem)vars["RowData"];
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
                    value = vars[columnName];
                dictionary.Add(tokens[0], value == null ? "" : value.ToString());
            }
            outputVars["Result"] = dictionary;
        }
    }
}
