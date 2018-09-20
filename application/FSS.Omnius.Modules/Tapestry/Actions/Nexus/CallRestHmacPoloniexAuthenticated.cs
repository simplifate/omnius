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
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{

    [NexusRepository]
    public class CallRestHmacPoloniexAuthenticatedAction : Action
    {
        public override int Id
        {
            get
            {
                return 3193;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Url", "Params", "ApiKey", "ApiSecret" };
            }
        }

        public override string Name
        {
            get
            {
                return "Call REST HMAC POLONIEX AUTHENTICATED";
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
            string Url = vars["Url"].ToString();
            string Params = vars["Params"].ToString() + nonce();
            string ApiKey = vars["ApiKey"].ToString();
            string ApiSecret = vars["ApiSecret"].ToString();

            string result =  SendPrivateApiRequest(Url, Params,ApiKey, ApiSecret);
        }
        private string SendPrivateApiRequest(string privUrl, string myParam,string ApiKey,string ApiSecret)
        {
            var c_nonce = nonce();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(privUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("Key", ApiKey);
            httpWebRequest.Headers.Add("Sign", GenerateHmac(myParam  + c_nonce, ApiKey, ApiSecret));

            var postData =  myParam + c_nonce;
            var data = Encoding.ASCII.GetBytes(postData);

            httpWebRequest.ContentLength = data.Length;
            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

          

            var response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string outputJsonString = responseReader.ReadToEnd();
            return outputJsonString;

        
        }
       
     
        public string GenerateHmac(string msg,string ApiKey,string ApiSecret) {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(ApiSecret));
            var messagebyte = Encoding.ASCII.GetBytes(msg);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }

        public string nonce() {
            return DateTime.Now.Ticks.ToString();
        }
    }
}
