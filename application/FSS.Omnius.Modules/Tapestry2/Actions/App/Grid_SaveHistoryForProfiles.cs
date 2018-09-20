using System;
using System.Net;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3007, "Grid: Save History For Profiles")]
        public static void SaveHistoryForProfiles(COREobject core)
        {
            DBConnection db = core.Entitron;

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
