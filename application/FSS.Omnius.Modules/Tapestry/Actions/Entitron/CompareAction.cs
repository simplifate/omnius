using FSS.Omnius.Entitron;
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

        public override ActionResult run(Dictionary<string, object> vars)
        {
            ActionResult ar = new ActionResult();
            try
            {
                DBItem model = (DBItem)vars["model"];
                string parameter = (string)vars["parameter"];
                object value = vars["value"];

                bool areSame = model[parameter] == value;
                ar.outputData = new Dictionary<string, object>() { { "comparation", areSame } };
            }
            catch(Exception ex)
            {
                ar.type = ActionResultType.Error;
                ar.Message = ex.Message;
            }

            return ar;
        }
    }
}
