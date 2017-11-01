using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class GetColdWalletBalanceAction : Action
    {
        public override int Id
        {
            get
            {
                return 19857;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] {};
            }
        }

        public override string Name
        {
            get
            {
                return "XMU Get Cold Wallet Balance Action";
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
            CORE.CORE core = (CORE.CORE)vars["__CORE__"];
            Modules.Entitron.Entitron ent = core.Entitron;

            var hotAndCold = core.Entitron.GetDynamicTable("hot_and_cold_wallets", false);
            var hotAndColdList = hotAndCold.Select().ToList();
            foreach (var coldWallet in hotAndColdList)
            {
                if (coldWallet["type"].ToString() == "cold") //only need cold
                {
                    switch (coldWallet["currency_code"].ToString())
                    {
                        case "BTC":
                            var resultBtc = GetResponse(string.Format("https://api.blockcypher.com/v1/btc/main/addrs/{0}/balance",coldWallet["address"].ToString()));
                            if (resultBtc != null)
                            {
                                coldWallet["balance"] = ((JValue)resultBtc["final_balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                        case "ETH":
                            var resultEth = GetResponse(string.Format("https://api.blockcypher.com/v1/eth/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                            if (resultEth != null)
                            {
                                coldWallet["balance"] = ((JValue)resultEth["final_balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                        case "DASH":
                            var resultDash = GetResponse(string.Format("https://api.blockcypher.com/v1/dash/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                            if (resultDash != null)
                            {
                                coldWallet["balance"] = ((JValue)resultDash["final_balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                        case "LTC":
                            var resultLtc = GetResponse(string.Format("https://api.blockcypher.com/v1/ltc/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                            if (resultLtc != null)
                            {
                                coldWallet["balance"] = ((JValue)resultLtc["final_balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                        case "DOGE":
                            var resultDoge = GetResponse(string.Format("https://api.blockcypher.com/v1/doge/main/addrs/{0}/balance", coldWallet["address"].ToString()));
                            if (resultDoge != null)
                            {
                                coldWallet["balance"] = ((JValue)resultDoge["final_balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                         case "ZEC":
                            var resultZec = GetResponse(string.Format("https://api.zcha.in/v2/mainnet/accounts/{0}", coldWallet["address"].ToString()));
                            if (resultZec != null)
                            {
                                coldWallet["balance"] = ((JValue)resultZec["balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;
                        case "XRP":
                            var resultXrp = GetResponse(string.Format("https://data.ripple.com/v2/accounts/{0}/balances", coldWallet["address"].ToString()));
                            if (resultXrp != null)
                            {
                                foreach(var b in (JArray)resultXrp["balances"])
                                {
                                    if(b["currency"].ToString() == "XRP")
                                    {
                                        coldWallet["balance"] = ((JValue)b["value"]).ToObject<double>();
                                        hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                        ent.Application.SaveChanges();
                                    }
                                }
                          
                            }
                            break;
                        case "XVG":
                            var resultXvg = GetResponse(string.Format("https://verge-blockchain.info/ext/getbalance/{0}", coldWallet["address"].ToString()));
                            if (resultXvg != null)
                            {
                                coldWallet["balance"] = ((JValue)resultXvg).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();

                            }
                            break;

                        case "NEO":
                            var resultNeo = GetResponse(string.Format("https://data.ripple.com/v2/accounts/{0}/balances", coldWallet["address"].ToString()));
                            if (resultNeo != null)
                            {
                                foreach (var b in (JArray)resultNeo["balance"])
                                {
                                    if (b["asset"].ToString() == "NEO")
                                    {
                                        coldWallet["balance"] = ((JValue)b["amount"]).ToObject<double>();
                                        hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                        ent.Application.SaveChanges();
                                    }
                                }
                            }
                            break;
                        case "ETC":
                            var resultEtc = GetResponse(string.Format("https://etcchain.com/api/v1/getAddressBalance?address={0}", coldWallet["address"].ToString()));
                            if (resultEtc != null)
                            {
                                coldWallet["balance"] = ((JValue)resultEtc["balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                                ent.Application.SaveChanges();
                            }
                            break;

                    }

                }
            }

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
            catch (Exception e)
            {
                return null;
            }
        }


    }
}
