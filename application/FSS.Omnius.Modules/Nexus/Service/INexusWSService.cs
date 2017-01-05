using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace FSS.Omnius.Modules.Nexus.Service
{
    public interface INexusWSService
    {
        bool CreateProxyForWS(WS model);
        JToken CallWebService(string serviceName, string methodName, string jsonBody);
        JToken CallWebService(string serviceName, string methodName, object[] args);
        JToken CallRestService(string serviceName, string methodName, NameValueCollection queryParams);
    }
}
