using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Watchtower;
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

		public override string[] InputVar => new string[] { "WsName", "Method", "Params", "?Endpoint", "?v$CustomHeaders" };

		public override string Name => "Call JSON RPC";

		public override string[] OutputVar => new string[] { "Result", "Error" };

		public override int? ReverseActionId => null;

		public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
		{
			try
			{
				string wsName = (string)vars["WsName"];
				string method = (string)vars["Method"];
				string parameters = (string)vars["Params"];
                List<object> customHeaders = vars.ContainsKey("CustomHeaders") ? (List<object>)vars["CustomHeaders"] : new List<object>();
                string endpoint = vars.ContainsKey("Endpoint") ? (string)vars["Endpoint"] : "";
                // vezmu uri
                CORE.CORE core = (CORE.CORE)vars["__CORE__"];
				var context = DBEntities.appInstance(core.Application);
				var service = context.WSs.First(c => c.Name == wsName);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                string endpointPath = string.Format("{0}/{1}", service.REST_Base_Url.TrimEnd('/'), endpoint.Trim('/'));

				// Create request
				var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Accept = "application/json";
                httpWebRequest.KeepAlive = false;

                foreach (string header in customHeaders)
                {
                    httpWebRequest.Headers.Add(header);
                }

                if (service != null && !string.IsNullOrEmpty(service.Auth_User))
                {
                    string authEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{service.Auth_User}:{service.Auth_Password}"));
                    httpWebRequest.Headers.Add("Authorization", $"Basic {authEncoded}");
                }

                // Build inputJson
                // Example form of the request: {"jsonrpc": "2.0", "method": "subtract", "params": {"minuend": 42, "subtrahend": 23}, "id": 3}
                string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"{method}\", \"params\": {parameters}, \"id\": {GenerateJsonId()}}}";
                byte[] postJsonBytes = Encoding.UTF8.GetBytes(inputJson);
                httpWebRequest.ContentLength = postJsonBytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);

                // Example form of the response {"jsonrpc": "2.0", "result": 19, "id": 3}
                var response = httpWebRequest.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();

                var outputJToken = (JObject)JToken.Parse(outputJsonString);
				outputVars["Result"] = outputJToken["result"];

				outputVars["Error"] = outputJToken["Error"];

			}
			catch (Exception e)
			{
				string errorMsg = e.Message;
				CORE.CORE core = (CORE.CORE)vars["__CORE__"];
				OmniusException.Log(e, OmniusLogSource.Nexus, core.Application, core.User);
				outputVars["Result"] = String.Empty;
				outputVars["Error"] = true;
			}
		}
	}
}
