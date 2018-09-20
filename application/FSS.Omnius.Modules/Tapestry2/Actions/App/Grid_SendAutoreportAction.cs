using System;
using System.Net;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Tapestry;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(30011, "Grid:SendAutoreportAction")]
        public static void SendAutoreport(COREobject core)
        {
            DBConnection db = core.Entitron;

            string appName = "Grid";
            string blockName = "CurrencyEmail";
            string hostname = TapestryUtils.GetServerHostName();
            
            var ae = db.Select("AssignedEmails", false).Where(c => c.Column("messageId").Equal("2")).ToList();
            foreach (var email in ae)
            {
                var modelId = email["userId"];
                var targetUrl = $"{hostname}/{appName}/{blockName}/Get?modelId={modelId}&User=scheduler&Password=194GsQwd/AgB4ZZnf_uF";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //request.GetResponseAsync();  // Bylo by mozne take pouzit (dobehne pridani zaznamu do MiningHistory asynchronne)
                //var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();  // Neni potreba
            }
        }
    }
}
