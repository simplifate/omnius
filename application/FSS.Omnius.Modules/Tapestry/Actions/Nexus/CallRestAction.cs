using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    public class CachedResult
    {
        public DateTime resultTime;
        public JToken data;
        public CookieCollection cookies;
    }

    [NexusRepository]
    public class CallRestAction : Action
    {
        public static Dictionary<string, CachedResult> Cache = new Dictionary<string, CachedResult>();

        public override int Id => 3002;

        public override string[] InputVar => new string[] { "WSName", "Method", "?Endpoint", "?InputData", "?InputFromJSON", "?isFromBase64", "?MaxAge", "?CustomHeaders", "?AddressOverride", "?Cookies", "?ContentType" };

        public override string Name => "Call REST";

        public override string[] OutputVar => new string[] { "Result", "Error", "Cookies", "CacheDate" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            /// init
            COREobject core = COREobject.i;
            DBEntities context = core.Context;

            /// input
            string wsName = (string)vars["WSName"];
            string method = (string)vars["Method"];
            string endpoint = vars.ContainsKey("Endpoint") ? (string)vars["Endpoint"] : "";
            string inputData = vars.ContainsKey("InputData") ? (string)vars["InputData"] : "";
            bool inputFromJSON = vars.ContainsKey("InputFromJSON") && (bool)vars["InputFromJSON"];
            bool isFromBase64 = vars.ContainsKey("isFromBase64") && (bool)vars["isFromBase64"];
            int maxAge = vars.ContainsKey("MaxAge") ? (int)vars["MaxAge"] : 0;
            List<object> customHeaders = vars.ContainsKey("CustomHeaders") ? (List<object>)vars["CustomHeaders"] : new List<object>();
            string addressOverride = vars.ContainsKey("AddressOverride") ? (string)vars["AddressOverride"] : null;
            Dictionary<string, string> inputCookies = vars.ContainsKey("Cookies") ? (Dictionary<string, string>)vars["Cookies"] : new Dictionary<string, string>();
            string contentType = vars.ContainsKey("ContentType") ? (string)vars["ContentType"] : "application/json";
                
            string cacheKey = CalculateMD5($"{endpoint}_{method}_{wsName}_{inputData}_{maxAge}_{inputFromJSON}_{isFromBase64}");

            try
            {
                ///
                if (maxAge > 0)
                {
                    if (Cache.ContainsKey(cacheKey))
                    {
                        CachedResult item = Cache[cacheKey];
                        double diff = (DateTime.UtcNow - item.resultTime).TotalSeconds;
                        if (diff <= maxAge)
                        {
                            outputVars["Result"] = item.data;
                            outputVars["Error"] = false;
                            outputVars["Cookies"] = CookiesToDictionary(item.cookies);
                            outputVars["CacheDate"] = item.resultTime;
                            return;
                        }
                    }
                }

                string queryString = "";
                var service = context.WSs.SingleOrDefault(c => c.Name == wsName) ?? throw new Exception($"WS[name:{wsName}] not found!");

                if (inputData != "" && method.ToUpper() == "GET")
                {
                    if (inputFromJSON)
                    {
                        JToken input = JToken.Parse(inputData);
                        queryString = string.Join("&", input.Select(prop => $"{(prop as JProperty).Name}={(prop as JProperty).Value}"));
                    }
                    else
                    {
                        queryString = inputData;
                    }
                }

                string endpointPath = $"{(addressOverride ?? service.REST_Base_Url).TrimEnd('/')}/{endpoint.Trim('/')}?{queryString}".TrimEnd('?').TrimEnd('/');

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);

                httpWebRequest.Method = method.ToUpper();
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Accept = "application/json";
                httpWebRequest.KeepAlive = false;

                foreach (string header in customHeaders)
                {
                    httpWebRequest.Headers.Add(header);
                }

                /// authorise
                if (service != null && !string.IsNullOrEmpty(service.Auth_User))
                {
                    string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{service.Auth_User}:{service.Auth_Password}"));
                    httpWebRequest.Headers.Add("Authorization", $"Basic {authEncoded}");

                    /*NetworkCredential credentials = new NetworkCredential(, );
                    CredentialCache cCache = new CredentialCache();
                    cCache.Add(endpointUri, "Basic", credentials);

                    httpWebRequest.PreAuthenticate = true;
                    httpWebRequest.Credentials = cCache;*/
                }

                /// input data
                if (isFromBase64 && vars.ContainsKey("InputData") && method.ToUpper() == "PUT")
                {
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                    using (BinaryWriter bw = new BinaryWriter(requestStream))
                    {
                        inputData = inputData.Replace("%3D", "=");
                        inputData = inputData.Replace("\r\n", "");
                        inputData = inputData.Replace("\n", "");
                        bw.Write(Convert.FromBase64String(inputData));
                    }
                }
                else if (vars.ContainsKey("InputData") && method.ToUpper() != "GET")
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
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                        requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);
                }
                
                /// cookies
                foreach (var cookie in inputCookies)
                    httpWebRequest.CookieContainer.Add(new Uri("http://" + httpWebRequest.Host), new Cookie(cookie.Key, cookie.Value));

                /// get response
                using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    CookieCollection cookies = response.Cookies;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // image
                        if (response.ContentType.StartsWith("image"))
                        {
                            byte[] rawData = ReadAllBytes(responseStream);

                            outputVars["Result"] = rawData;
                            outputVars["Error"] = false;
                            return;
                        }

                        // string
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string outputJsonString = responseReader.ReadToEnd();

                            if (!string.IsNullOrEmpty(outputJsonString))
                            {
                                try
                                {
                                    var outputJToken = JToken.Parse(outputJsonString);
                                    outputVars["Result"] = outputJToken;
                                    outputVars["Error"] = false;
                                    outputVars["Cookies"] = CookiesToDictionary(cookies);
                                    outputVars["CacheDate"] = new DateTime();

                                    if (maxAge > 0)
                                    {
                                        if (Cache.ContainsKey(cacheKey))
                                        {
                                            Cache[cacheKey].resultTime = DateTime.UtcNow;
                                            Cache[cacheKey].data = outputJToken;
                                            Cache[cacheKey].cookies = cookies;
                                        }
                                        else
                                        {
                                            Cache.Add(cacheKey, new CachedResult()
                                            {
                                                resultTime = DateTime.UtcNow,
                                                data = outputJToken,
                                                cookies = cookies
                                            });
                                        }
                                    }
                                }
                                catch (Exception)
                                { // Neni to json
                                    outputVars["Result"] = outputJsonString;
                                    outputVars["Error"] = false;
                                }
                            }
                            else
                            {
                                outputVars["Result"] = "";
                                outputVars["Error"] = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OmniusWarning.Log($"Call REST - using cached result of endpoint {endpoint} because of error {e.Message}, StackTrace={e.StackTrace}", Watchtower.OmniusLogSource.Nexus);
                if (Cache.ContainsKey(cacheKey))
                {
                    CachedResult item = Cache[cacheKey];
                    outputVars["Result"] = item.data;
                    outputVars["Error"] = true;
                    outputVars["Cookies"] = item.cookies;
                    outputVars["CacheDate"] = item.resultTime;
                    return;
                }
                outputVars["Result"] = string.Empty;
                outputVars["Cookies"] = null;
                outputVars["Error"] = true;
                outputVars["CacheDate"] = new DateTime();
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

        private Dictionary<string, string> CookiesToDictionary(CookieCollection cookies)
        {
            Dictionary<string, string> cookieDictionary = new Dictionary<string, string>();
            foreach (Cookie cookie in cookies)
            {
                cookieDictionary.Add(cookie.Name, cookie.Value);
            }
            return cookieDictionary;
        }

        private byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
