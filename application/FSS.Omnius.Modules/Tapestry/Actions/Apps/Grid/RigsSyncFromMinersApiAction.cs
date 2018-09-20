using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class RigsSyncFromMinersApiAction : Action
    {

        public override int Id
        {
            get
            {
                return 3117;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "InputJSON"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Rigs sync";
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

            JObject minersData = (JObject)vars["InputJSON"];
            JArray miners = (JArray)minersData["miners"];
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
