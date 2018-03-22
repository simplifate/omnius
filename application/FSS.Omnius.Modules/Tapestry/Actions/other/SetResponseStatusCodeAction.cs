using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    public class SetResponseStatusCodeAction : Action
    {
        public override int Id => 1851212;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "StatusCode" };

        public override string Name => "Set response status code";

        public override string[] OutputVar => new string[] {  };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            int statusCode = (int)vars["StatusCode"];
            HttpContext.Current.Response.StatusCode = statusCode;
        }
    }
}
