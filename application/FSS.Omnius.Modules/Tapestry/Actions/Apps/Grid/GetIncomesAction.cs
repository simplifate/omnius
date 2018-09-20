using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using FSS.Omnius.Modules.Entitron.Entity;
using Jayrock.Json;
using System.Web;
using System.Xml;

namespace FSS.Omnius.Modules.Tapestry.Actions.Apps.Grid
{
    [NexusRepository]
    class GetIncomesAction : Action
    {
        private const string xmrRpcUsername = "nakamotox";
        private const string xmrRpcPass = "EN140W9/cNeUO7qL";
        private const string scRpcUsername = "grid";
        private const string scRpcPass = "g149u6W3_e8/Wkqi";
        private int idRpc = 0;
        private DBConnection _db;

        private Dictionary<string, string> apiList = new Dictionary<string, string>()
        {
            { "ETH", "http://api.etherscan.io/api?module=account&action=txlist&address={0}&startblock={1}&sort=asc"},
            { "BTC", "https://blockexplorer.com/api/addrs/{0}/txs?from={1}&to={2}" },
            { "ZEC", "http://api.zcha.in/v2/mainnet/accounts/{0}/{1}?order=height&direction=ascending&offset={2}&limit={3}" },
            { "DCR", "https://mainnet.decred.org/api/addrs/{0}/txs/?from={1}" },
            { "LTC", "https://api.blockcypher.com/v1/ltc/main/addrs/{0}/full?limit={1}&before={2}" },
            { "XMR", "http://192.168.8.30:8093/json_rpc" },
            //{ "XMR", "http://185.59.209.146:8093/json_rpc" },
            { "SC", "http://192.168.8.30:8091/wallet/transactions?startheight={0}&endheight={1}" },
            //{ "SC", "http://185.59.209.146:8091/wallet/transactions?startheight={0}&endheight={1}" },
            { "MUSIC", "https://orbiter.musicoin.org/addr" }
        };

        private COREobject _core;

        private DBTable historyTable;
        private DBTable walletsTable;
        private DBItem currentWallet;
        private DBTable offsetTable;
        private List<string> blackListAddresses;
        private Dictionary<int, List<DBItem>> transactionList = new Dictionary<int, List<DBItem>>();

        //Tuple< txid, wallet address >
        private Dictionary<string, HashSet<Tuple<string,string>>> txIds = new Dictionary<string, HashSet<Tuple<string, string>>>();

        private List<string> allWallets = new List<string>();
        private object threadLock = new object();

        private int responseStatus = 200;

        public override int Id
        {
            get
            {
                return 3005;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[]
                {
                    "?s$WalletsTableName", "?s$HistoryTableName"
                };
            }
        }

