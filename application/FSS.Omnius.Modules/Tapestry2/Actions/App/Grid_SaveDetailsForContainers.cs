using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(3008, "Grid: Save Details For Containers")]
        public static void SaveDetailsForContainers(COREobject core, JArray InputJSON)
        {
            /// INIT
            DBConnection db = core.Entitron;
            Dictionary<Tuple<int, int>, DBItem> locationProfiles = new Dictionary<Tuple<int, int>, DBItem>();
            // source
            double costPercent = (double)db.Select("PlatformSettings", false, "service_cost_perc").First()["service_cost_perc"];
            double profitPercent = 100 - costPercent;
            DBTable tContainers = db.Table("Container");
            DBTable tRigPlacement = db.Table("RigPlacement");
            DBTable tWallets = db.Table("Wallets");
            Tabloid tUsdRevenueByWallet = db.Tabloid("CurrencyHistoryByWallet");
            // target
            DBTable tContainerProfile = db.Table("ProfileContainerCache");
            DBTable tLocationProfile = db.Table("ProfileLocationCache");
            double totalActiveMinerCount = Convert.ToDouble(InputJSON.Where(m => m["status"].ToString() == "up" || m["status"].ToString() == "warning").ToList().Count);
            Dictionary<int, Dictionary<string, double>> locationHashrate = new Dictionary<int, Dictionary<string, double>>(); // quick fix before presentation

            //// Containers ////
            // make new ListNewContainerProfile
            List<DBItem> tContainerProfileNew = new List<DBItem>();
            var tContainerProfileOld = tContainerProfile.Select().ToList();
            // for all combinations of profiles & containers
            foreach (DBItem combPlacement in tRigPlacement.Select("Profile_id", "Container_id").Group(ESqlFunction.none, null, "Profile_id", "Container_id").ToList())
            {
                int containerId = Convert.ToInt32(combPlacement["Container_id"] == DBNull.Value ? 0 : combPlacement["Container_id"]);
                int profileId = Convert.ToInt32(combPlacement["Profile_id"] == DBNull.Value ? 0 : combPlacement["Profile_id"]);
                object locationObject = 0;
                if (containerId != 0)
                    locationObject = tContainers.Select().Where(c => c.Column("id").Equal(containerId)).First()["LocationId"];
                int locationId = Convert.ToInt32(locationObject == DBNull.Value ? 0 : locationObject);
                // <int>    profile_id, container_id
                // <string> Status
                // <int>    Ok, Offline, Warning
                // <double> Current_cost, Profit, USD_revenue, Performance
                DBItem iContainerProfile = new DBItem(db, null);
                iContainerProfile["profile_id"] = profileId;
                iContainerProfile["container_id"] = containerId;
                // <int>    profile_id, location_id
                // <int>    Ok_container_count, Warning_container_count, Offline_container_count, Container_count
                // <double> Performance
                DBItem iLocationProfile;
                if (locationProfiles.ContainsKey(new Tuple<int, int>(profileId, locationId)))
                    iLocationProfile = locationProfiles[new Tuple<int, int>(profileId, locationId)];
                else
                {
                    iLocationProfile = new DBItem(db, null);
                    iLocationProfile["Performance"] = 0.0;
                    iLocationProfile["Ok_container_count"] = 0;
                    iLocationProfile["Warning_container_count"] = 0;
                    iLocationProfile["Offline_container_count"] = 0;
                    iLocationProfile["Container_count"] = 0;
                    iLocationProfile["Performance_xmr"] = 0.0;
                    iLocationProfile["Performance_dcr"] = 0.0;
                    locationProfiles.Add(new Tuple<int, int>(profileId, locationId), iLocationProfile);
                }


                // all rigs & miners of this combination
                IEnumerable<DBItem> combRigs = tRigPlacement.Select("Rig_name", "Testing").Where(con => con.Column("Container_id").Equal(containerId).And.Column("Profile_id").Equal(profileId)).ToList();
                IEnumerable<string> combRigNames = combRigs.Where(cr => !(bool)cr["Testing"]).Select(cr => (string)cr["Rig_name"]);
                IEnumerable<string> combRigNames_Testing = combRigs.Where(cr => (bool)cr["Testing"]).Select(cr => (string)cr["Rig_name"]);

                // !! there is no rigs for this combination !!
                if (!combRigs.Any())
                    continue;

                // get miners
                IEnumerable<JToken> combMiners = InputJSON.Where(m => combRigNames.Contains(m["name"].ToString()));
                IEnumerable<JToken> combTestingMiners = InputJSON.Where(m => combRigNames_Testing.Contains(m["name"].ToString()));

                /// Status
                // testing are always OK
                int statusOkCount = combMiners.Where(ac => ac["status"].ToString() == "up").ToList().Count + combTestingMiners.Count();
                int statusDownCount = combMiners.Where(ac => ac["status"].ToString() == "down").ToList().Count;
                int statusWarningCount = combMiners.Where(ac => ac["status"].ToString() == "warning").ToList().Count;
                int totalMinerCount = combMiners.Count() + combTestingMiners.Count();

                string status;
                if (totalMinerCount == statusDownCount)
                {
                    status = "Down";
                    iLocationProfile["Offline_container_count"] = (int)iLocationProfile["Offline_container_count"] + 1;
                }
                else if (statusWarningCount > 0)
                {
                    status = "Warning";
                    iLocationProfile["Warning_container_count"] = (int)iLocationProfile["Warning_container_count"] + 1;
                }
                else
                {
                    status = "Ok";
                    iLocationProfile["Ok_container_count"] = (int)iLocationProfile["Ok_container_count"] + 1;
                }

                List<string> pools = combMiners.Select(ac => ac["pools"].ToString()).ToList();

                List<string> poolsStrings = new List<string>();
                HashSet<string> uniqueStrings = new HashSet<string>();
                foreach (string pool in pools)
                {
                    string[] splitRow = pool.Split(';');
                    foreach (string s in splitRow)
                    {
                        uniqueStrings.Add(s);
                    }
                }

                string uniquePools = string.Join(";", uniqueStrings);

                iContainerProfile["pools"] = uniquePools;
                iContainerProfile["Ok"] = statusOkCount;
                iContainerProfile["Offline"] = statusDownCount;
                iContainerProfile["Warning"] = statusWarningCount;
                iContainerProfile["Status"] = status;

                iLocationProfile["Container_count"] = (int)iLocationProfile["Container_count"] + 1;

                /// hashrate
                double totalHashrate = 0;
                IEnumerable<string> listOfHashrates = combMiners.Select(ac => ac["rig_eth_hashrate"].ToString());
                foreach (string hr in listOfHashrates)
                {
                    totalHashrate += Convert.ToDouble(hr);
                }
                double roundGHashrate = System.Math.Round(totalHashrate / 1000000, 3); //0 a děleno 1 000 000 pro převod na GH
                iContainerProfile["Performance"] = roundGHashrate;
                iLocationProfile["Performance"] = (double)iLocationProfile["Performance"] + roundGHashrate;

                double totalHashrate_xmr = 0;
                IEnumerable<string> listOfHashrates_xmr = combMiners.Select(ac => ac["rig_xmr_hashrate"].ToString());
                foreach (string hr in listOfHashrates_xmr)
                {
                    totalHashrate_xmr += Convert.ToDouble(hr);
                }
                double roundGHashrate_xmr = System.Math.Round(totalHashrate_xmr, 3); //round kH/s (dont divide)
                iContainerProfile["Performance_xmr"] = roundGHashrate_xmr;
                iLocationProfile["Performance_xmr"] = (double)iLocationProfile["Performance_xmr"] + roundGHashrate_xmr;

                /// hashrate DCR
                double totalHashrate_dcr = 0;
                IEnumerable<string> listOfHashrates_dcr = combMiners.Select(ac => ac["rig_dcr_hashrate"].ToString());
                foreach (string hr in listOfHashrates_dcr)
                {
                    totalHashrate_dcr += Convert.ToDouble(hr);
                }
                double roundGHashrate_dcr = System.Math.Round(totalHashrate_dcr / 1000000, 3); //0 a děleno 1 000 000 pro převod na GH
                iContainerProfile["Performance_dcr"] = roundGHashrate_dcr;
                iLocationProfile["Performance_dcr"] = (double)iLocationProfile["Performance_dcr"] + roundGHashrate_dcr;

                /// revenue
                double containersRigsActive = Convert.ToDouble(statusOkCount + statusWarningCount);
                List<DBItem> wallets = tWallets.Select("Address").Where(c => c.Column("Profile_id").Equal(profileId).And.Column("Container_id").Equal(containerId)).ToList();
                List<DBItem> walletRevenues = tUsdRevenueByWallet.Select("AmountInUsd").Where(c => c.Column("days").Equal(1).And.Column("Wallet_Address").In(wallets.Select(w => (string)w["Address"]))).ToList();
                double UsdRevenueFinal = walletRevenues.Sum(wr => (double)wr["AmountInUsd"]);
                var profit = (UsdRevenueFinal / 100) * profitPercent;
                var cost = (UsdRevenueFinal / 100) * costPercent;

                iContainerProfile["Current_cost"] = System.Math.Round(cost, 2); // 2
                iContainerProfile["Profit"] = System.Math.Round(profit, 2); // 2
                iContainerProfile["USD_revenue"] = System.Math.Round(UsdRevenueFinal, 2); // 2
                tContainerProfileNew.Add(iContainerProfile);
            }

            foreach (var row in tContainerProfileNew)
            {
                var foundRowInOldTable = tContainerProfileOld.SingleOrDefault(c => c["container_id"].ToString() == row["container_id"].ToString() && c["profile_id"].ToString() == row["profile_id"].ToString());
                //if theres already the row with same ids, we need to update it
                if (foundRowInOldTable != null)
                {
                    foundRowInOldTable["Status"] = row["Status"];
                    foundRowInOldTable["Ok"] = row["Ok"];
                    foundRowInOldTable["Warning"] = row["Warning"];
                    foundRowInOldTable["Offline"] = row["Offline"];
                    foundRowInOldTable["Current_cost"] = row["Current_cost"];
                    foundRowInOldTable["Profit"] = row["Profit"];
                    foundRowInOldTable["USD_revenue"] = row["USD_revenue"];
                    foundRowInOldTable["Performance"] = row["Performance"];
                    foundRowInOldTable["Performance_xmr"] = row["Performance_xmr"];
                    foundRowInOldTable["Performance_dcr"] = row["Performance_dcr"];
                    foundRowInOldTable["pools"] = row["pools"];
                    tContainerProfile.Update(foundRowInOldTable, (int)foundRowInOldTable["id"]);
                    tContainerProfileOld.Remove(foundRowInOldTable);

                }
                //if its not found in old table, we gonna create it
                if (foundRowInOldTable == null)
                {
                    DBItem newRow = new DBItem(db, tContainerProfile);
                    newRow["Ok"] = row["Ok"];
                    newRow["Offline"] = row["Offline"];
                    newRow["Warning"] = row["Warning"];
                    newRow["Status"] = row["Status"];
                    newRow["Performance"] = row["Performance"];
                    newRow["Current_cost"] = row["Current_cost"];
                    newRow["Profit"] = row["Profit"];
                    newRow["USD_revenue"] = row["USD_revenue"];
                    newRow["profile_id"] = row["profile_id"];
                    newRow["container_id"] = row["container_id"];
                    newRow["Performance_xmr"] = row["Performance_xmr"];
                    newRow["Performance_dcr"] = row["Performance_dcr"];
                    newRow["pools"] = row["pools"];
                    tContainerProfile.Add(newRow);
                }

            }
            //remove all rows remaining in OldList
            foreach (var remainingRow in tContainerProfileOld)
            {
                tContainerProfile.Delete(remainingRow);
            }

            ///save changes
            db.SaveChanges();

            //// Locations ////
            List<DBItem> LocationProfileOld = tLocationProfile.Select().ToList();
            List<DBItem> LocationProfileNew = new List<DBItem>();

            foreach (KeyValuePair<Tuple<int, int>, DBItem> pair in locationProfiles)
            {
                Tuple<int, int> identify = pair.Key;
                DBItem iLocationProfile = pair.Value;

                iLocationProfile["profile_id"] = identify.Item1;
                iLocationProfile["location_id"] = identify.Item2;

                LocationProfileNew.Add(iLocationProfile);
                int locId = identify.Item2;
                if (locationHashrate.ContainsKey(locId))
                {
                    locationHashrate[locId]["ETH"] += Convert.ToDouble(iLocationProfile["Performance"]);
                    locationHashrate[locId]["XMR"] += Convert.ToDouble(iLocationProfile["Performance_xmr"]);
                    locationHashrate[locId]["DCR"] += Convert.ToDouble(iLocationProfile["Performance_dcr"]);
                }
                else
                {
                    locationHashrate.Add(locId, new Dictionary<string, double> {
                        { "ETH", Convert.ToDouble(iLocationProfile["Performance"]) },
                        { "XMR", Convert.ToDouble(iLocationProfile["Performance_xmr"]) },
                        { "DCR", Convert.ToDouble(iLocationProfile["Performance_dcr"]) }
                    });
                }

            }
            DBTable locationTable = db.Table("Location", false);
            foreach (var hashrateItem in locationHashrate)
            {
                int locId = hashrateItem.Key;
                var locationRow = locationTable.SelectById(locId);
                if (locationRow != null)
                {
                    locationRow["Performance"] = hashrateItem.Value["ETH"];
                    locationRow["Performance_xmr"] = hashrateItem.Value["XMR"];
                    locationRow["Performance_dcr"] = hashrateItem.Value["DCR"];
                    locationTable.Update(locationRow, locId);
                }
            }


            foreach (var row in LocationProfileNew)
            {
                var foundRowInOldTable = LocationProfileOld.SingleOrDefault(c => c["location_id"].ToString() == row["location_id"].ToString() && c["profile_id"].ToString() == row["profile_id"].ToString());
                //if theres already the row with same ids, we need to update it
                if (foundRowInOldTable != null)
                {
                    foundRowInOldTable["Ok_container_count"] = row["Ok_container_count"];
                    foundRowInOldTable["Warning_container_count"] = row["Warning_container_count"];
                    foundRowInOldTable["Offline_container_count"] = row["Offline_container_count"];
                    foundRowInOldTable["Container_count"] = row["Container_count"];
                    foundRowInOldTable["Performance"] = row["Performance"];
                    foundRowInOldTable["Performance_xmr"] = row["Performance_xmr"];
                    foundRowInOldTable["Performance_dcr"] = row["Performance_dcr"];
                    tLocationProfile.Update(foundRowInOldTable, (int)foundRowInOldTable["id"]);
                    LocationProfileOld.Remove(foundRowInOldTable);
                }
                //if its not found in old table, we gonna create it
                if (foundRowInOldTable == null)
                {
                    DBItem newRow = new DBItem(db, tLocationProfile);
                    newRow["Ok_container_count"] = row["Ok_container_count"];
                    newRow["Warning_container_count"] = row["Warning_container_count"];
                    newRow["Offline_container_count"] = row["Offline_container_count"];
                    newRow["Container_count"] = row["Container_count"];
                    newRow["Performance"] = row["Performance"];
                    newRow["profile_id"] = row["profile_id"];
                    newRow["location_id"] = row["location_id"];
                    newRow["Performance_xmr"] = row["Performance_xmr"];
                    newRow["Performance_dcr"] = row["Performance_dcr"];
                    tLocationProfile.Add(newRow);
                }
            }
            //remove all rows remaining in OldList
            foreach (var remainingRow in LocationProfileOld)
            {
                tLocationProfile.Delete(remainingRow);
            }

            /// truncate & add new lines
            db.SaveChanges();

            foreach (DBItem container in tContainers.Select().ToList())
            {
                var listRig = tRigPlacement.Select().ToList();
                int containerId = (int)container["id"];

                List<DBItem> containerRigs = tRigPlacement.Select("Rig_name", "Testing").Where(c => c.Column("Container_id").Equal(containerId)).ToList();
                IEnumerable<string> containerRigNames = containerRigs.Where(cr => !(bool)cr["Testing"]).Select(cr => (string)cr["Rig_name"]);
                IEnumerable<string> containerRigNames_Testing = containerRigs.Where(cr => (bool)cr["Testing"]).Select(cr => (string)cr["Rig_name"]);
                //var allContainerMiners = miners.Where(m => containersRig.Contains(m["name"].ToObject<string>()));
                var allContainerMiners = InputJSON.Where(m => containerRigNames.Contains(m["name"].ToString()));
                var allContainerTestingMiners = InputJSON.Where(m => containerRigNames_Testing.Contains(m["name"].ToString()));

                int statusDownCount = containerRigNames.Count() > 0 ? allContainerMiners.Where(ac => ac["status"].ToString() == "down").Count() : 0;
                int statusOkCount = containerRigNames.Count() > 0 ? allContainerMiners.Where(ac => ac["status"].ToString() == "up").Count() + allContainerTestingMiners.Count() : 0;
                int statusWarningCount = containerRigNames.Count() > 0 ? allContainerMiners.Where(ac => ac["status"].ToString() == "warning").Count() : 0;
                int totalMinerCount = containerRigNames.Count() > 0 ? allContainerMiners.Count() + allContainerTestingMiners.Count() : 0;

                //COUNTING REVENUE#####################################################
                double totalHashrate = 0;
                var listOfHashrates = allContainerMiners.Select(ac => ac["rig_eth_hashrate"].ToString()).ToList();
                foreach (var hr in listOfHashrates)
                {
                    totalHashrate += Convert.ToDouble(hr);
                }
                double totalHashrate_xmr = 0;
                var listOfHashrates_xmr = allContainerMiners.Select(ac => ac["rig_xmr_hashrate"].ToString()).ToList();
                foreach (var hr in listOfHashrates_xmr)
                {
                    totalHashrate_xmr += Convert.ToDouble(hr);
                }
                double totalHashrate_dcr = 0;
                var listOfHashrates_dcr = allContainerMiners.Select(ac => ac["rig_dcr_hashrate"].ToString()).ToList();
                foreach (var hr in listOfHashrates_dcr)
                {
                    totalHashrate_dcr += Convert.ToDouble(hr);
                }

                

                if (containerRigNames.Count() > 0)
                {
                    int containersRIgs = statusOkCount + statusWarningCount;
                    List<DBItem> wallets = tWallets.Select("Address").Where(c => c.Column("Container_id").Equal(containerId)).ToList();
                    List<DBItem> walletRevenues = tUsdRevenueByWallet.Select("AmountInUsd").Where(c => c.Column("days").Equal(1).And.Column("Wallet_Address").In(wallets.Select(w => (string)w["Address"]))).ToList();
                    double UsdRevenueFinal = walletRevenues.Sum(wr => (double)wr["AmountInUsd"]);
                    var profit = (UsdRevenueFinal / 100) * profitPercent;
                    var cost = (UsdRevenueFinal / 100) * costPercent;

                    container["Current_cost"] = System.Math.Round(cost, 2); //2
                    container["Profit"] = System.Math.Round(profit, 2); //2
                    container["USD_revenue"] = System.Math.Round(UsdRevenueFinal, 2); //2
                    container["Performance"] = System.Math.Round(totalHashrate / 1000000, 3); //0 a děleno 1 000 000 pro převod na GH
                    container["Performance_xmr"] = System.Math.Round(totalHashrate_xmr, 3); //round kH/s (dont divide)
                    container["Performance_dcr"] = System.Math.Round(totalHashrate_dcr / 1000000, 3); //0 a děleno 1 000 000 pro převod na GH
                    //END COUNTING REVENUE#######################################




                    //Count STAtus###########################
                    string status = "";
                    if (totalMinerCount == statusDownCount)
                    {
                        status = "Down";
                    }
                    else if (totalMinerCount == statusOkCount)
                    {
                        status = "Ok";
                    }
                    else
                    {
                        status = "Warning";
                    }
                    container["Ok"] = statusOkCount;
                    container["Offline"] = statusDownCount;
                    container["Warning"] = statusWarningCount;
                    container["Status"] = status;
                }
                else
                {
                    container["Ok"] = 0;
                    container["Offline"] = 0;
                    container["Warning"] = 0;
                    container["Current_cost"] = 0;
                    container["Profit"] = 0;
                    container["USD_revenue"] = 0;
                    container["Performance"] = 0;
                    container["Performance_xmr"] = 0;
                    container["Performance_dcr"] = 0;
                    container["Status"] = "Down";

                }

                //END COUNTING sTATUS#######################################

                tContainers.Update(container, Convert.ToInt32(container["id"]));
                db.SaveChanges();

            }

            foreach (var location in locationTable.Select().ToList())
            {
                int warning = 0;
                int ok = 0;
                int down = 0;
                double totalHR = 0;
                double totalHR_xmr = 0;
                double totalHR_dcr = 0;
                location["Warning_container_count"] = 0;
                location["Ok_container_count"] = 0;
                location["Offline_container_count"] = 0;
                int locationId = (int)location["id"];
                var containersInLocation = tContainers.Select().ToList().Where(x => x["LocationId"] is int ? (int)x["LocationId"] == locationId : false).ToList();
                foreach (var container in containersInLocation)
                {
                    totalHR += Convert.ToDouble(container["Performance"]);
                    totalHR_xmr += Convert.ToDouble(container["Performance_xmr"]);
                    totalHR_dcr += Convert.ToDouble(container["Performance_dcr"]);
                    if (Convert.ToInt32(container["Warning"]) > 0)
                    {
                        warning += 1;
                    }
                    else if (Convert.ToInt32(container["Ok"]) == 0 && Convert.ToInt32(container["Warning"]) == 0)
                    {
                        down += 1;
                    }
                    else if (Convert.ToInt32(container["Offline"]) == 0 && Convert.ToInt32(container["Warning"]) == 0)
                    {
                        ok += 1;
                    }
                    else
                    {
                        ok += 1;
                    }
                }

                location["Ok_container_count"] = ok;
                location["Warning_container_count"] = warning;
                location["Offline_container_count"] = down;
                location["Container_count"] = containersInLocation.Count();
                location["Performance"] = totalHR;
                location["Performance_xmr"] = totalHR_xmr;
                location["Performance_dcr"] = totalHR_dcr;
                locationTable.Update(location, Convert.ToInt32(location["id"]));
                db.SaveChanges();
            }
        }
    }
}
