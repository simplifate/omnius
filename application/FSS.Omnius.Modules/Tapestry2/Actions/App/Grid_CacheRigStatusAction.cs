using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(31207, "Grid:Cache rig status")]
        public static void CachRigStatus(COREobject core, JArray InputJSON, int MaxTemp = 75)
        {
            DBConnection db = core.Entitron;
            
            DBTable tRigPlacement = db.Table("RigPlacement");

            foreach (var rig in tRigPlacement.Select().ToList())
            {
                JToken miner = InputJSON.FirstOrDefault(m => m["name"].ToString() == rig["Rig_name"].ToString());

                //nastavit status
                rig["Status"] = miner["reason"].ToString();

                if (miner != null && miner["status"].ToString() == "down")
                {
                    //if (miner["error"].ToString().Contains("Error: connect ETIMEDOUT") || miner["error"].ToString().Contains("Error: connect EHOSTUNREACH"))
                    //{
                    //    rig["Status"] = "HW_Down";
                    //}
                    //else if (miner["error"].ToString().Contains("Error: connect ECONNREFUSED"))
                    //{
                    //    rig["Status"] = "MINER_Down";
                    //}
                    //else
                    //{
                    //    rig["Status"] = miner["reason"] != null ? miner["reason"].ToString() : "unknown";
                    //}

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
                            if (gpuTemp > MaxTemp)
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
                        for (int i = 0; i < miner["gpu_eth_hr"].Count(); i++)
                        {
                            var ethHR = ((JValue)miner["gpu_eth_hr"][i]).ToObject<string>();
                            var gpuTempString = ((JValue)miner["gpu_temp"][i]).ToObject<string>();
                            int gpuTemp = int.Parse(gpuTempString);
                            if (ethHR == "0" || ethHR == "stopped" || gpuTemp > MaxTemp)
                            {
                                gpuIndexes += $"{i},";
                            }
                        }
                    }
                     //Select z View. V tom view obsahuje column IP + gpus.
                    Select select = db.Select("ViewRigsGpusCount", false);
                    var gpuCount = 0;
                    var rigItem = select.ToList().SingleOrDefault(x => x["Rig_ip"] == rig["Rig_name"]);

                    if (rigItem != null)
                    {
                        gpuCount = (int)(rigItem["gpus"]); //get rigs gpu count

                        if (miner["gpu_temp"].Count() < gpuCount)
                            //set status bad riser
                            rig["Status"] = "GPU_Bad_riser";
                    }

                }
                else
                {
                    rig["Status"] = "OK";

                }

                tRigPlacement.Update(rig, (int)rig["id"]);
            }
            db.SaveChanges();
        }
    }
}