        public override string Name
        {
            get
            {
                return "Get wallets income Action";
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
            _core = COREobject.i;
            _db = _core.Entitron;

            try
            {
                lock (threadLock)
                {
                    //Ignore SSL/TLS errors on Windows Server
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    var blackListTable = _db.Table("BlackListWallets", false); //blackList table
                    blackListAddresses = blackListTable.Select().ToList().Select(x => x["Address"].ToString()).ToList();

                    transactionList = new Dictionary<int, List<DBItem>>();
                    txIds = new Dictionary<string, HashSet<Tuple<string, string>>>(); 
                    allWallets = new List<string>();

                    string walletsTableName = vars.ContainsKey("WalletsTableName") ? (string)vars["WalletsTableName"] : "Wallets";
                    string historyTableName = vars.ContainsKey("HistoryTableName") ? (string)vars["HistoryTableName"] : "TransactionHistory";
                    
                    walletsTable = _db.Table(walletsTableName, false);
                    historyTable = _db.Table(historyTableName, false);
                    offsetTable = _db.Table("AddressLastTransactionOffset");
                    
                    List<DBItem> wallets = _db.Select("DistinctWalletsIds", false).ToList();

                    foreach (DBItem wallet in wallets)
                    {
                        allWallets.Add(((string)wallet["Address"]).ToLower());
                        if (!txIds.ContainsKey((string)wallet["Currency_code"]))
                        {
                            txIds.Add((string)wallet["Currency_code"], new HashSet<Tuple<string, string>>());
                        }
                    }

                    bool isCurrencyNotFound = false;
                    HashSet<Tuple<string, string>> currenciesNotFound = new HashSet<Tuple<string, string>>();
                    foreach (DBItem item in historyTable.Select("Transaction_id", "Currency_code", "Wallet_Address").ToList())
                    {
                        try
                        {
                            if (item["Transaction_id"] != DBNull.Value)
                            {
                                txIds[(string)item["Currency_code"]].Add(new Tuple<string, string>((string)item["Transaction_id"],(string)item["Wallet_Address"]));
                            }
                        }
                        catch (KeyNotFoundException)
                        {
                            isCurrencyNotFound = true;
                            currenciesNotFound.Add(new Tuple<string, string>((string)item["Wallet_Address"], (string)item["Currency_code"]));
                        }
                        catch (Exception e)
                        {
                            Watchtower.OmniusLog.Log($"{Name}: {e.Message}", Watchtower.OmniusLogLevel.Error);
                        }
                    }
                    if (isCurrencyNotFound)
                    {
                        Watchtower.OmniusLog.Log($"{Name}: Transaction history contains data of non-existing wallet(s)! {String.Join(", ", currenciesNotFound.Select(t => string.Format("[{0} {1}]", t.Item2, t.Item1)))}", Watchtower.OmniusLogLevel.Warning);
                    }


                    foreach (DBItem wallet in wallets)
                    {
                        currentWallet = wallet;
                        switch ((string)wallet["Currency_code"])
                        {
                            case "ZEC": LoadZecTx(); break;
                            case "ETH": LoadEthTx(); break;
                            case "BTC": LoadBtcTx(); break;
                            case "LTC": LoadLtcTx(); break;
                            case "DCR": LoadDcrTx(); break;
                            case "XMR": LoadXmrTx(); break;
                            case "SC": LoadScTx(); break;
                            case "MUSIC": LoadMusicTx(); break;
                        }
                        foreach (KeyValuePair<int, List<DBItem>> pair in transactionList)
                        {
                            foreach (DBItem item in pair.Value)
                            {
                                //Retry getting exchange rate
                                if ((bool)item["ToBeFixed"])
                                {
                                    Tuple<double, bool> exRate = GetExchangeRate((DateTime)item["Transaction_date"]);
                                    item["Exchange_rate"] = exRate.Item1;
                                    item["ToBeFixed"] = exRate.Item2;
                                }
                                historyTable.Add(item);
                            }
                        }
                        _db.SaveChanges();
                        transactionList.Clear();
                    }

                 }

            }
            catch (Exception ex)
            {
                string m = ex.Message;
            }
        }

        private void LoadMusicTx()
        {
            //get last used offset
            DBItem offsetItem = null;
            long offset = GetOffset( "offset_start", ref offsetItem );

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiList["MUSIC"]);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            var formData = new Dictionary<string, string> { { "addr", (string)currentWallet["Address"] }, { "start", offset.ToString() } };
            string postData ="";
            foreach (string key in formData.Keys)
            {
                postData += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(formData[key]) + "&";
            }
            byte[] data = Encoding.ASCII.GetBytes(postData);

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            JToken JResponse;
            try
            {
                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();

                responseStatus = 200;

                if (!string.IsNullOrEmpty(outputJsonString))
                    JResponse = JToken.Parse(outputJsonString);
                else
                    JResponse = null;
            }
            catch (Exception e)
            {
                responseStatus = 500;
                Watchtower.OmniusException.Log($"Aborting transaction sync. Reason: Unable to load response from remote server. API url {apiList["MUSIC"]} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, _db.Application, _core.User);
                return;
            }
            if (JResponse.HasValues)
            {
                foreach(JArray tx in JResponse["data"])
                {
                    string txId = (string)tx[0];
                    if (IsInMemory(txId, (string)currentWallet["Address"]))
                    {
                        ++offset;
                        continue;
                    }
                    string addressTo = (string)tx[3];
                    string addressFrom = (string)tx[2];
                    if (allWallets.Contains(addressTo) && allWallets.Contains(addressFrom)) //přenos mezi systémovými peněženkami
                        continue;
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((int)tx[6]);
                    double ammount = (double)tx[4];
                    string type = allWallets.Contains( addressTo ) ? "income" : "outcome" ;
                    AddItem(txDate, ammount, txId, type);
                    ++offset;
                    
                } 
            }
            SaveOffset(offsetItem, offset);
        }

        private void LoadScTx()
        {
            DBItem offsetItem = null;
            long offset = GetOffset("block_start", ref offsetItem);

            long endHeight = 999999999999999999;
            DateTime lastRecordedTxDate = GetLastRecordedTxDate();
            string address = ((string)currentWallet["Address"]).ToLower();

            JToken response = GetResponse(GetApiURL(offset, endHeight),"SC");
            if (responseStatus != 500 && response != null && response["confirmedtransactions"].Children().Count() > 0)

            foreach (JToken tx in response["confirmedtransactions"])
            {
                if (tx["confirmationtimestamp"] == null)
                    continue;
                offset = (long)tx["confirmationheight"];
                DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);


                txDate = txDate.AddSeconds((double)tx["confirmationtimestamp"]);
                string txId = (string)tx["transactionid"];

                // Zajímají nás ty, co zatím nemáme v DB
                if (!IsInMemory(txId, (string)currentWallet["Address"]))
                {
                    double amountIn = 0;
                    double amountOut = 0;
                    List<string> incomeAddrs = new List<string>();

                    // outcome
                    foreach (JToken txIn in tx["inputs"])
                    {
                        string addr = (string)txIn["relatedaddress"];
                        if (!string.IsNullOrEmpty(addr))
                        {
                            incomeAddrs.Add(addr.ToLower());
                            if (((string)txIn["relatedaddress"]).ToLower() == address)
                            {
                                amountOut += (double)txIn["value"];
                            }
                        }
                    }
                    // income
                    foreach (JToken txOut in tx["outputs"])
                    {
                        if (((string)(txOut["relatedaddress"])).ToLower() == address && txOut["fundtype"].ToString() == "siacoin output")
                        {
                            bool mayBeForward = false;
                            foreach (string addr in incomeAddrs)
                            {
                                if (allWallets.Contains(addr))
                                {
                                    mayBeForward = true;
                                }
                            }
                            // Pokud by to mohlo být přeposlané z jiné systémové peněženky, ignorujeme
                            if (!mayBeForward)
                            {
                                amountIn += (double)txOut["value"];
                            }
                        }
                    }

                    if (amountIn > 0)
                    {
                        AddItem(txDate, amountIn, txId, "income");
                    }
                    if (amountOut > 0)
                    {
                        AddItem(txDate, amountOut, txId, "outcome");
                    }
                }
            }
            SaveOffset(offsetItem, offset);
        }
        private void LoadXmrTx()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + "CurrencyNodes.xml");


