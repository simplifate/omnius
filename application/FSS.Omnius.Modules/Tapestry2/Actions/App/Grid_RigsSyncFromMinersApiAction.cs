using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3117, "Grid: Rigs sync")]
        public static void RigsSyncFromMinersApi(COREobject core, JObject InputJSON)
        {
            DBConnection db = core.Entitron;
            
            JArray miners = (JArray)InputJSON["miners"];
            var dbRigPlacement = db.Table("RigPlacement");
            var listOfRigsId = dbRigPlacement.Select("Rig_name").ToList();
            foreach (var miner in miners) {
              
                //if rig id from miner is not in the database,we will add it
                if (!listOfRigsId.Any(i => i["Rig_name"].ToString() == miner["name"].ToString())){
                    DBItem item = new DBItem(db, dbRigPlacement);
                    item["Rig_ip"] = ((JValue)miner["host"]).ToObject<String>();
                    item["Rig_name"] = ((JValue)miner["name"]).ToObject<String>();
                    dbRigPlacement.Add(item);
                }
            }
            db.SaveChanges();
        }
    }
}
