using FSS.Omnius.Modules.CORE;
using System;   
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class EditRigPlacementAction : Action
    {
        public override int Id
        {
            get
            {
                return 212;
            }
        }
        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }
        public override string[] InputVar
        {
            get
            {
                return new string[] { "RigPlacementDataJSON" };
            }
        }

        public override string Name
        {
            get
            {
                return "Edit Rig Placement";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { "Result" };
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            if (!vars.ContainsKey("RigPlacementDataJSON"))
            {
                throw new Exception("Tapestry action Edit Rig Placement: RigPlacementDataJSON is required!");
            }

            COREobject core = COREobject.i;
            DBConnection db = core.Entitron;
            JToken data = (JToken)vars["RigPlacementDataJSON"];

            var rigPlacementTable = db.Table("RigPlacement", false);
            var shelfsTable = db.Table("Shelfs", false);

            var rigPlacementItems = data.SelectToken("$.data.item");
            var unassignedRigs = data.SelectToken("$.unassignedRigs.item");
            var shelfsItems = data.SelectToken("$.shelfs.item");

            List<DBItem> shelfsSelect = shelfsTable.Select().ToList();

            if(((JArray)data["removedShelfs"]).Count > 0) {
                foreach(JToken rs in (JArray)data["removedShelfs"]) {
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
                    row["container_id"] = (int)vars["__ModelId__"];
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
                DBItem row = tableSelect.SingleOrDefault(r => (string)r["Rig_name"] == item["Rig_name"].ToString());

                row["X"] = item["X"].ToObject<int>();
                row["Y"] = item["Y"].ToObject<int>();
                row["Side"] = item["Side"].ToObject<string>();
                row["Shelf_id"] = item["Shelf_id"].ToObject<int>();

                rigPlacementTable.Update(row, (int)row["id"]);
            }
            foreach (JToken item in unassignedRigs)
            {
                DBItem row = tableSelect.SingleOrDefault(r => (string)r["Rig_name"] == item["Rig_name"].ToString());
                row["X"] = -1;
                row["Y"] = -1;
                row["Shelf_id"] = null;
                rigPlacementTable.Update(row, (int)row["id"]);
            }

            db.SaveChanges();
            outputVars["Result"] = true;
        }
    }
}
