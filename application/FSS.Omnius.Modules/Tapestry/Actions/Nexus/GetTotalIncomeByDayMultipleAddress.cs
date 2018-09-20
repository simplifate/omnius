using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    /// <summary>

    /// </summary>
    [NexusRepository]
    public class GetIncomeByDaysMultipleAddressAction : Action
    {
        public override int Id
        {
            get
            {
                return 30124;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] {"AddressList", "ExchangeRate" };
            }
        }
        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }
        public override string Name
        {
            get
            {
                return "Get Income By Days - Multiple address";
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            
            #region DayOfWeekDictionary
            Dictionary<string, int> dictDaysOfWeek = new Dictionary<string, int>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek))
                              .OfType<DayOfWeek>()
                              .ToList()
                              .Skip(1))
            {
                dictDaysOfWeek.Add(day.ToString(), (int)day);
            }
            dictDaysOfWeek.Add("Sunday", 7);
            #endregion

            List<string> addressList = ((string)vars["AddressList"]).Split(',').ToList();
            Dictionary<int, double> totalIncomeByDay = new Dictionary<int, double>();

            #region callRest for each Address in the list
            foreach (string address in addressList)
            {
                Dictionary<string, object> CallRestVars = new Dictionary<string, object>();
                Dictionary<string, object> CallResrOutPutVars = new Dictionary<string, object>();

                CallRestVars.Add("InputData",String.Format(@"{{""module"":""account"",""action"":""txlist"",""address"":""{0}"",""startblock"":""0"",""endblock"":""99999999"",""sort"":""asc"",""apikey"":""2ESS39ZECRFEYR9QS1FQGPT2FNXZPQPFJI""}}",address));
                CallRestVars.Add("Endpoint", "api");
                CallRestVars.Add("Method", "GET");
                CallRestVars.Add("WSName", "BlockExplorer");
                CallRestVars.Add("InputFromJSON",true);
                // For each address we call the api to get json and work with that
                CallRestAction cr = new CallRestAction();
                cr.InnerRun(CallRestVars, CallResrOutPutVars, null, null);

                //NOw we have a JSON output from the Api call
                //we need to call InnerRun action now.
                #region InnerRun

                var jtoken = (JToken)CallResrOutPutVars["Result"];
                var transactions = (JArray)jtoken["result"];
                DayOfWeek dayOfTheWeek = DateTime.Now.DayOfWeek;
                foreach (KeyValuePair<string, int> kv in dictDaysOfWeek)
                {
                    if (dayOfTheWeek.ToString() != "Sunday")
                    {
                        //if today is later than current day in the dictionary
                        //for example today is tuesday in current day in dictionary is Monday
                        if ((int)dayOfTheWeek > kv.Value)
                        {
                            Int32 startTimeStamps = (Int32)(DateTime.Now.Date.AddDays(-(int)dayOfTheWeek + kv.Value).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            Int32 endTimeStamps = startTimeStamps + (24 * 3600);
                            double totalIncomeForDay = 0;
                            foreach (JObject transaction in transactions)
                            {
                                string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                                int timeStamp = Convert.ToInt32(transaction.GetValue("timeStamp"));
                                if (destinationAddress == address && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                                {
                                    var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                    totalIncomeForDay += ethereumValue;

                                }
                            }
                            if (!totalIncomeByDay.ContainsKey(kv.Value))
                            {
                                totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * Convert.ToDouble(vars["ExchangeRate"]));
                            }
                            else {
                                totalIncomeByDay[kv.Value] += (totalIncomeForDay / 1000000000000000000) * Convert.ToDouble(vars["ExchangeRate"]);
                            }
                        }
                    }
                    //endIf
                    //else if today is SUNDAY. The value in enum is 0, but it should be 7 by us.
                    else {
                        if (7 > kv.Value)
                        {
                            Int32 startTimeStamps = (Int32)(DateTime.Now.Date.AddDays(-7 + kv.Value).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            Int32 endTimeStamps = startTimeStamps + (24 * 3600);
                            double totalIncomeForDay = 0;
                            foreach (JObject transaction in transactions)
                            {
                                string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                                int timeStamp = Convert.ToInt32(transaction.GetValue("timeStamp"));
                                if (destinationAddress == address && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                                {
                                    var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                    totalIncomeForDay += ethereumValue;

                                }
                            }
                            if (!totalIncomeByDay.ContainsKey(kv.Value))
                            {
                                totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * Convert.ToDouble(vars["ExchangeRate"]));
                            }
                            else {
                                totalIncomeByDay[kv.Value] += (totalIncomeForDay / 1000000000000000000) * Convert.ToDouble(vars["ExchangeRate"]);
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion


            outputVars["Result"] = totalIncomeByDay;

        }
    }
}