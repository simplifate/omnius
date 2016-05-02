using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Sql;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class GetDictionariesFromRowsAction : Action
    {
        public override int Id
        {
            get
            {
                return 1033;
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
                return new string[] { "InputData", "KeyMapping" };
            }
        }

        public override string Name
        {
            get
            {
                return "Get dictionaries from rows";
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
            var result = new List<Dictionary<string, string>>();
            if (vars["InputData"] is DBItem)
            {
                var jObject = (JObject)((DBItem)vars["InputData"]).ToJson();
                var rowDictionary = new Dictionary<string, string>();
                foreach (var pair in jObject)
                {
                    rowDictionary.Add(pair.Key, pair.Value.ToString());
                }
                result.Add(rowDictionary);
            }
            else if(vars["InputData"] is IEnumerable<DBItem>)
            {
                var inputData = (List<DBItem>)vars["InputData"];
                foreach(var inputRow in inputData)
                {
                    var jObject = (JObject)inputRow.ToJson();
                    var rowDictionary = new Dictionary<string, string>();
                    foreach (var pair in jObject)
                    {
                        rowDictionary.Add(pair.Key, pair.Value.ToString());
                    }
                    result.Add(rowDictionary);
                }
            }
            outputVars["Result"] = result;
        }
    }
}
