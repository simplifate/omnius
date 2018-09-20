using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Jayrock.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FSS.Omnius.Modules.Tapestry2.Actions.App
{
    public class XMU : ActionManager
    {
        [Action(1984, "Parse Exchange Rates")]
        public static void ParseExchangeRates(COREobject core, JObject JToken)
        {
            DBConnection db = core.Entitron;
            DBTable table = db.Table("exchange_rates");
            
            JObject data = (JObject)JToken["result"];

            foreach (var row in data)
            {
                string exchange;
                string pair;
                float value = ((JValue)row.Value).ToObject<float>();

                string key = row.Key;

                string[] splitString = key.Split(':');

                exchange = splitString[0];
                pair = splitString[1];

                DBItem item = new DBItem(db, table);
                item["exchange"] = exchange;
                item["pair"] = pair;
                item["value"] = value;

                table.Add(item);
            }
            db.SaveChanges();
        }

        [Action(1985, "Create MoneyServer signature", "Result")]
        public static List<string> CreateMSSignature(COREobject core, string Data = "", string Controller = "config")
        {
            string privateKey = Controller == "config" ? ConfigurationManager.AppSettings["MS_configController"].ToString() : ConfigurationManager.AppSettings["MS_moneyController"].ToString();
            if (Controller == "config")
                privateKey = ConfigurationManager.AppSettings["MS_configController"].ToString();
            else if (Controller == "money")
                privateKey = ConfigurationManager.AppSettings["MS_moneyController"].ToString();
            else
                throw new InvalidParameterException($"CreateMSSignatureAction: Invalid controller type {Controller}");

            string timestamp = GetCurrentTimestamp();

            byte[] data = Encoding.UTF8.GetBytes(string.Format("{0}@{1}", Data, timestamp));

            StringReader reader = new StringReader(privateKey);
            PemObject pem = (new PemReader(reader)).ReadPemObject();
            AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(pem.Content);

            ISigner signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, key);
            signer.BlockUpdate(data, 0, data.Length);

            byte[] result = signer.GenerateSignature();
            byte[] encodedBytes;
            using (MemoryStream encStream = new MemoryStream())
            {
                Base64.Encode(result, 0, result.Length, encStream);
                encodedBytes = encStream.ToArray();
            }

            List<string> headers = new List<string>();
            headers.Add(string.Format("X-MS-Signature: {0}", Encoding.UTF8.GetString(encodedBytes)));
            headers.Add(string.Format("X-MS-Timestamp: {0}", timestamp));

            return headers;
        }
        private static string GetCurrentTimestamp()
        {
            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return string.Format("{0}", System.Math.Floor(unixTime));
        }

        [Action(19856, "Market Deph Action")]
        public static void MarketDeph(COREobject core, string CurencyPair, string IpAddress, int Port)
        {
            DBConnection db = core.Entitron;
            
            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{CurencyPair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"Orderbook.get\", \"params\": [], \"id\": {requestId++ + 1}}}";

            var result = SendJsonOverTCP(IpAddress, Port, initJson, inputJson);
            var orderCashTable = db.Table("order_book", false);

            foreach (var order in (JArray)result["result"])
            {
                DBItem row = new DBItem(db, orderCashTable);
                row["order_id"] = order[0].ToString();
                row["buy_sell"] = order[1].ToString();
                row["price"] = (double)order[2];
                row["amount"] = (double)order[3];
                row["pair"] = CurencyPair;
                orderCashTable.Add(row);
            }
            db.SaveChanges();
        }
        private static JObject SendJsonOverTCP(string host, int port, string initJson, string json, int receivedBufferSize = 20060)
        {
            var receiveInitBytes = new byte[receivedBufferSize];
            var receiveBytes = new byte[receivedBufferSize];
            string responseJson = "";

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = 10000;
                socket.Connect(host, port);
                socket.Send(Encoding.UTF8.GetBytes(initJson + "\n"));

                socket.Receive(receiveInitBytes, receiveInitBytes.Length, SocketFlags.None);

                socket.Send(Encoding.UTF8.GetBytes(json + "\n"));

                do
                {
                    socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.Partial);

                    responseJson += Encoding.UTF8.GetString(receiveBytes);
                    receiveBytes = new byte[receivedBufferSize];
                }
                while (socket.Available > 0);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            JObject Parsed = string.IsNullOrEmpty(responseJson) ? new JObject() : JObject.Parse(responseJson);
            return Parsed;
        }
        public static int requestId = 0;

        [Action(19857, "XMU Get Cold Wallet Balance Action")]
        public static void GetColdWalletBalance(COREobject core)
        {
            DBConnection db = core.Entitron;
            int idRpc = 0;

            DBTable hotAndCold = db.Table("hot_and_cold_wallets", false);
            var hotAndColdList = hotAndCold.Select().ToList();
            foreach (var coldWallet in hotAndColdList)
            {
                try
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
                            int outId;
                            var resultXmr = GetResponse(idRpc, out outId);
                            idRpc = outId;
                            if (resultXmr != null)
                            {
                                coldWallet["balance"] = ((JValue)resultXmr["balance"]).ToObject<double>();
                                hotAndCold.Update(coldWallet, Convert.ToInt32(coldWallet["id"]));
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    core.Message.Errors.Add(e.Message);
                }
            }
            db.SaveChanges();
        }
        private static JToken GetResponse(string url)
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
        private static JToken GetResponse(int idRpc, out int outId)
        {
            outId = idRpc;
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
                outId = idRpc;

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
        private const string XmrRpcUsername = "grid";
        private const string XmrRpcPass = "g149u6W3_e8/Wkqi";
    }
}
