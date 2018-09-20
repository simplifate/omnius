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
    /// <summary>
    /// Akce pro záznam aktuálního stavu rigu to tabulky historie RigGraphHistory.
    /// Z miners API zaznamená name (IP), status, ETH a DCR hashrate a průměr teplot rigu.
    /// Pro každý rig vytvoří samostatný řádek v tabulce RigGraphHistory s Timestampem spuštění vytváření záznamů.
    /// Bloku je nutno předřadit např. blok Call REST s připojenou WS: MinerAPI a result předat ve formě variable bloku Grid: Record Rig History.
    /// </summary>
    class RecordRigHistoryAction : Action
    {

        public override int Id
        {
            get
            {
                return 3010;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "InputJSON" };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid: Record Rig History";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] {};
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
            DBTable table = db.Table("RigGraphHistory", false);

            var miners = (JArray)vars["InputJSON"];

            // Record the same Timestamp for all rigs
            DateTime timestamp = DateTime.Now;

            foreach (var rig in miners)
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
                    rigTempsSum += rigTempArray[i].ToObject<string>() != "N/A" ? (int) rigTempArray[i] : 0;
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
            db.SaveChanges();
        }
    }
}
