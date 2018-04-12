using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Jayrock.Json;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GetColdWalletBalanceAction : Action
    {
        private const string XmrRpcUsername = "grid";
        private const string XmrRpcPass = "g149u6W3_e8/Wkqi";
        private int idRpc = 0;

        public override int Id => 19857;

        public override string[] InputVar => new string[] { };

        public override string Name => "XMU Get Cold Wallet Balance Action";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            DBTable hotAndCold = db.Table("hot_and_cold_wallets", false);
            var hotAndColdList = hotAndCold.Select().ToList();
            foreach (var coldWallet in hotAndColdList)
            {
                switch (coldWallet["currency_code"].ToString())
                {
                    case "btc":
                        var resultBtc = GetResponse(string.Format("https://api.blockcypher.com/v1/btc/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                        if (resultBtc != null)
                        {
                            coldWallet["balance"] = ((JValue)resultBtc["final_balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "eth":
                        var resultEth = GetResponse(string.Format("http://api.etherscan.io/api?module=account&action=balance&address={0}&tag=latest", coldWallet["address"].ToString()));
                        if (resultEth != null)
                        {
                            coldWallet["balance"] = ((JValue)resultEth["result"]).ToObject<double>() / 1000000000000000000.0;
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "dash":
                        var resultDash = GetResponse(string.Format("https://api.blockcypher.com/v1/dash/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                        if (resultDash != null)
                        {
                            coldWallet["balance"] = ((JValue)resultDash["final_balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "ltc":
                        var resultLtc = GetResponse(string.Format("https://api.blockcypher.com/v1/ltc/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                        if (resultLtc != null)
                        {
                            coldWallet["balance"] = ((JValue)resultLtc["final_balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "doge":
                        var resultDoge = GetResponse(string.Format("https://api.blockcypher.com/v1/doge/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                        if (resultDoge != null)
                        {
                            coldWallet["balance"] = ((JValue)resultDoge["final_balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "zec":
                        var resultZec = GetResponse(string.Format("https://api.zcha.in/v2/mainnet/accounts/{0}", coldWallet["address"].ToString()));
                        if (resultZec != null)
                        {
                            coldWallet["balance"] = ((JValue)resultZec["balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                    case "xrp":
                        var resultXrp = GetResponse(string.Format("https://data.ripple.com/v2/accounts/{0}/balances", coldWallet["address"].ToString()));
                        if (resultXrp != null)
                        {
                            foreach (var b in (JArray)resultXrp["balances"])
                            {
                                if (b["currency"].ToString() == "XRP")
                                {
                                    coldWallet["balance"] = ((JValue)b["value"]).ToObject<double>();
                                    hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                }
                            }

                        }
                        break;
                    case "xvg":
                        var resultXvg = GetResponse(string.Format("https://verge-blockchain.info/ext/getbalance/{0}", coldWallet["address"].ToString()));
                        if (resultXvg != null && resultXvg is JValue)
                        {
                            coldWallet["balance"] = ((JValue)resultXvg).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));

                        }
                        break;

                    case "neo":
                        var resultNeo = GetResponse(string.Format("https://data.ripple.com/v2/accounts/{0}/balances", coldWallet["address"].ToString()));
                        if (resultNeo != null)
                        {
                            foreach (var b in (JArray)resultNeo["balance"])
                            {
                                if (b["asset"].ToString() == "NEO")
                                {
                                    coldWallet["balance"] = ((JValue)b["amount"]).ToObject<double>();
                                    hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                }
                            }
                        }
                        break;
                    case "etc":
                        var resultEtc = GetResponse(string.Format("https://etcchain.com/api/v1/getAddressBalance?address={0}", coldWallet["address"].ToString()));
                        if (resultEtc != null)
                        {
                            coldWallet["balance"] = ((JValue)resultEtc["balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;

                    case "xmr":
                        var resultXmr = GetResponse();
                        if (resultXmr != null)
                        {
                            coldWallet["balance"] = ((JValue)resultXmr["balance"]).ToObject<double>();
                            hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                        }
                        break;
                }
            }
            db.SaveChanges();
        }

        private JToken GetResponse(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Timeout = 120000;

            try
            {
                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                string outputJsonString = responseReader.ReadToEnd();


                if (!string.IsNullOrEmpty(outputJsonString))
                {
                    return JToken.Parse(outputJsonString);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private JToken GetResponse()
        {
            try
            {
                string method = "getbalance";

                string rpcVersion = "2.0";
                //get username and password from rpc service 
                string rpcUserName = XmrRpcUsername;
                string rpcPassword = XmrRpcPass;

                // Create request
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://185.59.209.146:8093/json_rpc");
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Credentials = new NetworkCredential(rpcUserName, rpcPassword); //make credentials for request
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json";

                Dictionary<string, object> paramsDict = new Dictionary<string, object>();
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
                    return jtokenResult;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
