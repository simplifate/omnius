using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class CompareAction : Action
    {
        public override int Id
        {
            get
            {
                return 1001;
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
        
        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> invertedVars, Message message)
        {
            DBItem model = (DBItem)vars["model"];
            string parameter = (string)vars["parameter"];
            object value = vars["value"];

            bool areSame = model[parameter] == value;
            outputVars.Add("comparation", areSame);
        }
    }
}
