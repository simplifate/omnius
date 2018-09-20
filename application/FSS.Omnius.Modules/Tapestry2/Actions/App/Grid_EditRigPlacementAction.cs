using FSS.Omnius.Modules.CORE;
using System;   
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(212, "Edit Rig Placement", "Result")]
        public static bool EditRigPlacement(COREobject core, JToken RigPlacementDataJSON)
        {
            DBConnection db = core.Entitron;

            var rigPlacementTable = db.Table("RigPlacement", false);
            var shelfsTable = db.Table("Shelfs", false);

            var rigPlacementItems = RigPlacementDataJSON.SelectToken("$.data.item");
            var unassignedRigs = RigPlacementDataJSON.SelectToken("$.unassignedRigs.item");
            var shelfsItems = RigPlacementDataJSON.SelectToken("$.shelfs.item");

            List<DBItem> shelfsSelect = shelfsTable.Select().ToList();

            if(((JArray)RigPlacementDataJSON["removedShelfs"]).Count > 0) {
                foreach(JToken rs in (JArray)RigPlacementDataJSON["removedShelfs"]) {
                    int rsId = ((JValue)rs).ToObject<int>();
                    DBItem row = shelfsSelect.SingleOrDefault(r => (int)r["id"] == rsId);
                    if(row != null) {
                        shelfsTable.Delete(row);
                    }
                }
            }

            foreach(JToken item in shelfsItems) 
            {
                if(item["id"].ToObject<object>() != null) 
                {
                    DBItem row = shelfsSelect.SingleOrDefault(r => (int)r["id"] == (int)item["id"]);
                    row["name"] = (string)item["name"];
                    row["cols_names"] = item["cols_names"].ToString();
                    row["rows_names"] = item["rows_names"].ToString();
                    row["shelf_index"] = (int)item["shelf_index"];

                    shelfsTable.Update(row, (int)row["id"]);
                }
                else 
                {
                    DBItem row = new DBItem(db, shelfsTable);
                    row["height"] = (int)item["height"];
                    row["width"] = (int)item["width"];
                    row["both_sides"] = (bool)item["both_sides"];
                    row["container_id"] = core.ModelId;
                    row["name"] = (string)item["name"];
                    row["cols_names"] = item["cols_names"].ToString();
                    row["rows_names"] = item["rows_names"].ToString();
                    row["shelf_index"] = (int)item["shelf_index"];

                    shelfsTable.Add(row);
                }
            }

            List<DBItem> tableSelect = rigPlacementTable.Select().ToList();
            foreach (JToken item in rigPlacementItems)
            {
                DBItem row = tableSelect.SingleOrDefault(r => (string)r["Rig_ip"] == item["Rig_ip"].ToString());

                row["X"] = item["X"].ToObject<int>();
                row["Y"] = item["Y"].ToObject<int>();
                row["Side"] = item["Side"].ToObject<string>();
                row["Shelf_id"] = item["Shelf_id"].ToObject<int>();

                rigPlacementTable.Update(row, (int)row["id"]);
            }
            foreach (JToken item in unassignedRigs)
            {
                DBItem row = tableSelect.SingleOrDefault(r => (string)r["Rig_ip"] == item["Rig_ip"].ToString());
                row["X"] = -1;
                row["Y"] = -1;
                row["Shelf_id"] = null;
                rigPlacementTable.Update(row, (int)row["id"]);
            }

            db.SaveChanges();
            return true;
        }
    }
}