            var node = doc.SelectNodes(String.Format("//node[@currency='XMR']"))[0];


            DBItem offsetItem = null;
            long offset = GetOffset("block_height", ref offsetItem);
            try
            {
                string method = "get_transfers";
                //parameters
                Dictionary<string, object> paramsDict = new Dictionary<string, object>();
                paramsDict.Add("in", true);
                paramsDict.Add("filter_by_height", true);
                paramsDict.Add("min_height", offset);
                paramsDict.Add("max_height", 999999999999999999);
                string rpcVersion = "2.0";
                string rpcUserName = node.Attributes["rpcUser"].Value;
                string rpcPassword = node.Attributes["rpcPassword"].Value;

                // Create request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiList["XMR"]);
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Credentials = new NetworkCredential(rpcUserName, rpcPassword); //make credentials for request

                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";

                JsonObject call = new JsonObject();
                call["jsonrpc"] = rpcVersion;
                call["id"] = ++idRpc;
                call["method"] = method;
                call["params"] = paramsDict;
                string jsonString = call.ToString();

                byte[] postJsonBytes = Encoding.UTF8.GetBytes(jsonString);

                httpWebRequest.ContentLength = postJsonBytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(postJsonBytes, 0, postJsonBytes.Length);

                var response = httpWebRequest.GetResponse();
                using (Stream stream2 = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream2, Encoding.UTF8))
                {
                    JsonObject answer = new JsonObject();
                    answer.Import(new Jayrock.Json.JsonTextReader(reader));
                    object errorObject = answer["error"];
                    var jtokenResult = JToken.Parse(answer["result"].ToString());
                    if (!jtokenResult.HasValues)
                    {
                        // no new values
                        return;
                    }
                    foreach (var transactionIn in jtokenResult["in"]) { //iterate each object in the array of IN
                        //if (x["in"]["amount"] > 0)
                        //{
                        //    work = AddItem(x["date"], amount, txId, "income");
                        //}
                        if ((double)transactionIn["amount"] > 0) {
                            offset = (long)transactionIn["height"];
                            if (IsInMemory((string)transactionIn["txid"], (string)currentWallet["Address"]))
                                continue;
                            // Format our new DateTime object to start at the UNIX Epoch
                            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                            // Add the timestamp (number of seconds since the Epoch) to be converted
                            dateTime = dateTime.AddSeconds((double)transactionIn["timestamp"]);

                            AddItem(dateTime, (double)transactionIn["amount"] / 1000000000000, transactionIn["txid"].ToString(), "income");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Watchtower.OmniusException.Log($"Aborting transaction sync. Reason: Unable to load response from remote server. API url {apiList["XMR"]} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, _db.Application, _core.User);
            }
            SaveOffset(offsetItem, offset);
        }
        /*private void LoadBtcTx()
        {
            JToken response = GetResponse(GetApiURL());
            if (responseStatus == 200 && response != null)
            {
                if ((string)response["status"] == "success")
                {
                    DateTime lastRecordedTxDate = GetLastRecordedTxDate();
                    foreach (JToken tx in response["data"]["txs"])
                    {
                        double amountInCurrency = (double)tx["amount"];
                        string txId = (string)tx["tx"];
                        DateTime txDate = DateTime.Parse((string)tx["time_utc"]);

                        // Zajímají nás jen ty, co zatím nemáme v DB
                        if (txDate > lastRecordedTxDate && !IsInDB(txId))
                        {
                            if (!AddItem(txDate, amountInCurrency, txId, amountInCurrency < 0 ? "outcome" : "income"))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }*/

        private void LoadBtcTx()
        {
            int from = 0;
            int to = 50;
            int maxViewed = 50;
            bool work = true;

            while (work)
            {
                JToken response = GetResponse(GetApiURL(from, to));
                if (responseStatus == 500 || response == null || response["items"].Children().Count() == 0)
                {
                    work = false;
                    break;
                }
                
                DateTime lastRecordedTxDate = GetLastRecordedTxDate();
                foreach (JObject tx in response["items"])
                {
                    
                    string txId = (string)tx["txid"];
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    txDate = txDate.AddSeconds(Convert.ToDouble(tx["time"]));

                    // Zajímají nás jen ty, co zatím nemáme v DB
                    if (txDate > lastRecordedTxDate && !IsInMemory(txId, currentWallet["Address"].ToString()))
                    {
                        /*if (!AddItem(txDate, amountInCurrency, txId, amountInCurrency < 0 ? "outcome" : "income"))
                        {
                            break;
                        }*/
                        double amountIn = 0;
                        double amountOut = 0;
                        //outcomes
                        foreach (var vin in tx["vin"])
                        {
                            if (((string)vin["addr"]).ToLower() == ((string)currentWallet["Address"]).ToLower())
                            {
                                amountOut += (double)vin["value"];
                            }
                        }

                        //incomes
                        foreach (var vout in tx["vout"])
                        {
                            //přeskočit přenosy mezi peněženkami Gridu  
                            if (!allWallets.Contains((string)vout["scriptPubKey"]["addresses"][0]))
                            {
                                if (((string)vout["scriptPubKey"]["addresses"][0]).ToLower() == ((string)currentWallet["Address"]).ToLower())
                                {
                                    amountIn += Convert.ToDouble(vout["value"]);
                                }
                            }
                        }

                        if (amountIn > 0)
                        {
                            work = AddItem(txDate, amountIn, txId, "income");
                        }
                        if (amountOut > 0)
                        {
                            work = work && AddItem(txDate, amountOut, txId, "outcome");
                        }
                    }
                    else
                    {
                        work = false;
                        break;
                    }
                    if (!work)
                    {
                        break;
                    }
                }
                from = to + 1;
                to += maxViewed;
            }
        }

        /*private void LoadLtcTx()
        {
            LoadBtcTx();
        }*/

        private void LoadLtcTx()
        {
            try
            {
                bool hasMore = false; //začíná se bez posunutí
                int before = 0; //když hasMore je true, nastaví se na blockheight posledního itemu api odpovědi
                int maxViewed = 50;
                bool work = true;

                while (work)
                {
                    JToken response = GetResponse(GetApiURL(maxViewed, before));
                    if (responseStatus == 500 || response == null || response["txs"].Children().Count() == 0)
                    {
                        work = false;
                        break;
                    }

                    DateTime lastRecordedTxDate = GetLastRecordedTxDate();
                    foreach (JObject tx in response["txs"])
                    {
                        string txId = (string)tx["hash"];
                        DateTime txDate = DateTime.Parse(tx["received"].ToString());

                        // Zajímají nás jen ty, co zatím nemáme v DB
                        if (txDate > lastRecordedTxDate && !IsInMemory(txId, currentWallet["Address"].ToString()))
                        {
                            /*if (!AddItem(txDate, amountInCurrency, txId, amountInCurrency < 0 ? "outcome" : "income"))
                            {
                                break;
                            }*/
                            double amountIn = 0;
                            double amountOut = 0;
                            //outcomes
                            foreach (var vin in tx["inputs"])
                            {
                                if (((string)vin["addresses"][0]).ToLower() == ((string)currentWallet["Address"]).ToLower())
                                {
                                    amountOut += Convert.ToDouble(vin["output_value"]) / 100000000.0;
                                }
                            }

                            //incomes
                            foreach (var vout in tx["outputs"])
                            {
                                //přeskočit přenosy mezi peněženkami Gridu  
                                if (!allWallets.Contains((string)vout["addresses"][0]))
                                {
                                    if (((string)vout["addresses"][0]).ToLower() == ((string)currentWallet["Address"]).ToLower())
                                    {
                                        if(vout["value"] != null)
                                            amountIn += Convert.ToDouble(vout["value"]) / 100000000.0;
                                    }
                                }
                            }

                            if (amountIn > 0)
                            {
                                work = AddItem(txDate, amountIn, txId, "income");
                            }
                            if (amountOut > 0)
                            {
                                work = work && AddItem(txDate, amountOut, txId, "outcome");
                            }
                        }
                        else
                        {
                            work = false;
                            break;
                        }
                        if (!work)
                        {
                            break;
                        }
                    }
                    if(response.Contains("hasMore"))
                        hasMore = (bool)response["hasMore"];
                    if (hasMore)
                    {
                        int i = response["txs"].Count() - 1;
                        before = (int)response["txs"][i]["block_height"];
                    }
                    else
                    {
                        work = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                string er = e.Message;
            }
        }

        private void LoadDcrTx()
        {

            DBItem offsetItem = null;
            long offset = GetOffset("offset_from", ref offsetItem);

            bool work = true;
            string address = ((string)currentWallet["Address"]).ToLower();

            while (work)
            {
                JToken response = GetResponse(GetApiURL(offset));
                if (responseStatus == 500 || response == null || response["items"].Children().Count() == 0)
                {
                    work = false;
                    break;
                }

                foreach (JToken tx in response["items"])
                {
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                    if (tx["time"] == null)
                        continue;

                    txDate = txDate.AddSeconds((double)tx["time"]);
                    string txId = (string)tx["txid"];

                    // Zajímají nás ty, co zatím nemáme v DB
                    if (!IsInMemory(txId, (string)currentWallet["Address"]))
                    {
                        double amountIn = 0;
                        double amountOut = 0;
                        List<string> incomeAddrs = new List<string>();

                        // outcome
                        /*var vintArray = tx["vin"].Where(vin => ((string)vin["addr"]).ToLower().Equals(address)).ToList();
                        if (vintArray.Count > 0)
                            foreach (JToken vin in vintArray)
                            {
                                string addr = (string)vin["addr"];
                                if (!string.IsNullOrEmpty(addr))
                                {
                                    incomeAddrs.Add(addr.ToLower());
                                    if (((string)vin["addr"]).ToLower() == address)
                                    {
                                        amountOut += (double)vin["value"];
                                    }
                                }
                            }*/

                        //search inputs for outcoming transactions
                        foreach(var input in tx["vin"])
                        {

                            if(input["addr"] != null && input["addr"].ToString().ToLower() == address)
                            {
                                incomeAddrs.Add(input["addr"].ToString().ToLower());
                                amountOut += (double)input["value"];
                            }

                        }
                        // income

                        foreach(var output in tx["vout"])
                        {
                            if(((string)(output["scriptPubKey"]["addresses"][0])).ToLower() == address)
                            {
                                bool mayBeForward = false;
                                foreach (string addr in incomeAddrs)
                                {
                                    if (allWallets.Contains(addr))
                                    {
                                        mayBeForward = true;
                                    }
                                }
                                // Pokud by to mohlo být přeposlané z jiné systémové peněženky, ignorujeme
                                if (!mayBeForward)
                                    amountIn += (double)output["value"];
                            }
                        }
                        /*var voutArray = tx["vout"].Where(vout => ((string)(vout["scriptPubKey"]["addresses"][0])).ToLower().Equals(address)).ToList();
                        if (voutArray.Count > 0)
                            foreach (JToken vout in voutArray)
                            {
                                bool mayBeForward = false;
                                foreach (string addr in incomeAddrs)
                                {
                                    if (allWallets.Contains(addr))
                                    {
                                        mayBeForward = true;
                                    }
                                }
                                // Pokud by to mohlo být přeposlané z jiné systémové peněženky, ignorujeme
                                if (!mayBeForward)
                                    amountIn += (double)vout["value"];
                            }*/
                        if (amountIn > 0)
                            AddItem(txDate, amountIn, txId, "income");
                        if (amountOut > 0)
                            AddItem(txDate, amountOut, txId, "outcome");
                            ++offset;
                    }
                    else
                    {
                        ++offset;
                        continue; //work = false;
                    }
                    if (!work)
                        break;
                }
            }
            SaveOffset(offsetItem, offset);            
        }

        private void LoadZecTx()
        {
            LoadZecSentTx();
            LoadZecRecvTx();
        }

        private void LoadZecSentTx()
        {
            DBItem offsetItem = null;
            long offset = GetOffset("offset_sent", ref offsetItem);
            string currentAddress = (string)currentWallet["Address"];
            List<string> otherWallets = allWallets.Where(x => x != currentAddress).ToList();

            int limit = 20;
            bool work = true;
            HashSet<string> uniques = new HashSet<string>();
            //List<Tuple<string, double, DateTime>> txSums = new List<Tuple<string, double, DateTime>>();
            while (work)
            {
                JToken response = GetResponse(GetApiURL("sent", offset, limit));
                if (responseStatus == 500 || response == null || response.Children().Count() == 0)
                {
                    work = false;
                    break;
                }

                foreach (JObject jo in (JArray)response)
                {
                    if (uniques.Add((string)jo["hash"]))
                    {
                        //Skip if the transaction is between system wallets
                        bool isInternal = false;
                        foreach (JObject vout in (JArray)jo["vout"])
                        {
                            if (otherWallets.Contains(vout["scriptPubKey"]["addresses"][0].ToString()))
                            {
                                isInternal = true;
                                break;
                            }
                        }
                        if (isInternal)
                            continue;

                        double txsum = 0;
                        foreach (JObject vin in jo["vin"].Where(x => x["retrievedVout"]["scriptPubKey"]["addresses"][0].ToString() == currentAddress))
                        {
                            txsum += Convert.ToDouble(vin["retrievedVout"]["value"]);
                        }
                        //Substract the amount of currency which is send back to the same address
                        foreach (JObject vout in (JArray)jo["vout"])
                        {
                            if (vout["scriptPubKey"]["addresses"][0].ToString() == currentAddress)
                            {
                                txsum -= Convert.ToDouble(vout["value"]);
                                break;
                            }
                        }

                        DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)jo["timestamp"]);
                        AddItem(txDate, txsum, (string)jo["hash"], "outcome");
                    }
                }

                offset += response.Children().Count();
            }

            SaveOffset(offsetItem, offset);
        }

        private void LoadZecRecvTx()
        {
            DBItem offsetItem = null;
            long offset = GetOffset("offset_rec", ref offsetItem);
            string currentAddress = (string)currentWallet["Address"];
            List<string> otherWallets = allWallets.Where(x => x != currentAddress).ToList();

            int limit = 20;
            //zchain explorer api limit is 20 
            bool work = true;

            while (work)
            {
                JToken response = GetResponse(GetApiURL("recv", offset > 0 ? offset : 0, limit));
                if (responseStatus == 500 || response == null || response.Children().Count() == 0)
                {
                    work = false;
                    break;
                }

                foreach (JObject jo in (JArray)response)
                {
                    //Skip if the transaction is between system wallets
                    bool isInternal = false;
                    foreach (JObject vout in (JArray)jo["vin"])
                    {
                        if (otherWallets.Contains(vout["retrievedVout"]["scriptPubKey"]["addresses"][0].ToString()))
                        {
                            isInternal = true;
                            break;
                        }
                    }
                    if (isInternal)
                        continue;

                    double txsum = 0;
                    foreach (JObject vout in (JArray)jo["vout"])
                    {
                        if (vout["scriptPubKey"]["addresses"][0].ToString() == currentAddress)
                        {
                            txsum += Convert.ToDouble(vout["value"]);
                            break;
                        }
                    }
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)jo["timestamp"]);
                    AddItem(txDate, txsum, jo["hash"].ToString(), "income");
                }
                offset += response.Children().Count();
            }
            SaveOffset(offsetItem, offset);
            //foreach (var tx in rectxSums)
            //    AddItem(tx.Item3,tx.Item2, tx.Item1, "income");
            //SaveOffset(offsetItem, offset);
            /*while (work)
            {
                JToken response = GetResponse(GetApiURL("recv", offset > 0 ? offset : 0, limit));
                if (responseStatus == 500 || response == null || response.Children().Count() == 0)
                {
                    work = false;
                    break;
                }

                foreach (JToken tx in response)
                {
                    string txId = (string)tx["hash"];
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    txDate = txDate.AddSeconds((double)tx["timestamp"]);

                    // Zajímají nás jen ty, co zatím nemáme v DB
                    if (!IsInDB(txId, currentWallet["Address"].ToString()))
                    {
                        double amountInCurrency = 0.0;
                        var voutArray = tx["vout"].Where(
                            v => (((string)v["scriptPubKey"]["addresses"][0]).ToLower())
                            .Equals(((string)currentWallet["Address"]).ToLower()));
                        foreach (JToken vout in voutArray)
                            amountInCurrency += (double)vout["value"];
                        work = AddItem(txDate, amountInCurrency, txId, "income");
                        if (work)
                            offset++;
                    }
                    else
                    {
                        offset++;
                        continue; //work = false;
                    }
                    if (!work)
                        break;
                }
            }
            SaveOffset(offsetItem, offset);*/
        }

