using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusWSService
    {
        bool CreateProxyForWS(WS model);
        JObject CallWebService(string serviceName, string methodName, object[] args);
    }
}
