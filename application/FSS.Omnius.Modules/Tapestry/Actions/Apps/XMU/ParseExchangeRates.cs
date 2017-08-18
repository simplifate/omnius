using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class ParseExchangeRates : Action
    {
        public override int Id
        {
            get
            {
                return 1984;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "JToken" };
            }
        }

        public override string Name
        {
            get
            {
                return "Parse Exchange Rates";
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            string tableName = "exchange_rates";
            DBTable table = core.Entitron.GetDynamicTable(tableName);

            var input = (JObject)vars["JToken"];
            JObject data = (JObject)input["result"];

            foreach(var row in data)
            {
                string exchange;
                string pair;
                float value = ((JValue)row.Value).ToObject<float>();

                string key = row.Key;

                string[] splitString = key.Split(':');

                exchange = splitString[0];
                pair = splitString[1];

                DBItem item = new DBItem();
                item.createProperty(1, "exchange", exchange);
                item.createProperty(2, "pair", pair);
                item.createProperty(3, "value", value);

                table.Add(item);
            }
            core.Entitron.Application.SaveChanges();


        }
    }
}
