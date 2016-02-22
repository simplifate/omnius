using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    class SendMailAction : Action
    {
        public override int Id
        {
            get {
                return 2004;
            }
        }
        public override int? ReverseActionId
        {
            get {
                return null;
            }
        }
        public override string[] InputVar
        {
            get {
                return new string[] { "Recipients", "CC", "BCC", "Subject", "Template" };
            }
        }
        public override string[] OutputVar
        {
            get {
                return new string[] { "Result", "ErrorMessage" };
            }
        }
        public override string Name
        {
            get {
                return "Send mail";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars)
        {
            // Init
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];








        }
    }
}
