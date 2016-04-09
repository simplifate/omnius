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
    public class GetDictionaryFromTableAction : Action
    {
        public override int Id
        {
            get
            {
                return 1028;
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
                return new string[] { "TableData", "KeyColumn", "ValueColumn" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get dictionary from table";
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
            var result = new Dictionary<string, string>();
            foreach (var row in (List<DBItem>)vars["TableData"])
            {
                string key = (string)row[(string)vars["KeyColumn"]];
                string value = (string)row[(string)vars["ValueColumn"]];
                result.Add(key, value);
            }
            outputVars["Result"] = result;
        }
    }
}
