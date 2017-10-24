using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class RedirectToUrlAction : Action
    {
        public override int Id
        {
            get
            {
                return 219;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "URL" };
            }
        }

        public override string Name
        {
            get
            {
                return "Redirect to URL";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
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
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;

            string url = (string)vars["URL"];
            response.Redirect(url);
        }
    }
}
