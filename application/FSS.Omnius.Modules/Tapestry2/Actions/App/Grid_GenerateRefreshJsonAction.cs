using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(300122, "Grid: Generate refresh JSON", "Result")]
        public static JObject GenerateRefreshJson(COREobject core,
            JArray rigsWarning_List,
            DBItem rigsStatusHistory,
            DBItem expectReality, object predictedIncome,
            double day_earnings, double day_profit, double day_cost_service, double day_cost_elect,
            double week_earnings, double week_profit, double week_cost_service, double week_cost_elect,
            double month_earnings, double month_profit, double month_cost_service, double month_cost_elect,
            DBItem totalBalance,
            object consum_rigs, object consum_aircondition,
            int rigsOk_count = 0,
            double rigsOk_ghs = 0, double rigsWarning_ghs = 0,
            double rigsOK_ghs_XMR = 0, double rigsWarning_ghs_XMR = 0,
            int rigsWarning_count = 0, int rigsDown_count = 0,
            string dateTime = "",
            int Ok = 0,
            double RoundedPerformance = 0, DBItem consum_history = null,
            ListJson<DBItem> transaction_history_table = null,
            object btc_value = null, object btc_change = null, object btc_change_percent = null,
            object eth_value = null, object eth_change = null, object eth_change_percent = null,
            object zec_value = null, object zec_change = null, object zec_change_percent = null,
            object dcr_value = null, object dcr_change = null, object dcr_change_percent = null,
            object ltc_value = null, object ltc_change = null, object ltc_change_percent = null,
            object xmr_value = null, object xmr_change = null, object xmr_change_percent = null,
            object music_value = null, object music_change = null, object music_change_percent = null)
        {

            JObject result = new JObject();
            /// Miner status
            // rigs ok
            var rigsOk = new JObject();
            rigsOk.Add("y", new JValue(rigsOk_count));
            rigsOk.Add("x", new JValue(rigsOk_ghs));
            rigsOk.Add("x2", new JValue(rigsOK_ghs_XMR));
            // rigs warning
            var rigsWarning = new JObject();
            rigsWarning.Add("y", new JValue(rigsWarning_count));
            rigsWarning.Add("x", new JValue(rigsWarning_ghs));
            rigsWarning.Add("x2", new JValue(rigsWarning_ghs_XMR));
            // rigs down
            var rigsDown = new JObject();
            rigsDown.Add("y", new JValue(rigsDown_count));
            rigsDown.Add("x", new JValue(0));
            //
            result.Add("MinerStatus", new JArray(rigsOk, rigsWarning, rigsDown));

            /// Rigs warning method
            result.Add("RigsWarningMessages", rigsWarning_List);

            /// RigStatusHistory
            DBItem rigStatusHistory_item = rigsStatusHistory;
            var rigStatusHistory = new JObject();
            rigStatusHistory.Add("id", new JValue (dateTime));
            rigStatusHistory.Add("value", new JArray(Ok, RoundedPerformance));
            result.Add("RigStatusHistory", rigStatusHistory);

            /// ExpVsReal
            DBItem ExpVsReal_item = expectReality;
            var ExpVsReal = new JObject();
            ExpVsReal.Add("id", (string)ExpVsReal_item["Date"]);
            ExpVsReal.Add("value", new JArray(ExpVsReal_item["Hourly income"], predictedIncome));
            result.Add("ExpVsReal", ExpVsReal);

            /// Consum
            var Consum = new JObject();
            Consum.Add("rigs", new JValue(consum_rigs));
            Consum.Add("air", new JValue(consum_aircondition));
            if (consum_history != null)
            {
                var ConsumHistory = new JObject();
                ConsumHistory.Add("id", new JValue(consum_history["TimeStamp2"]));
                ConsumHistory.Add("value", new JArray(consum_history["Prikon_datCentrum"], consum_history["Prikon_chlazeni"]));
                Consum.Add("history", ConsumHistory);
                result.Add("Consum", Consum);
            }

            /// Earnings
            var Earnings = new JObject();
            Earnings.Add("day", new JArray(day_earnings, day_profit, day_cost_service, day_cost_elect));
            Earnings.Add("week", new JArray(week_earnings, week_profit, week_cost_service, week_cost_elect));
            Earnings.Add("month", new JArray(month_earnings, month_profit, month_cost_service, month_cost_elect));
            result.Add("Earnings", Earnings);

            /// LastTransaction
            if (transaction_history_table != null)
            {
                var LastTransaction = new JArray();
                foreach (DBItem item in transaction_history_table)
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
            if (totalBalance != null)
            {
                var TotalIncome = new JObject();
                TotalIncome.Add("id", new JValue(totalBalance["Date"]));
                TotalIncome.Add("value", new JValue(totalBalance["Total_in_usd"]));
                result.Add("TotalIncome", TotalIncome);
            }
       
            /// ExchangeRates
            var ExchangeRates = new JObject();

            if (btc_value != null)
            {
                var btc = new JObject();
                btc.Add("value", new JValue(btc_value));
                btc.Add("change", new JValue(btc_change));
                btc.Add("percent", new JValue(btc_change_percent));
                ExchangeRates.Add("btc", btc);
            }

            if (eth_value != null)
            {
                var eth = new JObject();
                eth.Add("value", new JValue(eth_value));
                eth.Add("change", new JValue(eth_change));
                eth.Add("percent", new JValue(eth_change_percent));
                ExchangeRates.Add("eth", eth);
            }

            if (zec_value != null)
            {
                var zec = new JObject();
                zec.Add("value", new JValue(zec_value));
                zec.Add("change", new JValue(zec_change));
                zec.Add("percent", new JValue(zec_change_percent));
                ExchangeRates.Add("zec", zec);
            }

            if (dcr_value != null)
            {
                var dcr = new JObject();
                dcr.Add("value", new JValue(dcr_value));
                dcr.Add("change", new JValue(dcr_change));
                dcr.Add("percent", new JValue(dcr_change_percent));
                ExchangeRates.Add("dcr", dcr);
            }

            if (ltc_value != null)
            {
                var ltc = new JObject();
                ltc.Add("value", new JValue(ltc_value));
                ltc.Add("change", new JValue(ltc_change));
                ltc.Add("percent", new JValue(ltc_change_percent));
                ExchangeRates.Add("ltc", ltc);
            }

            if (xmr_value != null)
            {
                var xmr = new JObject();
                xmr.Add("value", new JValue(xmr_value));
                xmr.Add("change", new JValue(xmr_change));
                xmr.Add("percent", new JValue(xmr_change_percent));
                ExchangeRates.Add("xmr", xmr);
            }

            if (music_value != null)
            {
                var music = new JObject();
                music.Add("value", new JValue(music_value));
                music.Add("change", new JValue(music_change));
                music.Add("percent", new JValue(music_change_percent));
                ExchangeRates.Add("music", music);
                result.Add("ExchangeRates", ExchangeRates);
            }

            /// 
            return result;
        }
    }
}
