using System;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3010, "Grid: Record Rig History")]
        public static void RecordRigHistory(COREobject core, JArray InputJSON)
        {
            DBConnection db = core.Entitron;
            DBTable table = db.Table("RigGraphHistory", false);
            
            // Record the same Timestamp for all rigs
            DateTime timestamp = DateTime.Now;

            foreach (var rig in InputJSON)
            {
                JObject rigObject = (JObject) rig;

                string rigName = rigObject.Property("name").Value.ToString();
                string rigStatus = rigObject.Property("status").Value.ToString();
                int rigEthHashrate = rigObject.Property("rig_eth_hashrate").ToObject<int>();
                int rigDcrHashrate = rigObject.Property("rig_dcr_hashrate").ToObject<int>();
                int rigZecHashrate = rigObject.Property("rig_zec_hashrate").ToObject<int>();
                int rigMusHashrate = rigObject.Property("rig_mus_hashrate").ToObject<int>();
                double rigXmrHashrate = rigObject.Property("rig_xmr_hashrate").ToObject<double>();

                JArray rigTempArray = (JArray) rigObject.Property("gpu_temp").Value;

                // Count average of rig temperatures
                double rigTempsSum = 0;
                for (int i = 0; i < rigTempArray.Count; i++)
                {
                    rigTempsSum += (int) rigTempArray[i];
                }
                double rigTempAvg = (double) rigTempsSum / rigTempArray.Count;

                // Create row for the rig
                DBItem item = new DBItem(db, table);
                item["Rig_name"] = rigName;
                item["Rig_avg_temp"] = System.Math.Round(rigTempAvg, 1);
                item["Rig_status"] = rigStatus;
                item["Rig_hashrate_eth"] = rigEthHashrate;
                item["Rig_hashrate_dcr"] = rigDcrHashrate;
                item["Rig_hashrate_zec"] = rigZecHashrate;
                item["Timestamp"] = timestamp;
                item["Rig_hashrate_mus"] = rigMusHashrate;
                item["Rig_hashrate_xmr"] = rigXmrHashrate;
                table.Add(item);
            }
        }
    }
}