using System.Collections.Generic;
using System.Web;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RedirectToUrlAction : Action
    {
        public override int Id => 219;
        public override string[] InputVar => new string[] { "URL" };
        public override string Name => "Redirect to URL";
        public override string[] OutputVar => new string[0];
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            string url = (string)vars["URL"];
            response.Redirect(url);
        }
    }
}
