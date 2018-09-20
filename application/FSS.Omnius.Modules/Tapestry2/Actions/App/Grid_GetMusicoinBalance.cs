using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(30213, "Get Musicoin Balance Action", "Result")]
        public static void GetMusicoinBalance(COREobject core, string Address, string ClientId, string ClientSecret)
        {
            string endpointPath = "http://api.musicoin.org/artist/profile/" + Address;
           
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpointPath);
            var headers = new Dictionary<string, string>
            {
                {"clientId", ClientId},
                {"clientSecret", ClientSecret}
            };
            if (httpWebRequest == null)
                throw new Exception("Non HTTP WebRequest");
            foreach (var a in headers)
            {
                httpWebRequest.Headers.Add(a.Key, a.Value);
            }

            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";

            var response = httpWebRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            string outputJsonString = responseReader.ReadToEnd();
        }
    }
}
