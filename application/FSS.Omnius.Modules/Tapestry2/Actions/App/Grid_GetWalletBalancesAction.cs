using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public partial class Grid : ActionManager
    {
        [Action(30051, "Grid: Get Wallet Balances")]
        public static void GetWalletBalances(COREobject core, string WalletsTableName = "Wallets")
        {
            Dictionary<string, string> apiList = new Dictionary<string, string>()
            {
                { "ETH", "http://api.etherscan.io/api?module=account&action=balance&address={0}&tag=latest" },
                { "BTC", "https://blockexplorer.com/api/addr/{0}/balance" },
                { "ZEC", "http://api.zcha.in/v2/mainnet/accounts/{0}" },
                { "DCR", "https://mainnet.decred.org/api/addr/{0}/balance" },
                { "SIA", "" },
                { "LTC", "https://api.blockcypher.com/v1/ltc/main/addrs/{0}/balance" }
            };
            
            DBItem currentWallet;
            List<string> blackListAddresses;
            int responseStatus = 200;

            DBConnection db = core.Entitron;
            DBTable blackListTable = db.Table("BlackListWallets", false); //blackList table
            blackListAddresses = blackListTable.Select().ToList().Select(x => x["Address"].ToString()).ToList();
            
            DBTable walletsTable = db.Table(WalletsTableName, false);
            var select = walletsTable.Select();
            List<DBItem> wallets = select.ToList();
            List<DBItem> cachedWallets = new List<DBItem>(); //aby se kvůli různým profilům nemusela peněženka načítat několikrát            

            foreach (DBItem wallet in wallets)
            {
                currentWallet = wallet;
                bool update = true;
                if (cachedWallets.Contains(wallet))
                {
                    //load cached
                    wallet["Balance"] = cachedWallets.Single(e => e["Address"] == wallet["Address"])["Balance"];
                }
                else
                {
                    double? balance = null;
                    JToken response;
                    switch ((string)wallet["Currency_code"])
                    {
                        case "ETH":
                            response = GetResponse(core, string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]), out responseStatus);
                            if (responseStatus == 200 && response != null && (int)response["status"] == 1)
                            {
                                balance = Convert.ToDouble(response["result"]) / 1000000000000000000.0;
                            }
                            break;
                        case "BTC":
                            response = GetResponse(core, string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]), out responseStatus);
                            if (responseStatus == 200 && response != null)
                            {
                                balance = response.ToObject<double>() / 100000000.0;
                            }
                            break;
                        case "LTC":
                            response = GetResponse(core, string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]), out responseStatus);
                            if (responseStatus == 200 && response != null)
                            {
                                balance = Convert.ToDouble(response["balance"]) / 100000000.0;
                            }
                            break;
                        case "DCR":
                            response = GetResponse(core, string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]), out responseStatus);
                            if (responseStatus == 200 && response != null)
                            {
                                balance = response.ToObject<double>() / 100000000.0;
                            }
                            break;
                        case "ZEC":
                            response = GetResponse(core, string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]), out responseStatus);
                            if (responseStatus == 200 && response != null)
                            {
                                balance = Convert.ToDouble(response["balance"]);
                            }
                            break;
                    }
                    if (balance != null)
                    {
                        wallet["Balance"] = balance;
                        cachedWallets.Add(wallet);
                    }
                    else
                        update = false;
                }

                if (update)
                {
                    walletsTable.Update(wallet, (int)wallet["id"]);
                }
            }

            db.SaveChanges();
        }
        private static JToken GetResponse(COREobject core, string url, out int responseStatus)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";

            try
            {
                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();

                responseStatus = 200;

                if (!string.IsNullOrEmpty(outputJsonString))
                {
                    return JToken.Parse(outputJsonString);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Watchtower.OmniusException.Log($"Unable to load response from remote server. API url {url} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, core.Application, core.User);
                responseStatus = 500;
                return null;
            }
        }
    }
}
