using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class SaveHistoryForProfilesAction : Action
    {

        public override int Id
        {
            get
            {
                return 3007;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Save History For Profiles";
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
            DBConnection db = COREobject.i.Entitron;

            string appName = "Grid";
            string blockName = "SaveHistory";
            string hostname = TapestryUtils.GetServerHostName();
            
            var ids = db.Select("Profile", false, "id").ToList();
            foreach (var id in ids)
            {
                var modelId = id["id"];
                var targetUrl = $"{hostname}/{appName}/{blockName}/Get?modelId={modelId}&User=scheduler&Password=194GsQwd/AgB4ZZnf_uF&button=INIT";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //request.GetResponseAsync();  // Bylo by mozne take pouzit (dobehne pridani zaznamu do MiningHistory asynchronne)
                //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();  // Neni potreba
            }
        }
    }
}
