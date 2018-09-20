using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class JArrayJoinAction : Action
    {

        public override int Id
        {
            get
            {
                return 300011;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "InputArray1","InputArray2"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Join JArray";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
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
            if (vars.ContainsKey("InputArray1") && vars.ContainsKey("InputArray2")) {
                JArray array1 = (JArray)vars["InputArray1"] == null? new JArray() : (JArray)vars["InputArray1"];
                JArray array2 = (JArray)vars["InputArray2"] == null ? new JArray() : (JArray)vars["InputArray2"];

                JArray array3 = new JArray(array1.Union(array2));

                outputVars["Result"] = array3;
            }
            else
            {
                JArray array3 = new JArray();

                outputVars["Result"] = array3;
            }
        }
    }
}
