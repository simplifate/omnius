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
                return new string[] { };
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
            var result2 = SendJsonOverTCP("194.145.180.207", 18123,initJson,inputJson);

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
