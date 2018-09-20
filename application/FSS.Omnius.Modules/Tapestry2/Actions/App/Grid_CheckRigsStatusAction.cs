using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using Renci.SshNet;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(9664, "Grid: Check rig status")]
        public static void CheckRigsStatus(COREobject core, JArray InputJSON)
        {
            var db = core.Entitron;
            List<DBItem> tRigPlacement = db.Tabloid("RigPlacementSsh").Select().ToList();

            DBTable tableRigHistory = db.Table("RigHistory", false);
            
            foreach (var miner in InputJSON)
            {
                string host = miner["host"].ToObject<string>();
                string reason = miner["reason"].ToObject<string>();
                
                var row = tRigPlacement.FirstOrDefault(x => x["Rig_ip"].ToString() == host);
                string rigIp = host.Split(':')[0];
                
                switch (reason)
                {
                    case "miner_down":
                    case "rig_not_mining":
                    case "rig_partially_down":
                    case "gpu_stopped":
                    case "gpu_not_mining":
                        DBItem item = new DBItem(db, tableRigHistory);
                        item["RigIp"] = rigIp;
                        item["Owner"] = core.Context.Users.First(u => u.UserName.ToLower() == "system");
                        item["Subject"] = $"Restarted by autorepair sevice. Reason: {reason}";
                        tableRigHistory.Add(item);

                        RunSshCommand($"rigctl restart {rigIp}", row);
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
