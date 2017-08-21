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
    public class AddToListAction : Action
    {
        public override int Id
        {
            get
            {
                return 10300;
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
                return new string[] { "Value", "?List" };
            }
        }

        public override string Name
        {
            get
            {
                return "Add to list";
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
            
            var list = new List<object>();
            if (vars.ContainsKey("List"))
                list = (List<object>)vars["List"];

            object value = vars.ContainsKey("Value") ? vars["Value"] : null;
            if(value != null) {
                list.Add(value);
            }
            
            outputVars["Result"] = list;
        }
    }
}
