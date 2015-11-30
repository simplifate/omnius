using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    class CompareAction : Action
    {
        public override int Id
        {
            get
            {
                return 1001;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "model",
                    "parameter",
                    "value"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Compare";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "comparation"
                };
            }
        }

        public override ActionResultCollection run(Dictionary<string, object> vars)
        {
            var outputVars = new Dictionary<string, object>();
            var outputStatus = ActionResultType.Success;
            var outputMessage = "OK";

            try
            {
                DBItem model = (DBItem)vars["model"];
                string parameter = (string)vars["parameter"];
                object value = vars["value"];

                bool areSame = model[parameter] == value;
                outputVars.Add("comparation", areSame);
            }
            catch(Exception ex)
            {
                outputStatus = ActionResultType.Error;
                outputMessage = ex.Message;
            }

            return new ActionResultCollection(outputStatus, outputMessage, outputVars);
        }
    }
}
