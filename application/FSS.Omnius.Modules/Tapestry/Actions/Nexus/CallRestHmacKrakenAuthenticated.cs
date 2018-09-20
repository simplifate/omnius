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
    public class CallRestHmacKrakenAuthenticatedAction : Action
    {
        public override int Id
        {
            get
            {
                return 3194;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Url", "ApiKey", "ApiSecret","ApiMethod","ApiVersion" };
            }
        }

        public override string Name
        {
            get
            {
                return "Call REST HMAC KRAKEN AUTHENTICATED";
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
            //string Url = vars["Url"].ToString();
            //string Path = vars["Path"].ToString();
            //string ApiKey = vars["ApiKey"].ToString();
            //string ApiSecret = vars["ApiSecret"].ToString();

            string Url = vars["Url"].ToString();
            string Path = "/" + vars["ApiVersion"].ToString() + "/private/" + vars["ApiMethod"].ToString(); // The path like "/0/private/Balance";
            string ApiKey = vars["ApiKey"].ToString();
            string ApiSecret = vars["ApiSecret"].ToString();

            string result = SendPrivateApiRequest(Url, Path, ApiKey, ApiSecret);
        }
        private string SendPrivateApiRequest(string baseUrl, string Path, string ApiKey, string ApiSecret)
        {
            var c_nonce = nonce();
            String postData = "nonce=" + c_nonce;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("API-Key", ApiKey);
            httpWebRequest.Headers.Add("API-Sign", GenerateHmac(Path + hash256(c_nonce + postData) , encode64(ApiSecret)));

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

        public string hash256(string strData)
        {
            var message = Encoding.ASCII.GetBytes(strData);
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";

            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        public string encode64(string strData)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(strData);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public string GenerateHmac(string message, string ApiSecret)
        {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(ApiSecret));
            var messagebyte = Encoding.ASCII.GetBytes(message);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }

        public string nonce()
        {
            return DateTime.Now.Ticks.ToString();
        }
    }
}
