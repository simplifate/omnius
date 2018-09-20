﻿using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Nexus.Service;
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
    public class GetIncomeByDaysAction : Action
    {
        public override int Id
        {
            get
            {
                return 30122;
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
                return new string[] { "TransactionArray", "Address","ExchangeRate"};
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
                return "Get Income By Days";
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

            Dictionary<int, double> totalIncomeByDay = new Dictionary<int, double>();
            var transactions = (JArray)vars["TransactionArray"];
            DayOfWeek dayOfTheWeek = DateTime.Now.DayOfWeek;
            foreach (KeyValuePair<string,int> kv in dictDaysOfWeek) {
                if (dayOfTheWeek.ToString() != "Sunday") {
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
                            if (destinationAddress == (string)vars["Address"] && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                            {
                                var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                totalIncomeForDay += ethereumValue;

                            }
                        }
                        totalIncomeByDay.Add(kv.Value, (totalIncomeForDay/1000000000000000000) * Convert.ToDouble(vars["ExchangeRate"]) );
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
                            if (destinationAddress == (string)vars["Address"] && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                            {
                                var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                totalIncomeForDay += ethereumValue;

                            }
                        }
                        totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * (Double)vars["ExchangeRate"]);
                    }
                }
            }
            outputVars["Result"] = totalIncomeByDay;

        }
    }
}