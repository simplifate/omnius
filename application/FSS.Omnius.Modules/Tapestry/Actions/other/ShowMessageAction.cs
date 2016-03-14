using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    public class ShowMessageAction : Action
    {
        public override int Id
        {
            get
            {
                return 182;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Message", "Type" };
            }
        }

        public override string Name
        {
            get
            {
                return "Show message";
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
            switch ((string)vars["Type"])
            {
                case "Success":
                    message.Success.Add((string)vars["Message"]);
                    break;
                case "Warning":
                    message.Warnings.Add((string)vars["Message"]);
                    break;
                case "Error":
                    message.Errors.Add((string)vars["Message"]);
                    break;
                default:
                    message.Info.Add((string)vars["Message"]);
                    break;
            }
        }
    }
}
