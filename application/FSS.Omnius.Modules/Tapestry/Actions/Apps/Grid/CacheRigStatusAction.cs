using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    class CachRigStatusAction : Action
    {

        public override int Id
        {
            get
            {
                return 31207;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "InputJSON", "?MaxTemp"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid:Cache rig status";
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
            DBConnection db = COREobject.i.Entitron;

            int maxTemp = vars.ContainsKey("MaxTemp") ? (int)vars["MaxTemp"] : 75;
            DBTable tRigPlacement = db.Table("RigPlacement");
            var rigPlacements = tRigPlacement.Select().ToList();
            JArray miners = (JArray)vars["InputJSON"];

            //Select z View. V tom view obsahuje column IP + gpus.
            Select select = db.Select("ViewRigsGpusCount", false);
            var rigGpus = select.ToList();

            foreach (var rig in rigPlacements)
            {
                JToken miner = miners.FirstOrDefault(m => m["name"].ToString() == rig["Rig_name"].ToString());
                rig["gpu_status_indexes"] = "";

                //nastavit status
                if (miner != null && miner["status"].ToString() == "down")
                {
                    rig["Status"] = !string.IsNullOrEmpty(miner["reason"].ToString()) ? miner["reason"].ToString() : "down";
                }

                else if (miner != null && miner["status"].ToString() == "warning")
                {
                    if (miner["reason"].ToString().Contains("cpu_not_mining"))
                    {
                        rig["CPUError"] = "CPU_Not_Mining";
                    }
                    else
                    {
                        rig["CPUError"] = "";
                    }
                    if (miner["reason"].ToString().Contains("gpu_not_mining"))
                    {
                        //rig["Status"] = "GPU_Not_Mining";
                        string gpuIndexes = "";
                        for (int i = 0; i < miner["gpu_eth_hr"].Count(); i++)
                        {
                            var ethHR = ((JValue)miner["gpu_eth_hr"][i]).ToObject<string>();
                            if (ethHR == "0")
                            {
                                gpuIndexes += $"{i},";
                            }
                        }
                        if (gpuIndexes != "")
                            gpuIndexes = gpuIndexes.Substring(0, gpuIndexes.Length - 1);
                        rig["gpu_status_indexes"] = gpuIndexes;
                    }
                    else if (miner["reason"].ToString().Contains("gpu_overheating"))
                    {
                        //rig["Status"] = "GPU_Overheating";
                        string gpuIndexes = "";
                        for (int i = 0; i < miner["gpu_temp"].Count(); i++)
                        {
                            var gpuTempString = ((JValue)miner["gpu_temp"][i]).ToObject<string>();
                            int gpuTemp = int.Parse(gpuTempString);
                            if (gpuTemp > maxTemp)
                            {
                                gpuIndexes += $"{i},";
                            }
                        }
                        if (gpuIndexes != "")
                            gpuIndexes = gpuIndexes.Substring(0, gpuIndexes.Length - 1);
                        rig["gpu_status_indexes"] = gpuIndexes;
                    }
                    else if (miner["reason"].ToString().Contains("gpu_stopped"))
                    {
                        //rig["Status"] = "GPU_Stopped";
                        string gpuIndexes = "";
                        for (int i = 0; i < miner["gpu_eth_hr"].Count(); i++)
                        {
                            var ethHR = ((JValue)miner["gpu_eth_hr"][i]).ToObject<string>();
                            if (ethHR == "stopped")
                            {
                                gpuIndexes += $"{i},";
                            }
                        }
                        if (gpuIndexes != "")
                            gpuIndexes = gpuIndexes.Substring(0, gpuIndexes.Length - 1);
                        rig["gpu_status_indexes"] = gpuIndexes;
                    }
                    else
                    {
                        string gpuIndexes = "";
                        //rig["Status"] = "GPU_Bad_riser";
                        for (int i = 0; i < miner["gpu_temp"].Count(); i++)
                        {
                            var ethHR = ((JValue)miner["gpu_eth_hr"][i]).ToObject<string>();
                            var gpuTempString = ((JValue)miner["gpu_temp"][i]).ToObject<string>();
                            int gpuTemp = int.Parse(gpuTempString);
                            if (ethHR == "0" || ethHR == "stopped" || gpuTemp > maxTemp)
                            {
                                gpuIndexes += $"{i},";
                            }
                        }
                    }
                }
                else
                {
                    rig["Status"] = "OK";
                }

                if (miner != null && miner["status"].ToString() != "down")
                {
                    //var rigItem = select.Where(x => x.Column("Rig_ip").Equal(rig["Rig_name"])).ToList().SingleOrDefault();
                    DBItem rigItem = rigGpus.Where(x => x["Rig_ip"].ToString() == rig["Rig_ip"].ToString()).SingleOrDefault();

                    if (rigItem != null)
                    {
                        int gpuCount = (int)(rigItem["gpus"]); //get rigs gpu count

                        if (miner["gpu_temp"].Count() < gpuCount)
                        {
                            rig["Status"] = "bad_riser";
                            rig["gpu_status_indexes"] = "";
                        }
                    }
                }

                tRigPlacement.Update(rig, (int)rig["id"]);
            }
            db.SaveChanges();
        }
    }
}
