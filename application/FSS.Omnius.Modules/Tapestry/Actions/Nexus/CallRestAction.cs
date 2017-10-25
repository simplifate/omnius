using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Watchtower;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    class CachedResult
    {
        public DateTime resultTime;
        public JToken data;
    }

    [NexusRepository]
    public class CallRestAction : Action
    {
        private static Dictionary<string, CachedResult> cache = new Dictionary<string, CachedResult>();

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
                return new string[] { "Endpoint", "Method", "?WSName", "?InputData", "?InputFromJSON", "?MaxAge", "?v$CustomHeaders" };
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
                    "Result", "Error"
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
            try
            {
                string endpoint = (string)vars["Endpoint"];
                string method = (string)vars["Method"];
                string wsName = (string)vars["WSName"];
                string inputData = vars.ContainsKey("InputData") ? (string)vars["InputData"] : "";
                List<string> customHeaders = vars.ContainsKey("CustomHeaders") ? (List<string>)vars["CustomHeaders"] : new List<string>(); 
                int maxAge = vars.ContainsKey("MaxAge") ? (int)vars["MaxAge"] : 0;
                bool inputFromJSON = vars.ContainsKey("InputFromJSON") ? (bool)vars["InputFromJSON"] : false;

                string keyData = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                            endpoint,
                            method,
                            wsName,
                            inputData,
                            maxAge.ToString(),
                            inputFromJSON ? "1" : "0"
                        );
                string cacheKey = CalculateMD5(keyData);

                if (maxAge > 0)
                {
                    if (cache.ContainsKey(cacheKey))
                    {
                        CachedResult item = cache[cacheKey];
                        double diff = (DateTime.UtcNow - item.resultTime).TotalSeconds;
                        if (diff <= maxAge)
                        {
                            outputVars["Result"] = item.data;
                            outputVars["Error"] = false;
                            return;
                        }
                    }
                }

                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                var context = DBEntities.appInstance(core.Entitron.Application);
                string queryString = "";
                var service = context.WSs.First(c => c.Name == wsName);

                if (inputData != "" && method.ToUpper() == "GET")
                {
                    if (inputFromJSON)
                    {
                        JToken input = JToken.Parse(inputData);
                        foreach (JProperty key in input)
                        {
                            queryString += key.Name + "=" + (string)key.Value + '&';
                        }
                        queryString = queryString.TrimEnd('&');
                    }
                    else
                    {
                        queryString = inputData;
                    }
                }

                string endpointPath = string.Format("{0}/{1}?{2}", service.REST_Base_Url.TrimEnd('/'), endpoint.Trim('/'), queryString);
                //var endpointUri = new Uri(endpointPath);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);

                httpWebRequest.Method = method.ToUpper();
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";
                
                foreach(string header in customHeaders) {
                    httpWebRequest.Headers.Add(header);
                }
                
                if (!string.IsNullOrEmpty(service.Auth_User))
                {
                    httpWebRequest.Credentials = new NetworkCredential(service.Auth_User, service.Auth_Password);
                }

                if (vars.ContainsKey("InputData") && method.ToUpper() != "GET")
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
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();

                if (!string.IsNullOrEmpty(outputJsonString))
                {
                    var outputJToken = JToken.Parse(outputJsonString);
                    outputVars["Result"] = outputJToken;
                    outputVars["Error"] = false;

                    if (maxAge > 0)
                    {
                        if (cache.ContainsKey(cacheKey))
                        {
                            cache[cacheKey].resultTime = DateTime.UtcNow;
                            cache[cacheKey].data = outputJToken;
                        }
                        else
                        {
                            cache.Add(cacheKey, new CachedResult()
                            {
                                resultTime = DateTime.UtcNow,
                                data = outputJToken
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Entitron.Application, core.User);
                outputVars["Result"] = String.Empty;
                outputVars["Error"] = true;
            }
        }

        private string CalculateMD5(string data)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(data);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
