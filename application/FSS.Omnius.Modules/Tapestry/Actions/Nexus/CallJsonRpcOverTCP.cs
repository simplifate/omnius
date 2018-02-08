using System;
using System.Collections.Generic;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Tapestry.Actions.Nexus;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Watchtower;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;

namespace FSS.Omnius.Modules.Tapestry.Actions.Nexus
{

    [NexusRepository]
    public class CallRestAction : Action
    {
        public static int requestId = 0;
        public override int Id
        {
            get
            {
                return 300212;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "IpAddress", "Port", "CurencyPair","Method","Params"};
            }
        }

        public override string Name
        {
            get
            {
                return "Call JsonRPC Over TCP";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
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
            string pair = vars["CurencyPair"].ToString();
            string method = vars["Method"].ToString();
            string parameters = vars.ContainsKey("Params") ? vars["Params"].ToString() : "";

            string initJson = $"{{\"jsonrpc\": \"2.0\", \"method\": \"init\", \"params\": {{\"market\" : \"{pair}\"}}, \"id\": {requestId++}}}";
            string inputJson =
                    $"{{\"jsonrpc\": \"2.0\", \"method\": \"{method}\", \"params\": {parameters} \"id\": {requestId++ + 1}}}";
            string ipAddress = vars["IpAddress"].ToString();
            int port = Convert.ToInt32(vars["Port"]);

            var result = SendJsonOverTCP(ipAddress, port, initJson, inputJson);
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
