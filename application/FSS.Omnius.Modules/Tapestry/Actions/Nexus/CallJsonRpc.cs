using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Watchtower;
using Jayrock.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
	class CallJsonRpc : Action
	{
		#region JsonIdGenerator
		private static int JsonId = 0;
		private static readonly object mutex = new object();
		private static int GenerateJsonId()
		{
			lock (mutex)
			{
				return JsonId++;
			}
		}
		#endregion

		public override int Id => 3012;

		public override string[] InputVar => new string[] { "WsName", "Method", "?Params", "?ParamsName[index]", "?ParamsValue[index]", "?RpcVersion", "?Endpoint", "?CustomHeaders" };

		public override string Name => "Call JSON RPC";

		public override string[] OutputVar => new string[] { "Result", "Error" };

		public override int? ReverseActionId => null;

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
		{
			try
            {
                /// INIT
                COREobject core = COREobject.i;
                DBEntities context = core.Context;
                var service = context.WSs.First(c => c.Name == (string)vars["WsName"]);

                /// input
				string method = (string)vars["Method"];
                List<object> customHeaders = vars.ContainsKey("CustomHeaders") ? (List<object>)vars["CustomHeaders"] : new List<object>();
                string endpoint = vars.ContainsKey("Endpoint") ? (string)vars["Endpoint"] : "";
                string rpcVersion = vars.ContainsKey("RpcVersion") ? (string)vars["RpcVersion"] : "2.0";
                object parameters;
                if (vars.ContainsKey("Params"))
                    parameters = (string)vars["Params"];
                else
                {
                    int ParamsCount = vars.Keys.Where(k => k.StartsWith("ParamsName[") && k.EndsWith("]")).Count();
                    parameters = new Dictionary<string, object>();
                    for (int i = 0; i < ParamsCount; i++)
                    {
                        (parameters as Dictionary<string, object>).Add(vars[$"ParamsName[{i}]"].ToString(), vars[$"ParamsValue[{i}]"]);
                    }
                }
                
                /// Create request
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create($"{service.REST_Base_Url.TrimEnd('/')}/{endpoint.TrimEnd('/')}");
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Accept = "application/json";
                httpWebRequest.KeepAlive = false;
                
                // authorize
                if (!string.IsNullOrEmpty(service.Auth_User))
                    httpWebRequest.Credentials = new NetworkCredential(service.Auth_User, service.Auth_Password);
                //if (service != null && !string.IsNullOrEmpty(service.Auth_User))
                //{
                //    string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{service.Auth_User}:{service.Auth_Password}"));
                //    httpWebRequest.Headers.Add("Authorization", $"Basic {authEncoded}");
                //}

                // customHeaders
                foreach (string header in customHeaders)
                {
                    httpWebRequest.Headers.Add(header);
                }


                /// Build inputJson
                JsonObject call = new JsonObject();
                call["jsonrpc"] = rpcVersion;
                call["id"] = GenerateJsonId();
                call["method"] = method;
                call["params"] = parameters;

                byte[] postJsonBytes = Encoding.UTF8.GetBytes(call.ToString());
                httpWebRequest.ContentLength = postJsonBytes.Length;
                httpWebRequest.GetRequestStream().Write(postJsonBytes, 0, postJsonBytes.Length);

                /// Response
                // Example form of the response {"jsonrpc": "2.0", "result": 19, "id": 3}
                var response = httpWebRequest.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader responseReader = new StreamReader(responseStream))
                    {
                        var outputJToken = (JObject)JToken.Parse(responseReader.ReadToEnd());

                        outputVars["Result"] = outputJToken["result"];
                        outputVars["Error"] = outputJToken["Error"];
                    }
                }

			}
			catch (Exception e)
			{
				string errorMsg = e.Message;
                COREobject core = (COREobject)vars["__CORE__"];
				OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
				outputVars["Result"] = String.Empty;
				outputVars["Error"] = true;
			}
		}
	}
}
