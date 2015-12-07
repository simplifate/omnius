using FSS.Omnius.Modules.Nexus.Gate;
using Newtonsoft.Json.Linq;

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

        public JObject CallWebService(string serviceName, string methodName, object[] args)
        {
            return ws.CallWebService(serviceName, methodName, args);
        }

    }
}