        private void LoadEthTx()
        {
            DBItem offsetItem = null;
            long offset = GetOffset("block_start", ref offsetItem);

            bool work = true;
            string previousTxId = "";
            while (work)
            {
                JToken response = GetResponse(GetApiURL(offset));
                if (responseStatus != 200 || response == null || response["result"].Children().Count() == 0)
                {
                    work = false;
                    break;
                }

                foreach (JToken tx in response["result"])
                {
                    offset = Convert.ToInt64((string)tx["blockNumber"]);
                    string txId = (string)tx["hash"];
                    if (txId == previousTxId)
                    {
                        work = false;  //novější již není
                        break; 
                    }
                    // Zajímají nás jen ty, co zatím nemáme v DB
                    if (IsInMemory(txId, currentWallet["Address"].ToString()))
                    {
                        previousTxId = txId;
                        continue;
                    }
                    double amountInCurrency = Convert.ToDouble((string)tx["value"]) / 1000000000000000000;
                    DateTime txDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    txDate = txDate.AddSeconds(Convert.ToDouble((string)tx["timeStamp"]));

                    bool isIncome = ((string)tx["to"]).ToLower() == ((string)currentWallet["Address"]).ToLower();
                    // Pokud Wallet_Address je shodna s recipient adresou, uloz jako secondAddress sender adresu
                    string secondAddress = isIncome
                        ? ((string)tx["from"]).ToLower()
                        : ((string)tx["to"]).ToLower();

                    // Pokud je odesílatel některá z peněženek v systému, ignorujeme
                    if (isIncome && allWallets.Contains(((string)tx["from"]).ToLower()))
                        continue;

                    previousTxId = txId;
                    AddItemETH(txDate, amountInCurrency, txId, !isIncome ? "outcome" : "income", secondAddress);
                }
            }
            SaveOffset(offsetItem, offset);
        }

