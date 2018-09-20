using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    /// <summary>
    /// Slouží pro update tabulky peněženek. Akce načte stav balance z API a uloží do tabulky peněženky.
    /// </summary>
    [NexusRepository]
    class GetWalletBalancesAction : Action
    {
        private COREobject _core;
        private Dictionary<string, string> apiList = new Dictionary<string, string>()
        {
            { "ETH", "http://api.etherscan.io/api?module=account&action=balance&address={0}&tag=latest" },
            { "BTC", "https://blockexplorer.com/api/addr/{0}/balance" },
            { "ZEC", "http://api.zcha.in/v2/mainnet/accounts/{0}" },
            { "DCR", "https://mainnet.decred.org/api/addr/{0}/balance" },
            { "SIA", "" },
            { "LTC", "https://api.blockcypher.com/v1/ltc/main/addrs/{0}/balance" }
        };
        
        private DBItem currentWallet;
        private List<string> blackListAddresses;
        private int responseStatus = 200;
        private DBConnection _db;

        public override int Id
        {
            get
            {
                return 30051;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "?s$WalletsTableName"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Grid: Get Wallet Balances";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[0];
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
            _core = COREobject.i;
            _db = _core.Entitron;
            DBTable blackListTable = _db.Table("BlackListWallets", false); //blackList table
            blackListAddresses = blackListTable.Select().ToList().Select(x => x["Address"].ToString()).ToList();

            string walletsTableName = vars.ContainsKey("WalletsTableName") ? (string)vars["WalletsTableName"] : "Wallets";
            DBTable walletsTable = _db.Table(walletsTableName, false);
            var select = walletsTable.Select();
            List<DBItem> wallets = select.ToList();    
            List<DBItem> cachedWallets = new List<DBItem>(); //aby se kvůli různým profilům nemusela peněženka načítat několikrát            

            foreach(DBItem wallet in wallets)
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
                    switch ((string)wallet["Currency_code"])
                    {
                        case "ETH": balance = GetEthBalance(); break;
                        case "BTC": balance = GetBtcBalance(); break;
                        case "LTC": balance = GetLtcBalance(); break;
                        case "DCR": balance = GetDcrBalance(); break;
                        case "ZEC": balance = GetZecBalance(); break;
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

            _db.SaveChanges();

            //testing
            //double? zec = GetZecBalance("https://api.zcha.in/v2/mainnet/accounts/t1VpYecBW4UudbGcy4ufh61eWxQCoFaUrPs");
            //double? btc = GetBtcBalance("https://blockexplorer.com/api/addr/198aMn6ZYAczwrE5NvNTUMyJ5qkfy4g3Hi/balance");
            //double? ltc = GetLtcBalance("https://api.blockcypher.com/v1/ltc/main/addrs/LfdB6hPkdsyJ63DT2uYWALe9DVViwyrNSo/balance");
        }

        private double? GetEthBalance(string apiUrl = null)
        {
            JToken response = GetResponse(apiUrl ?? GetApiURL());
            if (responseStatus == 200 && response != null)
            {
                if ((int)response["status"] == 1)
                    return Convert.ToDouble(response["result"]) / 1000000000000000000.0;
                else
                    return null;
            }
            else
                return null;
        }
        private double? GetBtcBalance(string apiUrl = null)
        {
            JToken response = GetResponse(apiUrl ?? GetApiURL());
            if (responseStatus == 200 && response != null)
            {
                return response.ToObject<double>() / 100000000.0;
            }
            else
                return null;
        }
        private double? GetLtcBalance(string apiUrl = null)
        {
            JToken response = GetResponse(apiUrl ?? GetApiURL());
            if (responseStatus == 200 && response != null)
            {

                 return Convert.ToDouble(response["balance"]) / 100000000.0;
            }
            else
                return null;
        }
        private double? GetDcrBalance(string apiUrl = null)
        {
            JToken response = GetResponse(apiUrl ?? GetApiURL());
            if (responseStatus == 200 && response != null)
            {
                return response.ToObject<double>() / 100000000.0;
            }
            else
                return null;
        }
        private double? GetZecBalance(string apiUrl = null)
        {
            JToken response = GetResponse(apiUrl ?? GetApiURL());
            if (responseStatus == 200 && response != null)
            {
                return Convert.ToDouble(response["balance"]);
            }
            else
                return null;
        }

        private JToken GetResponse(string url)
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
                Watchtower.OmniusException.Log($"Unable to load response from remote server. API url {url} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, _db.Application, _core.User);
                responseStatus = 500;
                return null;
            }
        }

        private string GetApiURL()
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]);
        }

    }
}
