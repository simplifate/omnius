using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    [NexusRepository]
    class GetZCashBalanceAction : Action
    {
        public override int Id
        {
            get
            {
                return 3003;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "baseURL", "tableData", "columnName"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Get ZCasch Balance Action";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
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
           string endpoint = (string)vars["baseURL"];
            var tableData = (List<DBItem>)vars["tableData"];
            String columnName = (String)vars["columnName"];
            bool data = vars.ContainsKey("") ? (bool)vars[""] : false;
            String baseURL = "http://api.zcha.in/v2/mainnet/accounts/";
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
            outputVars["Result"] = totalAmount;
        }
    }
}
