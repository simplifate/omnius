using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    public class GenerateRefreshJsonAction : Action
    {
        public override int Id => 300122;

        public override string[] InputVar => new string[]
        {
            "rigsOk_count",
            "rigsOk_ghs", "rigsWarning_ghs",
            "rigsOK_ghs_XMR", "rigsWarning_ghs_XMR",
            "rigsWarning_count", "rigsDown_count",
            "rigsWarning_List",
            "rigsStatusHistory",
            "expectReality", "predictedIncome",
            "consum_rigs","consum_aircondition","consum_history",
            "day_earnings","day_profit","day_cost_service","day_cost_elect",
            "week_earnings","week_profit","week_cost_service","week_cost_elect",
            "month_earnings","month_profit","month_cost_service","month_cost_elect",
            "transaction_history_table",
            "totalBalance",
            "btc_value","btc_change","btc_change_percent",
            "eth_value","eth_change","eth_change_percent",
            "zec_value","zec_change","zec_change_percent",
            "dcr_value","dcr_change","dcr_change_percent",
            "ltc_value","ltc_change","ltc_change_percent",
            "xmr_value","xmr_change","xmr_change_percent",
            "music_value","music_change","music_change_percent"
        };

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override string Name => "Grid: Generate refresh JSON";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            JObject result = new JObject();
            /// Miner status
            // rigs ok
            var rigsOk = new JObject();
            rigsOk.Add("y", new JValue(vars["rigsOk_count"]==null ? 0 : (int)vars["rigsOk_count"]));
            rigsOk.Add("x", new JValue(vars["rigsOk_ghs"] == null ? 0 : (double)vars["rigsOk_ghs"]));
            rigsOk.Add("x2", new JValue(vars["rigsOK_ghs_XMR"] == null ? 0 : (double)vars["rigsOK_ghs_XMR"]));
            // rigs warning
            var rigsWarning = new JObject();
            rigsWarning.Add("y", new JValue(vars["rigsWarning_count"] == null ? 0 : (int)vars["rigsWarning_count"]));
            rigsWarning.Add("x", new JValue(vars["rigsWarning_ghs"] == null ? 0 : (double)vars["rigsWarning_ghs"]));
            rigsWarning.Add("x2", new JValue(vars["rigsWarning_ghs_XMR"] == null ? 0 : (double)vars["rigsWarning_ghs_XMR"]));
            // rigs down
            var rigsDown = new JObject();
            rigsDown.Add("y", new JValue(vars["rigsDown_count"] == null ? 0 : (int)vars["rigsDown_count"]));
            rigsDown.Add("x", new JValue(0));
            //
            result.Add("MinerStatus", new JArray(rigsOk, rigsWarning, rigsDown));

            /// Rigs warning method
            result.Add("RigsWarningMessages", (JArray)vars["rigsWarning_List"]);

            /// RigStatusHistory
            DBItem rigStatusHistory_item = (DBItem)vars["rigsStatusHistory"];
            var rigStatusHistory = new JObject();
            rigStatusHistory.Add("id", new JValue (vars.ContainsKey("dateTime") ? (string)rigStatusHistory_item["dateTime"] : ""));
            rigStatusHistory.Add("value", new JArray(vars.ContainsKey("Ok") ? (int)rigStatusHistory_item["Ok"] : 0, vars.ContainsKey("RoundedPerformance") ? (double)rigStatusHistory_item["RoundedPerformance"] : 0));
            result.Add("RigStatusHistory", rigStatusHistory);

            /// ExpVsReal
            DBItem ExpVsReal_item = (DBItem)vars["expectReality"];
            var ExpVsReal = new JObject();
            ExpVsReal.Add("id", (string)ExpVsReal_item["Date"]);
            ExpVsReal.Add("value", new JArray(ExpVsReal_item["Hourly income"], vars["predictedIncome"]));
            result.Add("ExpVsReal", ExpVsReal);

            /// Consum
            var Consum = new JObject();
            Consum.Add("rigs", new JValue(vars["consum_rigs"]));
            Consum.Add("air", new JValue(vars["consum_aircondition"]));
            if (vars["consum_history"] != null)
            {
                DBItem Consum_item = (DBItem)vars["consum_history"];
                var ConsumHistory = new JObject();
                ConsumHistory.Add("id", new JValue(Consum_item["TimeStamp2"]));
                ConsumHistory.Add("value", new JArray(Consum_item["Prikon_datCentrum"], Consum_item["Prikon_chlazeni"]));
                Consum.Add("history", ConsumHistory);
                result.Add("Consum", Consum);
            }

            /// Earnings
            var Earnings = new JObject();
            Earnings.Add("day", new JArray(vars["day_earnings"], vars["day_profit"], vars["day_cost_service"], vars["day_cost_elect"]));
            Earnings.Add("week", new JArray(vars["week_earnings"], vars["week_profit"], vars["week_cost_service"], vars["week_cost_elect"]));
            Earnings.Add("month", new JArray(vars["month_earnings"], vars["month_profit"], vars["month_cost_service"], vars["month_cost_elect"]));
            result.Add("Earnings", Earnings);

            /// LastTransaction
            if (vars["transaction_history_table"] != null)
            {
                var LastTransaction = new JArray();
                foreach (DBItem item in (ListJson<DBItem>)vars["transaction_history_table"])
                {
                    var jItem = new JObject();
                    jItem.Add("hidden_id", new JValue(-1));
                    jItem.Add("hidden_add", new JValue(item["hidden__add"]));
                    jItem.Add("hidden_date", new JValue(item["hidden__date"]));
                    jItem.Add("currency", new JValue(item["Currency"]));
                    jItem.Add("amount", new JValue(item["Amount"]));
                    jItem.Add("wallet", new JValue(item["Wallet"]));
                    jItem.Add("date", new JValue(item["Date"]));
                    jItem.Add("time", new JValue(item["Time"]));
                    LastTransaction.Add(jItem);
                }
                result.Add("LastTransaction", LastTransaction);
            }

            /// TotalIncome
            if (vars["totalBalance"] != null)
            {
                DBItem TotalIncome_item = (DBItem)vars["totalBalance"];
                var TotalIncome = new JObject();
                TotalIncome.Add("id", new JValue(TotalIncome_item["Date"]));
                TotalIncome.Add("value", new JValue(TotalIncome_item["Total_in_usd"]));
                result.Add("TotalIncome", TotalIncome);
            }
       
            /// ExchangeRates
            var ExchangeRates = new JObject();

            if (vars["btc_value"] != null)
            {
                var btc = new JObject();
                btc.Add("value", new JValue(vars["btc_value"]));
                btc.Add("change", new JValue(vars["btc_change"]));
                btc.Add("percent", new JValue(vars["btc_change_percent"]));
                ExchangeRates.Add("btc", btc);
            }

            if (vars["eth_value"] != null)
            {
                var eth = new JObject();
                eth.Add("value", new JValue(vars["eth_value"]));
                eth.Add("change", new JValue(vars["eth_change"]));
                eth.Add("percent", new JValue(vars["eth_change_percent"]));
                ExchangeRates.Add("eth", eth);
            }
            
            if (vars["zec_value"] != null)
            {
                var zec = new JObject();
                zec.Add("value", new JValue(vars["zec_value"]));
                zec.Add("change", new JValue(vars["zec_change"]));
                zec.Add("percent", new JValue(vars["zec_change_percent"]));
                ExchangeRates.Add("zec", zec);
            }
            
            if (vars["dcr_value"] != null)
            {
                var dcr = new JObject();
                dcr.Add("value", new JValue(vars["dcr_value"]));
                dcr.Add("change", new JValue(vars["dcr_change"]));
                dcr.Add("percent", new JValue(vars["dcr_change_percent"]));
                ExchangeRates.Add("dcr", dcr);
            }

            if (vars["ltc_value"] != null)
            {
                var ltc = new JObject();
                ltc.Add("value", new JValue(vars["ltc_value"]));
                ltc.Add("change", new JValue(vars["ltc_change"]));
                ltc.Add("percent", new JValue(vars["ltc_change_percent"]));
                ExchangeRates.Add("ltc", ltc);
            }

            if (vars["xmr_value"] != null)
            {
                var xmr = new JObject();
                xmr.Add("value", new JValue(vars["xmr_value"]));
                xmr.Add("change", new JValue(vars["xmr_change"]));
                xmr.Add("percent", new JValue(vars["xmr_change_percent"]));
                ExchangeRates.Add("xmr", xmr);
            }

            if (vars["music_value"] != null)
            {
                var music = new JObject();
                music.Add("value", new JValue(vars["music_value"]));
                music.Add("change", new JValue(vars["music_change"]));
                music.Add("percent", new JValue(vars["music_change_percent"]));
                ExchangeRates.Add("music", music);
                result.Add("ExchangeRates", ExchangeRates);
            }

            /// 
            outputVars["Result"] = result;
        }
    }
}
