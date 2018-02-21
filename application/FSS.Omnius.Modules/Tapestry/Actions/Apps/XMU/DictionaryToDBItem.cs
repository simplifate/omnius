using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class DictionaryToDBItemAction : Action
    {
        public override int Id => 19845;

        public override string[] InputVar => new string[] { "Dictionary" };

        public override string Name => "Dictionary to DBItem action";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            Dictionary<string, object> sourceDictionary = (Dictionary<string, object>)vars["Dictionary"];

            var item = new DBItem(Modules.Entitron.Entitron.i, null, sourceDictionary);

            outputVars["Result"] = item;

        }
    }
}
