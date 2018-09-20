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

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [ImageRepository]
    public class DownloadAction : Action
    {
        public override int Id
        {
            get
            {
                return 7000;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "s$Url" };
            }
        }

        public override string Name
        {
            get
            {
                return "Image: Download";
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
                string url = (string)vars["Url"];
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                
                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();

                if (response.ContentType.StartsWith("image")) {
                    byte[] rawData = ReadAllBytes(responseStream);

                    outputVars["Result"] = rawData;
                    outputVars["Error"] = false;
                }
                else {
                    Watchtower.OmniusInfo.Log($"{Name}: Requested file \"{url}\" is not image.");
                    outputVars["Result"] = new byte[] { };
                    outputVars["Error"] = true;
                }
            }
            catch (Exception e)
            {
                Watchtower.OmniusInfo.Log(e.Message);
                outputVars["Result"] = new byte[] { };
                outputVars["Error"] = true;
            }
        }
        
        private byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream()) {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
