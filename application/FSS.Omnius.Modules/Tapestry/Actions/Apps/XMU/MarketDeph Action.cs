using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.DB;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class MarketDephAction : Action
    {
        public static int requestId = 0;
        public override int Id => 19856;

        public override string[] InputVar => new string[] { "IpAddress", "Port", "CurencyPair" };

        public override string Name => "Market Deph Action";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            DBConnection db = Modules.Entitron.Entitron.i;

            string pair = vars["CurencyPair"].ToString();
            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{pair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"Orderbook.get\", \"params\": [], \"id\": {requestId++ + 1}}}";
            string ipAddress = vars["IpAddress"].ToString();
            int port = Convert.ToInt32(vars["Port"]);
            var result = SendJsonOverTCP(ipAddress, port, initJson, inputJson);
            var orderCashTable = db.Table("order_book", false);

            foreach (var order in (JArray)result["result"])
            {
                DBItem row = new DBItem(db, orderCashTable);
                row["order_id"] = order[0].ToString();
                row["buy_sell"] = order[1].ToString();
                row["price"] = (double)order[2];
                row["amount"] = (double)order[3];
                row["pair"] = pair;
                orderCashTable.Add(row);
            }
            db.SaveChanges();

        }

        public JObject SendJsonOverTCP(string host, int port, string initJson, string json, int receivedBufferSize = 20060)
        {
            var receiveInitBytes = new byte[receivedBufferSize];
            var receiveBytes = new byte[receivedBufferSize];
            string responseJson = "";

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = 10000;
                socket.Connect(host, port);
                socket.Send(Encoding.ASCII.GetBytes(initJson + "\n"));

                socket.Receive(receiveInitBytes, receiveInitBytes.Length, SocketFlags.None);

                socket.Send(Encoding.ASCII.GetBytes(json + "\n"));

                do
                {
                    socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.Partial);

                    responseJson += Encoding.ASCII.GetString(receiveBytes);
                    receiveBytes = new byte[receivedBufferSize];
                }
                while (socket.Available > 0);

                socket.Shutdown(SocketShutdown.Both);
            }

            JObject Parsed = string.IsNullOrEmpty(responseJson) ? new JObject() : JObject.Parse(responseJson);
            return Parsed;
        }
    }
}