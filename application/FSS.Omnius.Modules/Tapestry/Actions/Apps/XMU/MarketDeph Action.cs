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

        public override string[] InputVar => new string[] { "IpAddress", "Port", "CurencyPair", "Method", "?Params" };

        public override string Name => "Market Deph Action";

        public override string[] OutputVar => new string[0];

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string method = vars["Method"].ToString();
            string parameters = "[]";
            if (vars["Params"] != null)
            {
                parameters = $"[{vars["Params"]}]";
            }
            string pair = vars["CurencyPair"].ToString();
            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{pair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"{method}\", \"params\": {parameters}, \"id\": {requestId++ + 1}}}";
            string ipAddress = vars["IpAddress"].ToString();
            int port = Convert.ToInt32(vars["Port"]);
            var result = SendJsonOverTCP(ipAddress, port,initJson,inputJson);
            outputVars["Result"] = result;
        }

        public JObject SendJsonOverTCP(string host, int port, string initJson, string json, int receivedBufferSize = 20060)
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
