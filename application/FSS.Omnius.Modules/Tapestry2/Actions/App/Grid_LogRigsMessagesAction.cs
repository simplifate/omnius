using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(300010, "Log Rigs Messages", "Result")]
        public static void LogRigsMessages(COREobject core, JArray CachedDataJsonArray, JArray MinerApiDataJsonArray, int max_temp)
        {
            DBConnection db = core.Entitron;
            DBTable tableRigHistory = db.Table("RigHistory", false);
            
            foreach (JObject rig in MinerApiDataJsonArray)
            {

                //if theres any rig with same name in cachedData, we gonna  check and log
                var oldMiner = CachedDataJsonArray.SingleOrDefault(r => r["name"].ToObject<string>() == rig["name"].ToObject<string>());
                if (oldMiner != null)
                {
                    string rigName = rig["name"].ToObject<string>();


                    //IF THE RIG IS IN RIGPLACEMENT TABLE,WE WILL LOG. OTHERWISE WE WONT!

                    //HwDown,
                    if (rig["reason"].ToObject<string>() == "hw_down" && oldMiner["reason"].ToObject<string>() != "hw_down")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Hardware is currently down");

                    }
                    //HwUp,
                    if (rig["reason"].ToObject<string>() != "hw_down" && oldMiner["reason"].ToObject<string>() == "hw_down")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Hardware is currently running");

                    }

                    //MinerDown
                    if (rig["reason"].ToObject<string>() == "miner_down" && oldMiner["reason"].ToObject<string>() != "miner_down")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Miner is currently down");

                    }
                    //MinerUp
                    if (rig["reason"].ToObject<string>() != "miner_down" && oldMiner["reason"].ToObject<string>() == "miner_down")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Miner is currently running");

                    }

                    //Rig not mining
                    if (rig["reason"].ToObject<string>() == "rig_not_mining" && oldMiner["reason"].ToObject<string>() != "rig_not_mining")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Rig is currently not mining");

                    }
                    //Rig is mining
                    if (rig["reason"].ToObject<string>() != "rig_not_mining" && oldMiner["reason"].ToObject<string>() == "rig_not_mining")
                    {
                        //log
                        CreateNewDBItemForLog(core, tableRigHistory, rigName, "Rig is currently mining");

                    }


                    for (int i = 0; i < rig["gpu_temp"].ToArray().Length; i++)
                    {
                        if (i >= oldMiner["gpu_temp"].ToArray().Length)
                            break;

                        //if gpu temp is over max
                        if (rig["gpu_temp"][i].ToObject<int>() > max_temp && oldMiner["gpu_temp"][i].ToObject<int>() <= max_temp)
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "GPU overheated");

                        }

                        //if gpu temp is < max
                        if (rig["gpu_temp"][i].ToObject<int>() < max_temp && oldMiner["gpu_temp"][i].ToObject<int>() >= max_temp)
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "GPU no longer overheated");

                        }


                    }

                    for (int i = 0; i < rig["gpu_eth_hr"].ToArray().Length; i++)
                    {
                        if (i >= oldMiner["gpu_eth_hr"].ToArray().Length)
                            break;

                        //if gpu hr is stopped
                        if (rig["gpu_eth_hr"][i].ToObject<string>() == "stopped" && oldMiner["gpu_eth_hr"][i].ToObject<string>() != "stopped")
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "GPU is currently stopped");

                        }

                        //if gpu hr is not stopped
                        if (rig["gpu_eth_hr"][i].ToObject<string>() != "stopped" && oldMiner["gpu_eth_hr"][i].ToObject<string>() == "stopped")
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "GPU is currently running");

                        }

                        //if gpu hr is 0
                        if (rig["gpu_eth_hr"][i].ToObject<string>() == "0" && oldMiner["gpu_eth_hr"][i].ToObject<string>() != "0")
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "ETH hashrate is 0");

                        }

                        //if gpu hr is not 0
                        if (rig["gpu_eth_hr"][i].ToObject<string>() != "0" && oldMiner["gpu_eth_hr"][i].ToObject<string>() == "0")
                        {
                            //log
                            CreateNewDBItemForLog(core, tableRigHistory, rigName, "ETH hashrate is greater than 0");

                        }
                    }
                }
            }

            db.SaveChanges();
        }
        private static void CreateNewDBItemForLog(COREobject core, DBTable table, object rigName, object logMessage)
        {
            DBItem item = new DBItem(core.Entitron, table);
            item["RigIp"] = rigName;
            item["Owner"] = core.Context.Users.FirstOrDefault(u => u.UserName.ToLower() == "system").Id;
            item["Subject"] = logMessage;
            item["Timestamp"] = DateTime.Now;
            table.Add(item);
        }
    }
}
