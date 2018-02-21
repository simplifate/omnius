using System.Collections.Generic;
using System.Web;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Mozaic
{
    [MozaicRepository]
    public class SaveFormStateAction : Action
    {
        public override int Id => 2008;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { };

        public override string Name => "Save form state";

        public override string[] OutputVar => new string[] { };

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
