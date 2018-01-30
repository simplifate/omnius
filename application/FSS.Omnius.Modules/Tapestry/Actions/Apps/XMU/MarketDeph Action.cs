using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class MarketDephAction : Action
    {
        public static int requestId = 0;
        public override int Id
        {
            get
            {
                return 19856;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] {"IpAddress","Port", "Market"};
            }
        }

        public override string Name
        {
            get
            {
                return "Market Deph Action";
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
            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"btcusd\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"Orderbook.get\", \"params\": [], \"id\": {requestId++ + 1}}}";
            string ipAddress = vars["IpAddress"].ToString();
            int port = Convert.ToInt32(vars["Port"]);
            var result = SendJsonOverTCP(ipAddress, port,initJson,inputJson);
            var orderCashTable = core.Entitron.GetDynamicTable("order_book", false);
            core.Entitron.TruncateTable("order_book");
            core.Entitron.Application.SaveChanges();

            foreach (var order in (JArray)result["result"])
            {
                DBItem row = new DBItem();
                row.createProperty(0, "order_id", order[0].ToString());
                row.createProperty(1, "buy_sell", order[1].ToString());
                row.createProperty(2, "price", (double)order[2]);
                row.createProperty(3, "amount", (double)order[3]);
                orderCashTable.Add(row);
                core.Entitron.Application.SaveChanges();
            }

        }

        public JObject SendJsonOverTCP(string host, int port,string initJson, string json, int receivedBufferSize = 20060)
        {
            var receiveBytes = new byte[receivedBufferSize];
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = 10000;
                socket.Connect(host, port);
                socket.Send(Encoding.ASCII.GetBytes(initJson + "\n"));

         

                socket.Send(Encoding.ASCII.GetBytes(json + "\n"));

                socket.Shutdown(SocketShutdown.Send);
                socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);

                socket.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);
            }
            var responseJson = Encoding.ASCII.GetString(receiveBytes);
            return JObject.Parse(responseJson);
        }
    }
}
