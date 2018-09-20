using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ParseExchangeRates : Action
    {
        public override int Id => 1984;

        public override string[] InputVar => new string[] { "JToken" };

        public override string Name => "Parse Exchange Rates";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = COREobject.i.Entitron;
            DBTable table = db.Table("exchange_rates");

            var input = (JObject)vars["JToken"];
            JObject data = (JObject)input["result"];

            foreach (var row in data)
            {
                string exchange;
                string pair;
                float value = ((JValue)row.Value).ToObject<float>();

                string key = row.Key;

                string[] splitString = key.Split(':');

                exchange = splitString[0];
                pair = splitString[1];

                DBItem item = new DBItem(db, table);
                item["exchange"] = exchange;
                item["pair"] = pair;
                item["value"] = value;

                table.Add(item);
            }
            db.SaveChanges();
        }
    }
}
