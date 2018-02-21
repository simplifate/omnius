using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class SubstringAction : Action
    {
        public override int Id => 202;
        public override string[] InputVar => new string[] { "InputString", "Index", "Length" };
        public override string Name => "Substring";
        public override string[] OutputVar => new string[] { "Result" };
        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string input = (string)vars["InputString"];
            int index = (int)vars["Index"];
            int length = (int)vars["Length"];

            outputVars["Result"] = input.Substring(index, length);
        }
    }
}
