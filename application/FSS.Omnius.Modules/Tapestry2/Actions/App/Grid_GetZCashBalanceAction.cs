using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3003, "Get ZCasch Balance Action", "Result")]
        public static double GetZCashBalance(COREobject core, List<DBItem> tableData, string columnName, string baseURL = "http://api.zcha.in/v2/mainnet/accounts/")
        {
            double totalAmount = 0;

            foreach (var row in tableData) 
            {
                String address = (String)row[columnName];

                string endpointPath = baseURL + address;
                //var endpointUri = new Uri(endpointPath);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";

                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();
            
                if (!string.IsNullOrEmpty(outputJsonString))
                {
                    var outputJToken = JToken.Parse(outputJsonString);
                    totalAmount += outputJToken["balance"].ToObject<double>();
                }
            }
            return totalAmount;
        }
    }
}
