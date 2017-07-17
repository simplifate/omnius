using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Tapestry.Actions.Entitron;
using FSS.Omnius.Modules.Watchtower;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
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

		private const string WsName = "WSName";
		private const string Method = "Method";
		private const string Params = "Params";
		private const string Endpoint = "Endpoint";

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
				return new string[] { WsName, Method, Params, Endpoint };
			}
		}

		public override string Name
		{
			get
			{
				return "Call JSON RPC";
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
				string wsName = (string) vars[WsName];
				string method = (string) vars[Method];
				string parameters = (string) vars[Params];
				string endpoint = (string) vars[Endpoint];

				// vezmu uri
				CORE.CORE core = (CORE.CORE) vars["__CORE__"];
				var context = DBEntities.appInstance(core.Entitron.Application);
				var service = context.WSs.First(c => c.Name == wsName);

				string endpointPath = string.Format("{0}/{1}?{2}", service.REST_Base_Url.TrimEnd('/'), endpoint.Trim('/'));

				// Create request
				var httpWebRequest = (HttpWebRequest) WebRequest.Create(endpointPath);
				httpWebRequest.Method = method.ToUpper();
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Accept = "application/json";

				// Build inputJson
				// Example form of the request: {"jsonrpc": "2.0", "method": "subtract", "params": {"minuend": 42, "subtrahend": 23}, "id": 3}
				string inputJson =
					$"{{\"jsonrpc\": \"2.0\", \"method\": {method}, \"params\": {parameters}, \"id\": {GenerateJsonId()}";
				byte[] postJsonBytes = Encoding.UTF8.GetBytes(inputJson);
				httpWebRequest.ContentLength = postJsonBytes.Length;
				Stream requestStream = httpWebRequest.GetRequestStream();
				requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);

				// Example form of the response {"jsonrpc": "2.0", "result": 19, "id": 3}
				var response = httpWebRequest.GetResponse();
				Stream responseStream = response.GetResponseStream();
				StreamReader responseReader = new StreamReader(responseStream);
				string outputJsonString = responseReader.ReadToEnd();

				var outputJToken = (JObject) JToken.Parse(outputJsonString);
				outputVars["Result"] = outputJToken["result"];

				outputVars["Error"] = outputJToken["Error"];

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
	}
}
