using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using FSS.Omnius.Modules.CORE;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{
    [NexusRepository]
    public class CallRestAction : Action
    {
        public static int requestId = 0;
        public override int Id => 300212;

        public override string[] InputVar => new string[] { "IpAddress", "Port", "CurencyPair", "Method", "Params", "?SkipInit" };

        public override string Name => "Call JsonRPC Over TCP";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string pair = vars.ContainsKey("CurrencyPair") ? vars["CurencyPair"].ToString() : "";
            string method = vars["Method"].ToString();
            string parameters = vars.ContainsKey("Params") ? vars["Params"].ToString() : "";
            bool skipInit = vars.ContainsKey("SkipInit") ? (bool)vars["SkipInit"] : false; ;

            string initJson = skipInit ? "" : $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{pair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"{method}\", \"params\": {parameters} \"id\": {requestId++ + 1}}}";
            string ipAddress = vars["IpAddress"].ToString();
            int port = Convert.ToInt32(vars["Port"]);

            var result = SendJsonOverTCP(ipAddress, port, initJson, inputJson, skipInit);
            outputVars["Result"] = result;
     
        }
        public JObject SendJsonOverTCP(string host, int port, string initJson, string json, bool skipInit = false, int receivedBufferSize = 20060)
        {
            var receiveBytes = new byte[receivedBufferSize];
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.ReceiveTimeout = 10000;
                socket.Connect(host, port);
                if(!skipInit)
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
