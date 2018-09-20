using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using Renci.SshNet;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class CheckRigsStatusAction : Action
    {

        public override int Id
        {
            get
            {
                return 9664;
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
                return "Grid:Check rig status";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[] { };
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
            var db = COREobject.i.Entitron;
            List<DBItem> tRigPlacement = db.Select("RigPlacementSsh").ToList();
            JArray miners = (JArray)vars["InputJSON"];

            DBTable tableRigHistory = db.Table("RigHistory", false);


            foreach (var miner in miners)
            {
                string name = miner["name"].ToObject<string>();
                string reason = miner["reason"].ToObject<string>();


                var row = tRigPlacement.FirstOrDefault(x => x["Rig_name"].ToString() == name);

                string command = "";

                switch (reason)
                {
                    case "miner_down":
                    case "rig_not_mining":
                    case "rig_partially_down":
                    case "gpu_stopped":
                    case "gpu_not_mining":
                        DBItem item = new DBItem(db, tableRigHistory);
                        item["RigIp"] = name;
                        item["Owner"] = 79; //system user id  = 79
                        item["Subject"] = $"Restarted by autorepair sevice. Reason: {reason}";
                        item["Timestamp"] = DateTime.Now;
                        tableRigHistory.Add(item);
                        command = $"rigctl restart {name}";

                        RunSshCommand(command, row);
                        break;
                    default:
                        break;
                }
            }
            db.SaveChanges();
        }

        private static void RunSshCommand(string command, DBItem row)
        {
            string password = (string)row["Password"];
            string hostname = (string)row["Hostname"];
            string userName = (string)row["Username"];
            int port = (int)row["Port"];
            using (var client = new SshClient(hostname, port, userName, password))
            {
                client.Connect();
                var result = client.RunCommand(command);
                client.Disconnect();
            }
        }
    }
}
