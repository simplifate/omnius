using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class GetMusicoinBalance : Action
    {
        public override int Id
        {
            get
            {
                return 30213;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "ClientId", "ClientSecret", "Address"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Get Musicoin Balance Action";
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
            string endpointPath = "http://api.musicoin.org/artist/profile/" + (string)vars["Address"];
           
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);
            var headers = new Dictionary<string, string>
            {
                {"clientId",(string)vars["ClientId"]},
                {"clientSecret",(string)vars["ClientSecret"]}
            };
            if (httpWebRequest == null)
                throw new Exception("Non HTTP WebRequest");
            foreach (var a in headers)
            {
                httpWebRequest.Headers.Add(a.Key, a.Value);
            }

            httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";

                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();
        }
    }
}
