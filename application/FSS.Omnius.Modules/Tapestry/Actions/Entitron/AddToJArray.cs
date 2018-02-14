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
    public class AddToJarrayAction : Action
    {
        public override int Id
        {
            get
            {
                return 10301;
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
                return new string[] { "Object", "?Array" };
            }
        }

        public override string Name
        {
            get
            {
                return "Add to JArray";
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
            var Array = new JArray();
            if (vars.ContainsKey("Array"))
                Array = (JArray)vars["Array"];

            object Obj = vars.ContainsKey("Object") ? vars["Object"] : null;
            if (Obj != null)
            {
                Array.Add(Obj);
            }

            outputVars["Result"] = Array;
        }
    }
}
