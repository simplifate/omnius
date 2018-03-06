using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GetFileExtensionAction : Action
    {
        public override int Id => 9119;

        public override string[] InputVar => new string[] { "s$FileName" };

        public override string Name => "Get File Extension";

        public override string[] OutputVar => new string[] { "Extension" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string file = (string)vars["FileName"];
            outputVars["Extension"] = file.Split('.')[1];
        }
    }
}