        private bool AddItem(DateTime txDate, double amountInCurrency, string txId, string type)
        {
            Tuple<double, bool> exchRate = GetExchangeRate(txDate);
            if (!IsInMemory(txId, (string)currentWallet["Address"]) && !IsInDB(txId, (string)currentWallet["Address"]) && !blackListAddresses.Contains(currentWallet["Address"].ToString()))
            {
                DBItem item = new DBItem(_db, null);
                item["Wallet_Address"] = currentWallet["Address"];
                item["Currency_code"] = currentWallet["Currency_code"];
                item["Transaction_date"] = txDate;
                item["Exchange_rate"] = exchRate.Item1;
                item["Amount_in_currency"] = System.Math.Abs(amountInCurrency);
                item["Amount_in_usd"] = System.Math.Abs(amountInCurrency) * exchRate.Item1;
                item["Transaction_id"] = txId;
                item["Transaction_type"] = type;
                item["ToBeFixed"] = exchRate.Item2;
                if (!transactionList.ContainsKey((int)currentWallet["id"]))
                {
                    transactionList.Add((int)currentWallet["id"], new List<DBItem>());
                }

                transactionList[(int)currentWallet["id"]].Add(item);
                txIds[(string)currentWallet["Currency_code"]].Add(new Tuple<string, string>(txId, (string)currentWallet["Address"]));
                return true;
            }
            return false;
        }

