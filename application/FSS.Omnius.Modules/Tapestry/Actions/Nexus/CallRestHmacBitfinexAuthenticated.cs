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

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
 
    [NexusRepository]
    public class CallRestHmacBitfinexAuthenticatedAction : Action
    {
        public override int Id
        {
            get
            {
                return 3192;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "Url","ParamsDict","ApiKey", "ApiSecret" };
            }
        }

        public override string Name
        {
            get
            {
                return "Call REST HMAC BITFINEX AUTHENTICATED";
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
            string ApiKey = vars["ApiKey"].ToString();
            string ApiSecret = vars["ApiSecret"].ToString();

            string payload = vars["ParamsDict"].ToString();
            string payload64 = EncodeTo64(payload);

            
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(ApiSecret);
                HMACSHA384 hmacsha384 = new HMACSHA384(keyByte);
                byte[] messageBytes = encoding.GetBytes(payload64);
                byte[] hashmessage = hmacsha384.ComputeHash(messageBytes);
                string signature = ByteToString(hashmessage);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            var headers = new Dictionary<string, string>
            {
                {"X-BFX-APIKEY",ApiKey},
                {"X-BFX-PAYLOAD",payload},
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

        public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
       
        public string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }
}
