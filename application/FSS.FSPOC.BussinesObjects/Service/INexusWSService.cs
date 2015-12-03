using FSS.Omnius.Modules.Entitron.Entity.Nexus;

namespace FSS.Omnius.BussinesObjects.Service
{
    public interface INexusWSService
    {
        bool CreateProxyForWS(WS model);
        object CallWebService(string serviceName, string methodName, object[] args);
    }
}
