using FSS.Omnius.Modules.Nexus.Gate;

namespace FSS.Omnius.BussinesObjects.Service
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

        public object CallWebService(string serviceName, string methodName, object[] args)
        {
            return ws.CallWebService(serviceName, methodName, args);
        }

    }
}
