using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusWSService
    {
        bool CreateProxyForWS(WS model);
        JObject CallWebService(string serviceName, string methodName, object[] args);
        JObject CallRestService(string serviceName, string methodName, NameValueCollection queryParams);
    }
}
