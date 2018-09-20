using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Nexus.Service;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Jayrock.Json;
using Renci.SshNet;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Nexus : ActionManager
    {
        [Action(190, "WebDav Var Upload", "UploadMetadataId")]
        public static int WebDavVarUpload(COREobject core, byte[] FileContent, string FileName = "", string WebDavServerName = "")
        {
            var entities = core.Context;

            FileMetadata fmd = new FileMetadata();
            fmd.AppFolderName = core.Application.Name;
            fmd.CachedCopy = new FileSyncCache();

            fmd.CachedCopy.Blob = FileContent;

            fmd.Filename = FileName;
            fmd.TimeChanged = DateTime.Now;
            fmd.TimeCreated = DateTime.Now;
            fmd.Version = 0;
            fmd.ModelEntityName = "";
            fmd.Tag = "";

            if (!string.IsNullOrWhiteSpace(WebDavServerName))
            {
                fmd.WebDavServer = entities.WebDavServers.Single(a => a.Name == WebDavServerName);
            }
            else
            {
                fmd.WebDavServer = entities.WebDavServers.First();
            }

            entities.FileMetadataRecords.Add(fmd);
            entities.SaveChanges(); //ukládat po jednom souboru

            IFileSyncService service = new WebDavFileSyncService();
            service.UploadFile(fmd);

            return fmd.Id;
        }

        [Action(195, "WebDav Upload", "UploadMetadataId")]
        public static List<int> WebDavUpload(COREobject core, string InputName, string WebDavServerName)
        {
            var files = core.GetRequestFiles();
            if (files == null)
                return null;

            var context = core.AppContext;
            List<int> result = new List<int>();
            foreach (var file in files)
            {
                FileMetadata fmd = new FileMetadata();
                fmd.AppFolderName = core.Application.Name;
                fmd.CachedCopy = new FileSyncCache();
                
                fmd.CachedCopy.Blob = file.Value;

                fmd.Filename = Path.GetFileName(file.Key);
                fmd.TimeChanged = DateTime.Now;
                fmd.TimeCreated = DateTime.Now;
                fmd.Version = 0;

                if (!string.IsNullOrWhiteSpace(WebDavServerName))
                {
                    fmd.WebDavServer = context.WebDavServers.Single(a => a.Name == WebDavServerName);
                }
                else
                    fmd.WebDavServer = context.WebDavServers.First();

                context.FileMetadataRecords.Add(fmd);
                context.SaveChanges(); //ukládat po jednom souboru

                IFileSyncService service = new WebDavFileSyncService();
                service.UploadFile(fmd);

                result.Add(fmd.Id);
            }
            return result;
        }

        [Action(196, "WebDav Download")]
        public static void WebDavDownload(COREobject core, int FileId)
        {
            var entities = core.AppContext;
            FileMetadata fmd = entities.FileMetadataRecords.Find(FileId);

            IFileSyncService serviceFileSync = new WebDavFileSyncService();
            serviceFileSync.DownloadFile(fmd);
            
            core.HttpResponse(fmd.Filename, "application/octet-stream", fmd.CachedCopy.Blob);
        }

        [Action(197, "WebDav entity upload")]
        public static void WebDavEntityUpload(COREobject core, string DescriptionInput, string ModelEntityName = null, string InputNames = null, string WebDavServerName = null, int? NewId = null)
        {
            string modelEntityName = ModelEntityName ?? core.BlockAttribute.ModelTableName;
            string[] descriptionInputs = DescriptionInput.Split(',');
            int newId = NewId ?? core.ModelId;

            var context = core.AppContext;

            var files = core.GetRequestFiles();
            if (files == null)
                return;
            string[] inputNames = InputNames != null
                ? InputNames.Split(',')
                : files.Select(f => f.Key).ToArray();

            foreach (var file in files)
            {
                FileMetadata fmd = new FileMetadata();
                fmd.AppFolderName = core.Application.Name;
                fmd.CachedCopy = new FileSyncCache();
                
                fmd.CachedCopy.Blob = file.Value;

                fmd.Filename = Path.GetFileName(file.Key);
                fmd.TimeChanged = DateTime.Now;
                fmd.TimeCreated = DateTime.Now;
                fmd.Version = 0;

                if (!string.IsNullOrWhiteSpace(WebDavServerName))
                {
                    fmd.WebDavServer = context.WebDavServers.Single(a => a.Name == WebDavServerName);
                }
                else
                    fmd.WebDavServer = context.WebDavServers.First();

                if (descriptionInputs != null && descriptionInputs.Length > 0)
                {
                    int inputIndex = Array.IndexOf(inputNames, file.Key);
                    string descInp = descriptionInputs[inputIndex];

                    fmd.Description = core.Data[descInp].ToString();
                }
                else if (core.Data.ContainsKey(file.Key + "_description"))
                {
                    fmd.Description = core.Data[file.Key + "_description"].ToString();
                }

                fmd.ModelEntityId = newId;
                fmd.ModelEntityName = modelEntityName;
                fmd.Tag = file.Key; //TODO: český čitelný název (systémová tabulka s klíčema a hodnotama?)

                context.FileMetadataRecords.Add(fmd);
                context.SaveChanges(); //ukládat po jednom souboru

                IFileSyncService service = new WebDavFileSyncService();
                service.UploadFile(fmd);
            }
        }

        [Action(199, "WebDav Delete")]
        public static void WebDavDelete(COREobject core, int FileId)
        {
            var context = core.AppContext;
            FileMetadata fmd = context.FileMetadataRecords.Find(FileId);

            IFileSyncService serviceFileSync = new WebDavFileSyncService();
            serviceFileSync.DeleteFile(fmd);
        }

        [Action(205, "Convert JValue", "Result")]
        public static object ConvertJValue(COREobject core, string Type, JValue From)
        {
            switch (Type)
            {
                case "Integer":
                    return From.ToObject<int>();
                case "Double":
                    return From.ToObject<double>();
                case "Bool":
                    return From.ToObject<bool>();
                default:
                    return From.ToObject<string>();
            }
        }

        [Action(3001, "Call SOAP", "Data")]
        public static JToken CallSOAPWS(COREobject core, string WSName, string MethodName, string[] Param, string JsonBody = null)
        {
            // init
            NexusWSService svc = new NexusWSService();
            JToken data;

            if (JsonBody != null)
            {
                data = svc.CallWebService(WSName, MethodName, JsonBody);
            }
            else
            {
                List<string> parameters = new List<string>();

                foreach (string key in Param)
                {
                    parameters.Add((string)core.Data[key]);
                }
                object[] args = parameters.ToArray<object>();

                data = svc.CallWebService(WSName, MethodName, args);
            }

            return data;
        }

        [Action(3002, "Call REST", "Result", "Error", "Cookies", "CacheDate")]
        public static (JToken, bool, Dictionary<string, string>, DateTime) CallRest(COREobject core, string Endpoint, string Method, string WSName = null, string InputData = "", string AddressOverride = null, int MaxAge = 0, bool InputFromJSON = false, Dictionary<string, string> Cookies = null)
        {
            var context = core.Context;
            try
            {
                string keyData = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                            Endpoint,
                            Method,
                            WSName,
                            InputData,
                            MaxAge.ToString(),
                            InputFromJSON ? "1" : "0"
                        );
                string cacheKey = CalculateMD5(keyData);

                if (MaxAge > 0)
                {
                    if (cache.ContainsKey(cacheKey))
                    {
                        CachedResult item = cache[cacheKey];
                        double diff = (DateTime.UtcNow - item.resultTime).TotalSeconds;
                        if (diff <= MaxAge)
                        {
                            return (item.data, false, CookiesToDictionary(item.cookies), item.resultTime);
                        }
                    }
                }

                string queryString = "";
                var service = !String.IsNullOrEmpty(WSName) ? context.WSs.First(c => c.Name == WSName) : null;

                if (InputData != "" && Method.ToUpper() == "GET")
                {
                    if (InputFromJSON)
                    {
                        JToken input = JToken.Parse(InputData);
                        foreach (JProperty key in input)
                        {
                            queryString += key.Name + "=" + (string)key.Value + '&';
                        }
                        queryString = queryString.TrimEnd('&');
                    }
                    else
                    {
                        queryString = InputData;
                    }
                }

                string endpointPath = string.Format("{0}/{1}?{2}",
                    AddressOverride == null ? service.REST_Base_Url.TrimEnd('/') : AddressOverride.TrimEnd('/'),
                    Endpoint.Trim('/'),
                    queryString);
                //var endpointUri = new Uri(endpointPath);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);

                httpWebRequest.CookieContainer = new CookieContainer();
                httpWebRequest.Method = Method.ToUpper();
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Accept = "application/json";

                if (service != null && !string.IsNullOrEmpty(service.Auth_User))
                {
                    string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{service.Auth_User}:{service.Auth_Password}"));
                    httpWebRequest.Headers.Add("Authorization", $"Basic {authEncoded}");
                }

                if (InputData != "" && Method.ToUpper() != "GET")
                {
                    string inputJson;
                    if (InputFromJSON)
                    {
                        inputJson = InputData;
                    }
                    else
                    {
                        inputJson = JsonConvert.SerializeObject(InputData);
                    }

                    byte[] postJsonBytes = Encoding.UTF8.GetBytes(inputJson);
                    httpWebRequest.ContentLength = postJsonBytes.Length;
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);
                }

                if (Cookies != null)
                {
                    foreach (var cookie in Cookies)
                        httpWebRequest.CookieContainer.Add(new Uri("http://" + httpWebRequest.Host), new Cookie(cookie.Key, cookie.Value));
                }
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                CookieCollection cookies = response.Cookies;
                Stream responseStream = response.GetResponseStream();

                if (response.ContentType.StartsWith("image"))
                {
                    byte[] rawData;

                    using (var ms = new MemoryStream())
                    {
                        responseStream.CopyTo(ms);
                        rawData = ms.ToArray();
                    }

                    return (rawData, false, null, default(DateTime));
                }
                else
                {
                    StreamReader responseReader = new StreamReader(responseStream);
                    string outputJsonString = responseReader.ReadToEnd();

                    if (!string.IsNullOrEmpty(outputJsonString))
                    {
                        try
                        {
                            var outputJToken = JToken.Parse(outputJsonString);

                            if (cache.ContainsKey(cacheKey))
                            {
                                cache[cacheKey].resultTime = DateTime.UtcNow;
                                cache[cacheKey].data = outputJToken;
                                cache[cacheKey].cookies = cookies;
                            }
                            else
                            {
                                cache.Add(cacheKey, new CachedResult()
                                {
                                    resultTime = DateTime.UtcNow,
                                    data = outputJToken,
                                    cookies = cookies
                                });
                            }

                            return (outputJToken, false, CookiesToDictionary(cookies), new DateTime());
                        }
                        catch (Exception)
                        { // Neni to json
                            return (outputJsonString, false, null, default(DateTime));
                        }
                    }

                    return (null, false, null, default(DateTime));
                }
            }
            catch (Exception e)
            {
                string wsName = AddressOverride ?? WSName;
                string errorMsg = e.Message;
                Watchtower.OmniusWarning.Log($"Call REST - using cached result of endpoint {Endpoint} because of error {errorMsg}, StackTrace={e.StackTrace}", OmniusLogSource.Nexus);
                //outputVars["Result"] = String.Empty;
                string keyData = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                       Endpoint,
                       Method,
                       wsName,
                       InputData,
                       MaxAge.ToString(),
                       InputFromJSON ? "1" : "0"
                   );
                string cacheKey = CalculateMD5(keyData);

                if (cache.ContainsKey(cacheKey))
                {
                    CachedResult item = cache[cacheKey];
                    return (item.data, true, CookiesToDictionary(item.cookies), item.resultTime);
                }

                return (string.Empty, true, null, new DateTime());
            }
        }
        private static string CalculateMD5(string data)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
        private static Dictionary<string, string> CookiesToDictionary(CookieCollection cookies)
        {
            Dictionary<string, string> cookieDictionary = new Dictionary<string, string>();
            foreach (Cookie cookie in cookies)
            {
                cookieDictionary.Add(cookie.Name, cookie.Value);
            }
            return cookieDictionary;
        }
        public static Dictionary<string, CachedResult> cache = new Dictionary<string, CachedResult>();
        public class CachedResult
        {
            public DateTime resultTime;
            public JToken data;
            public CookieCollection cookies;
        }

        [Action(3012, "Call JSON RPC", "Result", "Error")]
        public static (JToken, JToken) CallJsonRpc(COREobject core, string WSName, string Method, string[] ParamsName, object[] ParamsValue, string RpcVersion, string Endpoint = null)
        {
            var context = core.Context;

            try
            {
                //parameters
                Dictionary<string, object> paramsDict = new Dictionary<string, object>();
                for (int i = 0; i < ParamsName.Length; i++)
                {
                    paramsDict.Add(ParamsName[i], ParamsValue[i]);
                }
                
                // vezmu uri
                var service = context.WSs.First(c => c.Name == WSName);

                //get username and password from rpc service 
                string rpcUserName = service.Auth_User;
                string rpcPassword = service.Auth_Password;

                // Create request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(service.REST_Base_Url);
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Credentials = new NetworkCredential(rpcUserName, rpcPassword); //make credentials for request


                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";

                JsonObject call = new JsonObject();
                call["jsonrpc"] = RpcVersion;
                call["id"] = GenerateJsonId();
                call["method"] = Method;
                call["params"] = paramsDict;
                string jsonString = call.ToString();

                byte[] postJsonBytes = Encoding.UTF8.GetBytes(jsonString);

                httpWebRequest.ContentLength = postJsonBytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);

                var response = httpWebRequest.GetResponse();
                using (Stream stream2 = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream2, Encoding.UTF8))
                {
                    JsonObject answer = new JsonObject();
                    answer.Import(new Jayrock.Json.JsonTextReader(reader));
                    return (JToken.Parse(answer["result"].ToString()), JToken.Parse(answer["error"].ToString()));
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
                return (null, JToken.Parse("true"));
            }
        }
        private static int JsonId = 0;
        private static readonly object mutex = new object();
        private static int GenerateJsonId()
        {
            lock (mutex)
            {
                return JsonId++;
            }
        }

        [Action(3014, "Run SSH command", "Result", "Error")]
        public static (string, bool) RunSSHCommand(COREobject core, string Hostname, int Port, string Username, string Password, string Command)
        {
            try
            {
                using (var client = new SshClient(Hostname, Port, Username, Password))
                {
                    client.Connect();
                    var result = client.RunCommand(Command);
                    client.Disconnect();

                    return (result.Result, false);
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
                return (string.Empty, true);
            }
        }

        [Action(3015, "List to string", "Result")]
        public static string ListToString(COREobject core, List<object> List, string Separator = ",")
        {
            return string.Join(Separator, List.ToArray());
        }

        [Action(3017, "ExtDB: Update", "Result", "Error")]
        public static (object, bool) ExtDBUpdate(COREobject core, string dbName, string TableName, JToken Data, object Where)
        {
            var context = core.AppContext;
            try
            {
                if (Where == null || (Where is String && string.IsNullOrEmpty((string)Where)))
                {
                    throw new Exception("where is missing. You must provide where clausule or rethingDB item id");
                }

                ExtDB dbInfo = context.ExtDBs.Where(d => d.DB_Alias == dbName).SingleOrDefault();
                if (dbInfo == null)
                {
                    throw new Exception("Integration was not found");
                }

                NexusExtDBBaseService service;
                switch (dbInfo.DB_Type)
                {
                    case ExtDBType.RethinkDB:
                        service = new NexusExtDBRethingService(dbInfo);
                        break;
                    default:
                        service = (new NexusExtDBService(dbInfo.DB_Server, dbInfo.DB_Alias)).NewQuery("");
                        break;
                }

                NexusExtDBResult result = service.Update(TableName, Data, Where);

                if (result.Errors == 0)
                {
                    return (result.Replaced, false);
                }
                else
                {
                    OmniusException.Log(result.FirstError, OmniusLogSource.Nexus, null, core.Application, core.User);
                    return (result.FirstError, true);
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
                return (string.Empty, true);
            }
        }

        [Action(3018, "ExtDB: Select", "Result", "Error")]
        public static (JToken, bool) ExtDBSelect(COREobject core, string dbName, string TableName, string[] CondColumn, string[] CondOperator, object[] CondValue, string OrderBy = null, bool OrderByIndex = false, int? Limit = null, int? Skip = null)
        {
            var context = core.AppContext;
            try
            {
                ExtDB dbInfo = context.ExtDBs.Where(d => d.DB_Alias == dbName).SingleOrDefault();
                if (dbInfo == null)
                    throw new Exception("Integration was not found");

                NexusExtDBBaseService service;
                switch (dbInfo.DB_Type)
                {
                    case ExtDBType.RethinkDB:
                        service = new NexusExtDBRethingService(dbInfo);
                        break;
                    default:
                        service = (new NexusExtDBService(dbInfo.DB_Server, dbInfo.DB_Alias)).NewQuery("").Select("*");
                        break;
                }

                var query = service.From(TableName);

                if (service is NexusExtDBRethingService && OrderBy != null)
                {
                    query = OrderByIndex ? query.OrderBy($"index:{OrderBy}") : query.OrderBy(OrderBy);
                }
                if (CondColumn.Length > 0)
                {
                    JArray cond = new JArray();
                    // setConditions
                    for (int i = 0; i < CondColumn.Length; i++)
                    {
                        string condOperator = CondOperator[i] ?? "eq";
                        string condColumn = CondColumn[i];
                        object condValue = CondValue[i];

                        var c = new JObject();
                        c["column"] = condColumn;
                        c["operator"] = condOperator;
                        c["value"] = JToken.FromObject(condValue);

                        cond.Add(c);
                    }
                    query = query.Where(cond);
                }

                if (service is NexusExtDBService && OrderBy != null)
                {
                    query = query.OrderBy(OrderBy);
                }

                if (Limit != null)
                {
                    query = query.Limit(Limit.Value);
                }
                if (Skip != null)
                {
                    query = query.Offset(Skip.Value);
                }

                var data = query.FetchAll();

                return (data, false);
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
                return (string.Empty, true);
            }
        }

        [Action(3019, "ExtDB: Insert", "Result", "Error")]
        public static (object, bool) ExtDBInsert(COREobject core, string dbName, string TableName, JToken Data)
        {
            var context = core.AppContext;
            try
            {
                ExtDB dbInfo = context.ExtDBs.Where(d => d.DB_Alias == dbName).SingleOrDefault();
                if (dbInfo == null)
                {
                    throw new Exception("Integration was not found");
                }

                NexusExtDBBaseService service;
                switch (dbInfo.DB_Type)
                {
                    case ExtDBType.RethinkDB:
                        service = new NexusExtDBRethingService(dbInfo);
                        break;
                    default:
                        service = (new NexusExtDBService(dbInfo.DB_Server, dbInfo.DB_Alias)).NewQuery("");
                        break;
                }

                NexusExtDBResult result = service.Insert(TableName, Data);

                if (result.Errors == 0)
                {
                    return (result.GeneratedKeys[0], false);
                }
                else
                {
                    OmniusException.Log(result.FirstError, OmniusLogSource.Nexus, null, core.Application, core.User);
                    return (result.FirstError, true);
                }
            }
            catch (Exception e)
            {
                string errorMsg = e.Message;
                OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
                return (string.Empty, true);
            }
        }

        [Action(3020, "Json Stringify", "Result")]
        public static string JsonStringify(COREobject core, JToken JToken, bool PrettyPrint = false)
        {
            if (PrettyPrint)
            {
                return JToken.ToString(Formatting.Indented);
            }

            return JToken.ToString(Formatting.None);
        }

        [Action(3192, "Call REST HMAC BITFINEX AUTHENTICATED")]
        public static void CallRestHmacBitfinexAuthenticated(COREobject core, string Url, string ApiKey, string ApiSecret, string ParamsDict)
        {
            string payload64 = EncodeTo64(ParamsDict);


            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(ApiSecret);
            HMACSHA384 hmacsha384 = new HMACSHA384(keyByte);
            byte[] messageBytes = encoding.GetBytes(payload64);
            byte[] hashmessage = hmacsha384.ComputeHash(messageBytes);
            string signature = ByteToString(hashmessage);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            var headers = new Dictionary<string, string>
            {
                {"X-BFX-APIKEY",ApiKey},
                {"X-BFX-PAYLOAD",ParamsDict},
                {"X-BFX-SIGNATURE",signature}
            };
            if (httpWebRequest == null)
                throw new Exception("Non HTTP WebRequest");

            var data = encoding.GetBytes(payload64);
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 30000;
            httpWebRequest.ContentLength = data.Length;
            foreach (var a in headers)
            {
                httpWebRequest.Headers.Add(a.Key, a.Value);
            }

            var write = httpWebRequest.GetRequestStream();
            write.Write(data, 0, data.Length);

            var response = httpWebRequest.GetResponse() as HttpWebResponse;
        }
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.UTF8.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }

        [Action(3193, "Call REST HMAC POLONIEX AUTHENTICATED")]
        public static void CallRestHmacPoloniexAuthenticated(COREobject core, string Url, string Params, string ApiKey, string ApiSecret)
        {
            string myParam = Params + DateTime.Now.Ticks.ToString();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("Key", ApiKey);
            httpWebRequest.Headers.Add("Sign", GenerateHmac(myParam, ApiKey, ApiSecret));

            var postData = myParam;
            var data = Encoding.UTF8.GetBytes(postData);

            httpWebRequest.ContentLength = data.Length;
            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }



            var response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string outputJsonString = responseReader.ReadToEnd();
        }
        private static string GenerateHmac(string msg, string ApiKey, string ApiSecret)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(ApiSecret));
            var messagebyte = Encoding.UTF8.GetBytes(msg);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }

        [Action(3194, "Call REST HMAC KRAKEN AUTHENTICATED")]
        public static void CallRestHmacKrakenAuthenticated(COREobject core, string Url, string ApiVersion, string ApiMethod, string ApiKey, string ApiSecret)
        {
            string Path = "/" + ApiVersion + "/private/" + ApiMethod; // The path like "/0/private/Balance";

            var c_nonce = DateTime.Now.Ticks.ToString();
            String postData = "nonce=" + c_nonce;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("API-Key", ApiKey);
            httpWebRequest.Headers.Add("API-Sign", GenerateHmac(Path + hash256(c_nonce + postData), encode64(ApiSecret)));

            var data = Encoding.UTF8.GetBytes(postData);

            httpWebRequest.ContentLength = data.Length;
            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }



            var response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string outputJsonString = responseReader.ReadToEnd();
        }
        public static string hash256(string strData)
        {
            var message = Encoding.UTF8.GetBytes(strData);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        public static string encode64(string strData)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(strData);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }
        public static string GenerateHmac(string message, string ApiSecret)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(ApiSecret));
            var messagebyte = Encoding.UTF8.GetBytes(message);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }

        [Action(30101, "Get Table From JSON", "Result")]
        public static List<DBItem> GetTableFromJSON(COREobject core, JArray InputJSON)
        {
            DBConnection db = core.Entitron;

            List<DBItem> dbItems = new List<DBItem>();
            if (InputJSON != null)
            {
                //iterate jsonObject in the array
                foreach (var jsonobject in InputJSON.Children())
                {
                    DBItem item = new DBItem(db, null);
                    foreach (JProperty property in ((JObject)jsonobject).Properties())
                    {
                        item[property.Name] = property.Value;
                    } //create properties for item
                    dbItems.Add(item); //add item to the list
                }
            }

            return dbItems;
        }

        [Action(30102, "Count Object In Array", "Result")]
        public static int CountObjectInArray(COREobject core, JArray Array, string Property = null, string Value = null)
        {
            if (Array.Count <= 0)
                return 0;
            
            if (Property == null || Value == null)
                return Array.Count;

            return Array.Count(i => (i as JObject).Property(Property).Value.ToString() == Value);
        }

        [Action(30103, "Count Object In Object", "Result")]
        public static int CountObjectInObject(COREobject core, JObject InputObject, string Property = null, string Value = null)
        {
            if (InputObject.Count <= 0)
                return 0;

            if (Property == null || Value == null)
                return InputObject.Count;

            int counter = 0;
            foreach (var item in InputObject)
            {
                if (((JObject)item.Value).Property(Property).Value.ToString() == Value)
                    counter++;
            }
            return counter;
        }

        [Action(30105, "Total Addresses Balance", "Result")]
        public static double TotalAddressesBalance(COREobject core, JToken Array, string BalanceKey = "balance")
        {
            double total = 0;
            if (Array is JArray)
            {
                JArray array = (JArray)Array;
                if (array.Count <= 0)
                    return 0;

                foreach (JObject obj in array)
                {
                    total += Convert.ToDouble(obj[BalanceKey]);
                }
                return total;
            }
            else if (Array is JObject)
            {
                JObject jObject = (JObject)Array;
                if (jObject[BalanceKey] != null)
                    return Convert.ToDouble(jObject[BalanceKey]);
                else
                    return 0;
            }
            else
            {
                throw new ArgumentException("Total Address Balance Action: Input is not JArray nor JObject!");
            }
        }

        [Action(30106, "Sum Values In JArray", "Result")]
        public static double SumValuesInJArray(COREobject core, JArray Array, string ValueName)
        {
            if (Array.Count <= 0)
                return 0.0;

            double total = 0;
            foreach (JObject obj in Array)
            {
                total += Convert.ToDouble(obj[ValueName]);
            }
            return total;
        }

        [Action(30119, "Get Total Income By Address", "Result")]
        public static double GetTotalIncomeByAddress(COREobject core, JArray TransactionArray, string Address)
        {
            double totalIncome = 0;
            //iterate transaction in the JSON array
            foreach (JObject transaction in TransactionArray)
            {
                string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                if (destinationAddress == Address)
                {
                    var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                    totalIncome += ethereumValue;
                }
            }

            return totalIncome;
        }

        [Action(30122, "Get Income By Days", "Result")]
        public static Dictionary<int, double> GetIncomeByDays(COREobject core, JArray TransactionArray, string Address, double ExchangeRate)
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
                        foreach (JObject transaction in TransactionArray)
                        {
                            string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                            int timeStamp = Convert.ToInt32(transaction.GetValue("timeStamp"));
                            if (destinationAddress == Address && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                            {
                                var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                totalIncomeForDay += ethereumValue;

                            }
                        }
                        totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * ExchangeRate);
                    }
                }
                //endIf
                //else if today is SUNDAY. The value in enum is 0, but it should be 7 by us.
                else
                {
                    if (7 > kv.Value)
                    {
                        Int32 startTimeStamps = (Int32)(DateTime.Now.Date.AddDays(-7 + kv.Value).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        Int32 endTimeStamps = startTimeStamps + (24 * 3600);
                        double totalIncomeForDay = 0;
                        foreach (JObject transaction in TransactionArray)
                        {
                            string destinationAddress = transaction.GetValue("to").ToString(); //get destination address value
                            int timeStamp = Convert.ToInt32(transaction.GetValue("timeStamp"));
                            if (destinationAddress == Address && timeStamp >= startTimeStamps && timeStamp <= endTimeStamps)
                            {
                                var ethereumValue = Convert.ToDouble(transaction.GetValue("value"));
                                totalIncomeForDay += ethereumValue;

                            }
                        }
                        totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * ExchangeRate);
                    }
                }
            }

            return totalIncomeByDay;
        }

        [Action(30123, "Filter Object In Array By String Value", "Result")]
        public static JArray FilterObjectInArray(COREobject core, JArray Array, string Property = null, string Value = null)
        {
            var outputArray = new JArray();

            if (Array.Count <= 0)
                return outputArray;

            if (Property == null || Value == null)
                return Array;
            
            foreach (var item in Array)
            {
                if (((JObject)item).Property(Property).Value.ToString() == Value)
                    outputArray.Add(item);
            }
            return outputArray;
        }

        [Action(30124, "Get Income By Days - Multiple address")]
        public static Dictionary<int, double> GetIncomeByDaysMultipleAddress(COREobject core, string AddressList, double ExchangeRate)
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

            List<string> addressList = AddressList.Split(',').ToList();
            Dictionary<int, double> totalIncomeByDay = new Dictionary<int, double>();

            #region callRest for each Address in the list
            foreach (string address in addressList)
            {
                string data = $"{{\"module\":\"account\",\"action\":\"txlist\",\"address\":\"{address}\",\"startblock\":\"0\",\"endblock\":\"99999999\",\"sort\":\"asc\",\"apikey\":\"2ESS39ZECRFEYR9QS1FQGPT2FNXZPQPFJI\"}}";
                // For each address we call the api to get json and work with that
                var CallResrOut = CallRest(core, "api", "GET", "BlockExplorer", data);

                //NOw we have a JSON output from the Api call
                //we need to call InnerRun action now.
                #region InnerRun
                
                var transactions = (JArray)CallResrOut.Item1["result"];
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
                                totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * ExchangeRate);
                            }
                            else
                            {
                                totalIncomeByDay[kv.Value] += (totalIncomeForDay / 1000000000000000000) * ExchangeRate;
                            }
                        }
                    }
                    //endIf
                    //else if today is SUNDAY. The value in enum is 0, but it should be 7 by us.
                    else
                    {
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
                                totalIncomeByDay.Add(kv.Value, (totalIncomeForDay / 1000000000000000000) * ExchangeRate);
                            }
                            else
                            {
                                totalIncomeByDay[kv.Value] += (totalIncomeForDay / 1000000000000000000) * ExchangeRate;
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion
            
            return totalIncomeByDay;
        }

        [Action(30125, "Filter Object In Array By Value In Array", "Result")]
        public static JArray FilterObjectInArrayByValueInArray(COREobject core, JArray Array, string Property = null, List<Object> Value = null)
        {
            var outputArray = new JArray();

            if (Array.Count <= 0)
                return outputArray;

            if (Property == null || Value == null)
                return Array;

            foreach (var item in Array)
            {
                Object dbItemObject = ((JObject)item).Property(Property).Value.ToObject<Object>();

                if (Value.Contains(dbItemObject))
                    outputArray.Add(item);
            }
            return outputArray;
        }

        [Action(30128, "Parse modbus values", "Result")]
        public static JObject ParseModbusValues(COREobject core, JToken ModbusValuesArray)
        {
            List<DataItem> dataMap = new List<DataItem>()
            {
                new DataItem() { Id = "EnergieChlazeni",   High = 0,  Low = 3  },
                new DataItem() { Id = "PrikonChlazeni",    High = 6,  Low = 9  },
                new DataItem() { Id = "EnergieDatCentrum", High = 12, Low = 15 },
                new DataItem() { Id = "PrikonDatCentrum",  High = 18, Low = 21 },
                new DataItem() { Id = "EnergieKGJ",        High = 24, Low = 27 },
                new DataItem() { Id = "PrikonKGJ",         High = 30, Low = 33 }
            };

            JObject modbusValues = new JObject();

            for (var i = 0; i < dataMap.Count; i++)
            {
                DataItem item = dataMap[i];
                double value = 0;
                int exp = 6;
                int k = 0;
                for (var j = item.High; j < item.Low; j++)
                {
                    var b = (double)ModbusValuesArray[j];
                    value += b * System.Math.Pow(0x10, exp - k * 2);
                    k++;
                }

                modbusValues.Add(item.Id, value);
            }

            return modbusValues;
        }
        class DataItem
        {
            public string Id;
            public int High;
            public int Low;
        }

        [Action(30234, "Json to Dictionary", "Result")]
        public static Dictionary<string, object> Json2Dict(COREobject core, JObject JsonObject)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (JProperty property in JsonObject.Properties())
            {
                var value = property.Value.ToString();
                var key = property.Name;
                dict.Add(key, value);
            }

            return dict;
        }

        [Action(300212, "Call JsonRPC Over TCP", "Result")]
        public static JObject CallJsonRpcTcp(COREobject core, string CurencyPair, string Method, string IpAddress, int Port, string Params = "", int receivedBufferSize = 20060)
        {
            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{CurencyPair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"{Method}\", \"params\": {Params} \"id\": {requestId++ + 1}}}";

            var receiveBytes = new byte[receivedBufferSize];
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = 10000;
                socket.Connect(IpAddress, Port);
                socket.Send(Encoding.UTF8.GetBytes(initJson + "\n"));

                socket.Send(Encoding.UTF8.GetBytes(inputJson + "\n"));

                socket.Shutdown(SocketShutdown.Send);
                socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);

                socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);
            }
            var responseJson = Encoding.UTF8.GetString(receiveBytes);
            return JObject.Parse(responseJson);
        }
        public static int requestId = 0;
    }
}
