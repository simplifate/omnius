using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using FSS.Omnius.Modules.Nexus.Gate;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public class NexusWSService : INexusWSService
    {
        WS ws;

        public NexusWSService()
        {
            ws = new WS();
        }

        public bool CreateProxyForWS(Omnius.Modules.Entitron.Entity.Nexus.WS model)
        {
            return ws.CreateProxyForWS(model);
        }

        public JToken CallWebService(string serviceName, string methodName, string jsonBody)
        {
            return ws.CallWebService(serviceName, methodName, jsonBody);
        }

        public JToken CallWebService(string serviceName, string methodName, object[] args)
        {
            return ws.CallWebService(serviceName, methodName, args);
        }

        public JToken CallRestService(string serviceName, string methodName, NameValueCollection queryParams)
        {
            return ws.CallRestService(serviceName, methodName, queryParams);
        }

    }
}
