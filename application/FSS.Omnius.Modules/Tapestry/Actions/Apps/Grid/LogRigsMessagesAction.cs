using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class LogRigsMessages : Action
    {
        private DBConnection _db;
        private List<string> ListOfRigsIp = new List<string>();

        public override int Id
        {
            get
            {
                return 300010;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "CachedDataJsonArray", "MinerApiDataJsonArray", "max_temp"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Log Rigs Messages";
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

        private void LogAndSetLastUpdateTime(Dictionary<string, object> vars, DBTable table, object rigName, object logMessage,string rigIp)
        {
            DBItem item = new DBItem(_db, table);
            item["RigIp"] = rigName;
            item["Owner"] = 79; //system user id  = 79
            item["Subject"] = logMessage;
            item["Timestamp"] = DateTime.Now;
            table.Add(item);
            ListOfRigsIp.Add(rigIp);
            
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            try
            {
                _db = COREobject.i.Entitron;
                JArray cachedLogData = (JArray)vars["CachedDataJsonArray"];
                JArray minerApiData = (JArray)vars["MinerApiDataJsonArray"];

                DBTable tableRigHistory = _db.Table("RigHistory", false);
                DBTable tableRigPlacement = _db.Table("RigPlacement", false);
                List<DBItem> rigPlacementList = tableRigPlacement.Select().ToList();

                foreach (JToken rig in minerApiData)
                {
                    var rigObj = rig as JObject;
                    if (rigObj == null)
                    {
                        continue;
                    }
                    //if theres any rig with same name in cachedData, we gonna  check and log
                    var oldMiner = cachedLogData.SingleOrDefault(r => r["name"].ToObject<string>() == rigObj["name"].ToObject<string>());
                    if (oldMiner != null)
                    {
                        string rigName = rigObj["name"].ToObject<string>();


                        //IF THE RIG IS IN RIGPLACEMENT TABLE,WE WILL LOG. OTHERWISE WE WONT!

                        //HwDown,
                        if (rigObj["reason"].ToObject<string>() == "hw_down" && oldMiner["reason"].ToObject<string>() != "hw_down")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Hardware is currently down", oldMiner["host"].ToObject<string>());
                        }
                        //HwUp,
                        if (rigObj["reason"].ToObject<string>() != "hw_down" && oldMiner["reason"].ToObject<string>() == "hw_down")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Hardware is currently running", oldMiner["host"].ToObject<string>());
                        }

                        //MinerDown
                        if (rigObj["reason"].ToObject<string>() == "miner_down" && oldMiner["reason"].ToObject<string>() != "miner_down")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Miner is currently down", oldMiner["host"].ToObject<string>());
                        }
                        //MinerUp
                        if (rigObj["reason"].ToObject<string>() != "miner_down" && oldMiner["reason"].ToObject<string>() == "miner_down")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Miner is currently running", oldMiner["host"].ToObject<string>());
                        }

                        //Rig not mining
                        if (rigObj["reason"].ToObject<string>() == "rig_not_mining" && oldMiner["reason"].ToObject<string>() != "rig_not_mining")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Rig is currently not mining", oldMiner["host"].ToObject<string>());
                        }
                        //Rig is mining
                        if (rigObj["reason"].ToObject<string>() != "rig_not_mining" && oldMiner["reason"].ToObject<string>() == "rig_not_mining")
                        {
                            //log
                            LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "Rig is currently mining", oldMiner["host"].ToObject<string>());
                        }


                        for (int i = 0; i < rigObj["gpu_temp"].ToArray().Length; i++)
                        {
                            if (i >= oldMiner["gpu_temp"].ToArray().Length)
                                break;

                            //if gpu temp is over max
                            if (!String.IsNullOrEmpty(rigObj["gpu_temp"][i].ToObject<string>()))
                            {
                                int result;
                                int resultOld;
                                bool parseRigTemp = int.TryParse(rigObj["gpu_temp"][i].ToObject<string>(), out result);
                                bool parseOldRigTemp = int.TryParse(oldMiner["gpu_temp"][i].ToObject<string>(), out resultOld);
                                if (parseRigTemp && parseOldRigTemp)
                                {
                                    if (result > (int)vars["max_temp"] && resultOld <= (int)vars["max_temp"])
                                    {
                                        //log
                                        LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "GPU overheated", oldMiner["host"].ToObject<string>());
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(rigObj["gpu_temp"][i].ToObject<string>()) )
                            {
                                //if gpu temp is < max
                                int result;
                                int resultOld;
                                bool parseRigTemp = int.TryParse(rigObj["gpu_temp"][i].ToObject<string>(), out result);
                                bool parseOldRigTemp = int.TryParse(oldMiner["gpu_temp"][i].ToObject<string>(), out resultOld);
                                if (parseRigTemp && parseOldRigTemp)
                                {
                                    if (result < (int)vars["max_temp"] && resultOld >= (int)vars["max_temp"])
                                    {
                                        //log
                                        LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "GPU no longer overheated", oldMiner["host"].ToObject<string>());
                                    }
                                }
                            }


                        }

                        for (int i = 0; i < rigObj["gpu_eth_hr"].ToArray().Length; i++)
                        {
                            if (i >= oldMiner["gpu_eth_hr"].ToArray().Length)
                                break;

                            //if gpu hr is stopped
                            if (rigObj["gpu_eth_hr"][i].ToObject<string>() == "stopped" && oldMiner["gpu_eth_hr"][i].ToObject<string>() != "stopped")
                            {
                                //log
                                LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "GPU is currently stopped", oldMiner["host"].ToObject<string>());
                            }

                            //if gpu hr is not stopped
                            if (rigObj["gpu_eth_hr"][i].ToObject<string>() != "stopped" && oldMiner["gpu_eth_hr"][i].ToObject<string>() == "stopped")
                            {
                                //log
                                LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "GPU is currently running", oldMiner["host"].ToObject<string>());
                            }

                            //if gpu hr is 0
                            if (rigObj["gpu_eth_hr"][i].ToObject<string>() == "0" && oldMiner["gpu_eth_hr"][i].ToObject<string>() != "0")
                            {
                                //log
                                LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "ETH hashrate is 0", oldMiner["host"].ToObject<string>());
                            }

                            //if gpu hr is not 0
                            if (rigObj["gpu_eth_hr"][i].ToObject<string>() != "0" && oldMiner["gpu_eth_hr"][i].ToObject<string>() == "0")
                            {
                                //log
                                LogAndSetLastUpdateTime(vars, tableRigHistory, rigName, "ETH hashrate is greater than 0", oldMiner["host"].ToObject<string>());
                            }
                        }
                    }
                }


                //NOw when we already have the list of rigs IP, we gonna update the last update time for time
                foreach (string rigsIp in ListOfRigsIp)
                {
                    //find this rig in placement
                    DBItem foundRig = rigPlacementList.SingleOrDefault(r => r["Rig_ip"].ToString() == rigsIp);
                    if (foundRig != null)
                    {
                        foundRig["Status_changed"] = DateTime.UtcNow;
                        tableRigPlacement.Update(foundRig, (int)foundRig["id"]);
                    }
                    else
                    {
                        Watchtower.OmniusWarning.Log("rig is not in placement: " + rigsIp, Watchtower.OmniusLogSource.none,null,null);

                    }
                }

                _db.SaveChanges();
            }

            catch (Exception e) {
                Watchtower.OmniusWarning.Log("Log rig message action error: "+ e.Message, Watchtower.OmniusLogSource.none,null, null);

            }
        }
            
    }
}
