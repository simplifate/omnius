using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Watchtower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class SaveFormStateAction : Action
    {
        public override int Id
        {
            get
            {
                return 2008;
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
                return new string[] { };
            }
        }

        public override string Name
        {
            get
            {
                return "Save form state";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            Dictionary<string, string> state = new Dictionary<string, string>();
            foreach(KeyValuePair<string, object> kv in vars) {
                state.Add(kv.Key, kv.Value.ToString());
            }

            HttpContext.Current.Session["FormState"] = state;
        }
    }
}
