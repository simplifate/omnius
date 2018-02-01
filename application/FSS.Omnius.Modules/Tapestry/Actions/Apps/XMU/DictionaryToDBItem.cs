using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class DictionaryToDBItemAction : Action
    {
        public override int Id
        {
            get
            {
                return 19845;
            }
        }
        
        public override string[] InputVar
        {
            get
            {
                return new string[] { "Dictionary" };
            }
        }

        public override string Name
        {
            get
            {
                return "Dictionary to DBItem action";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Dictionary<string, object> sourceDictionary = (Dictionary<string, object>)vars["Dictionary"];

            var item = new DBItem();
            int index = 0;

            foreach (var kv in sourceDictionary)
            {
                item.createProperty(index+=1, kv.Key,kv.Value);
            }

            outputVars["Result"] = item;

        }
    }
}
