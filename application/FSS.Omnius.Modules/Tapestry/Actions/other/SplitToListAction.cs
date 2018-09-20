using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class SplitToListAction : Action
    {
        public override int Id
        {
            get
            {
                return 5001;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] {"s$SourceString", "?s$Separator"};
            }
        }

        public override string Name
        {
            get
            {
                return "SplitToListAction";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
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
            char Separator = ';';
            if (vars.ContainsKey("Separator"))
            {
                Separator = vars["Separator"].ToString()[0];
            }
            outputVars["Result"] = ((string)vars["SourceString"]).Split(Separator).ToList();
        }
    }
}


