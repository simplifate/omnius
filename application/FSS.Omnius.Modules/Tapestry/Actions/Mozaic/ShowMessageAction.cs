using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    public class ShowMessageAction : Action
    {
        public override int Id => 182;

        public override string[] InputVar => new string[] { "s$Message", "s$Type[Success|Warning|Error|Info]" };

        public override string Name => "Show message";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

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
