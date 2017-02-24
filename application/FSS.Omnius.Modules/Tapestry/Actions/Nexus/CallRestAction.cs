using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.Entitron.Entity;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [NexusRepository]
    class CallRestAction : Action
    {
        public override int Id
        {
            get
            {
                return 3002;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Endpoint", "Method", "?WSName", "?InputData", "?InputFromJSON" };
            }
        }

        public override string Name
        {
            get
            {
                return "Call REST";
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
            string endpoint = (string)vars["Endpoint"];
            string method = (string)vars["Method"];
            string wsName = (string)vars["WSName"];
            bool inputFromJSON = vars.ContainsKey("WSName") ? (bool)vars["WSName"] : false;

            var context = DBEntities.instance;

            var service = context.WSs.First(c => c.Name == wsName);

            string endpointPath = string.Format("{0}/{1}", service.REST_Base_Url.TrimEnd('/'), endpoint.Trim('/'));
            var endpointUri = new Uri(endpointPath);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointUri);

            httpWebRequest.Method = method.ToUpper();
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";

            if (!string.IsNullOrEmpty(service.Auth_User))
                httpWebRequest.Credentials = new NetworkCredential(service.Auth_User, service.Auth_Password);

            if (vars.ContainsKey("InputData"))
            {
                string inputJson;
                if (inputFromJSON)
                {
                    inputJson = (string)vars["InputData"];
                }
                else
                {
                    inputJson = JsonConvert.SerializeObject(vars["InputData"]);
                }

                byte[] postJsonBytes = Encoding.UTF8.GetBytes(inputJson);
                httpWebRequest.ContentLength = postJsonBytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);
            }

            var response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            byte[] buffer = new byte[16 * 1024];
            MemoryStream ms = new MemoryStream();
            int read;
            while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            string outputJsonString = ms.ToString();
            if(!string.IsNullOrEmpty(outputJsonString))
            {
                var outputJToken = JToken.Parse(outputJsonString);
                outputVars["Result"] = outputJToken;
            }
        }
    }
}
