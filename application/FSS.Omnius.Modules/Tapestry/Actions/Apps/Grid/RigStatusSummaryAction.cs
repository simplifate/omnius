using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class RigStatusSummaryAction : Action
    {

        public override int Id
        {
            get
            {
                return 3120712;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "InputJSON","?Errors"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Rig status summary";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] {"Result","Errors" };
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
            DBConnection db = COREobject.i.Entitron;
            //get jarray elements
            JArray miners = (JArray)vars["InputJSON"];
            JArray errors = vars.ContainsKey("Errors") ? (JArray)vars["Errors"] : new JArray();

            //get rows from view and map to list
            var viewData = db.Select("ViewRigsGpusCount", false).ToList();
            foreach (JObject rig in miners)
            {
                //get rigItem from viewData with same ip in the JSON input
                var rigItem = viewData.SingleOrDefault(x => x["Rig_name"].ToString() == rig["name"].ToObject<string>());
                if (rigItem != null && rig["status"].ToObject<string>() == "up")
                {
                    var gpuCount = (int)(rigItem["gpus"]); //get rigs gpu count
                    if (gpuCount > rig["gpu_temp"].Count()) //if gpu count in view is higher than real gpu count, set status to warning
                    {
                        var thisRig = rig["name"].ToObject<string>();
                        errors.Add("Warning: GPU has a faulty rizer " + thisRig);
                        rig["status"] = "warning";
                    }
                }
            } //end loop
            
            outputVars["Result"] = miners;
            outputVars["Errors"] = errors;

        }
    }
}