        private bool AddItemETH(DateTime txDate, double amountInCurrency, string txId, string type, string secondAddress)
        {
            Tuple<double, bool> exchRate = GetExchangeRate(txDate);
            if (!IsInMemory(txId, (string)currentWallet["Address"]) && !IsInDB(txId, (string)currentWallet["Address"]) && !blackListAddresses.Contains(currentWallet["Address"].ToString()))
            {
                DBItem item = new DBItem(_db, null);
                item["Wallet_Address"] = currentWallet["Address"];
                item["Currency_code"] = currentWallet["Currency_code"];
                item["Transaction_date"] = txDate;
                item["Exchange_rate"] = exchRate.Item1;
                item["Amount_in_currency"] = System.Math.Abs(amountInCurrency);
                item["Amount_in_usd"] = System.Math.Abs(amountInCurrency) * exchRate.Item1;
                item["Transaction_id"] = txId;
                item["Transaction_type"] = type;
                item["Second_Address"] = secondAddress;
                item["ToBeFixed"] = exchRate.Item2;
                if (!transactionList.ContainsKey((int)currentWallet["id"]))
                {
                    transactionList.Add((int)currentWallet["id"], new List<DBItem>());
                }

                transactionList[(int)currentWallet["id"]].Add(item);
                txIds[(string)currentWallet["Currency_code"]].Add(new Tuple<string, string>(txId, (string)currentWallet["Address"]));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Formerly as IsInDB
        /// </summary>
        /// <param name="txId"></param>
        /// <returns></returns>
        private bool IsInMemory(string txId, string walletAddress)
        {
            //Txid_WalletAddress tx_wa = new Txid_WalletAddress(txId, walletAddress);
            Tuple<string, string> ts = new Tuple<string, string>(txId, walletAddress);
            return txIds[(string)currentWallet["Currency_code"]].Contains(ts);
        }
       
        private bool IsInDB(string txId, string walletAddress)
        {
            return  historyTable.Select("Transaction_id", "Wallet_Address")
                        .Where(c => c.Column("Transaction_id").Equal(txId)
                        .And.Column("Wallet_Address").Equal(walletAddress))
                        .ToList().Any();
        }

        private JToken GetResponse(string url, string currency="")
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Timeout = 120000;

            switch (currency)
            {
                case "SC":
                       httpWebRequest.Credentials = new NetworkCredential(scRpcUsername, scRpcPass); //make credentials for request
                    httpWebRequest.UserAgent = "Sia-Agent";
                    break;

            }

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
                /////RevertChanges(e, url);
                responseStatus = 500;
                Watchtower.OmniusException.Log($"Aborting transaction sync. Reason: Unable to load response from remote server. API url {url} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, _db.Application, _core.User);
                return null;
            }
        }

        private void RevertChanges(Exception e, string url)
        {
            transactionList.Remove((int)currentWallet["id"]);

            Watchtower.OmniusException.Log($"Unable to load response from remote server. API url {url} (error: {e.Message})", Watchtower.OmniusLogSource.Tapestry, e, _db.Application, _core.User);
            responseStatus = 500;
        }

        private string GetApiURL()
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"]);
        }

