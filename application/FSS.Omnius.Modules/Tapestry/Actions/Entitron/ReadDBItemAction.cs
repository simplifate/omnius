using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [EntitronRepository]
    public class ReadDBItemAction : Action
    {
        public override int Id => 1022;

        public override int? ReverseActionId => null;

        public override string[] InputVar => new string[] { "TableName", "Id", "?SearchInShared" };

        public override string Name => "Select Item (by Id)";

        public override string[] OutputVar => new string[] { "Data" };

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            // init
            DBConnection db = COREobject.i.Entitron;

            int targetId = vars["Id"] is int ? (int)vars["Id"] : int.Parse((string)vars["Id"]);
            bool searchInShared = vars.ContainsKey("SearchInShared") ? (bool)vars["SearchInShared"] : false;

            outputVars["Data"] = db.Table((string)vars["TableName"], searchInShared).SelectById(targetId);
            if(outputVars["Data"] == null)
                throw new Exception($"Položka nebyla nalezena (Tabulka: {vars["TableName"]}, Id: {vars["Id"]}, Akce: {Name} ({Id}))");
        }
    }
}