        private string GetApiURL(long offset)
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"], offset);
        }


        private string GetApiURL(long startHeight, long endHeight)
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], startHeight, endHeight);
        }

        private string GetApiURL(long offset, int limit)
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"], offset, limit);
        }

        private string GetApiURL(string type, long offset, int limit)
        {
            return string.Format(apiList[(string)currentWallet["Currency_code"]], (string)currentWallet["Address"], type, offset, limit);
        }

        private DateTime GetLastRecordedTxDate()
        {
            var select = historyTable.Select();
            DBItem lastRecord = select.Limit(1).Where(i => i
                .Column("Wallet_Address").Equal(currentWallet["Address"]).And
                .Column("Currency_code").Equal(currentWallet["Currency_code"])
            ).Order(AscDesc.Desc, "Transaction_date").FirstOrDefault();

            return lastRecord != null ? (DateTime)lastRecord["Transaction_date"] : new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        private DateTime GetLastRecordedTxDate(string type)
        {
            var select = historyTable.Select();
            DBItem lastRecord = select.Limit(1).Where(i => i
                .Column("Wallet_Address").Equal(currentWallet["Address"]).And
                .Column("Currency_code").Equal(currentWallet["Currency_code"]).And
                .Column("Transaction_type").Equal(type)
            ).Order(AscDesc.Desc, "Transaction_date").FirstOrDefault();

            return lastRecord != null ? (DateTime)lastRecord["Transaction_date"] : new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        private Tuple<double,bool> GetExchangeRate(DateTime txDate)
        {
            try
            {
                string urlFormat = "";
                string walletCurrency = (string)currentWallet["Currency_code"];
                DateTime txDateRounded;
                //Měny, které nejsou v Poloniexu
                List<string> notInPoloniex = new List<string> { "DCR", "SC", "MUSIC" };
                if ((DateTime.UtcNow - txDate).TotalSeconds < 3600 * 24 * 7)
                {
                    if (notInPoloniex.Contains(walletCurrency.ToUpper()))
                        urlFormat = "https://min-api.cryptocompare.com/data/histominute?fsym={0}&tsym=USD&toTs={1}&limit=1";
                    else
                        urlFormat = "https://min-api.cryptocompare.com/data/histominute?fsym={0}&tsym=USD&e=Poloniex&toTs={1}&limit=1";
                    txDateRounded = RoundDateTime(txDate, new TimeSpan(0, 1, 0));
                }
                else
                {
                    if (notInPoloniex.Contains(walletCurrency.ToUpper()))
                        urlFormat = "https://min-api.cryptocompare.com/data/histohour?fsym={0}&tsym=USD&toTs={1}&limit=1";
                    else
                        urlFormat = "https://min-api.cryptocompare.com/data/histohour?fsym={0}&tsym=USD&e=Poloniex&toTs={1}&limit=1";
                    txDateRounded = RoundDateTime(txDate, new TimeSpan(1, 0, 0));
                }

                string url = string.Format(urlFormat, walletCurrency, (txDateRounded.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                JToken response = GetResponse(url, "");
                responseStatus = 200; // if GetResponse fails, continue anyway with AddItem 
                if (response != null)
                {
                    if ((string)response["Response"] == "Success")
                    {
                        JToken row = response["Data"].Last();
                        return new Tuple<double, bool>((double)row["close"], false);
                    }
                }
                //get the last exchange rate by the currency
                var lastHistoryRowByCurrency = historyTable.Select().ToList().Where(x => x["Currency_code"].ToString() == walletCurrency).OrderBy(y => y["Transaction_date"]).Last();
                return new Tuple<double, bool>((double)lastHistoryRowByCurrency["Exchange_rate"], true);

            }
            catch (Exception)
            {
                //When all fails - ie. when the history is empty
                return new Tuple<double, bool>(0, true);
            }
        }

        private DateTime RoundDateTime(DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }

        /// <summary>
        /// Get last used offset and prepare a DBItem for later saving
        /// </summary>
        /// <param name="offsetType">specific offset type (block start, offset etc.)</param>
        /// <param name="addressOffset">ref of a DBItem that can be initialized if needed</param>
        /// <returns>Zero if not found. In which case a new DBItem will be created. Otherwise returns last used offset</returns>
        private long GetOffset(string offsetType, ref DBItem addressOffset)
        {
            addressOffset = offsetTable.Select().Where(c => c.Column("Address").Equal((string)currentWallet["Address"]).And.Column("Offset_type").Equal(offsetType)).SingleOrDefault();
            if (addressOffset == null)
            {
                DBItem newAddress = new DBItem(_db, offsetTable);
                newAddress["Address"] = (string)currentWallet["Address"];
                newAddress["Offset_type"] = offsetType;
                newAddress["Offset_value"] = 0;
                offsetTable.Add(newAddress);
                _db.SaveChanges();
                //we need the id
                addressOffset = offsetTable.Select().Where(c => c.Column("Address").Equal((string)currentWallet["Address"]).And.Column("Offset_type").Equal(offsetType)).SingleOrDefault(); 
                return 0;
            }
            else
                return Convert.ToInt64(addressOffset["Offset_value"]);
        }

        private void SaveOffset(DBItem addressOffset, long offsetValue)
        {
            addressOffset["Offset_value"] = offsetValue;
            offsetTable.Update(addressOffset, (int)addressOffset["id"]);
            _db.SaveChanges();
        }
    }
}
