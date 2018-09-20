using System.Collections.Generic;
using System.Linq;

namespace FSS.Omnius.Modules.Tapestry2
{
    using FSS.Omnius.Modules.CORE;
    using FSS.Omnius.Modules.Entitron.Entity;
    using Transfer;

    public class TapestryCOREobject : COREbase
    {
        private TapestryCOREobject()
        {
            Context = new DBEntities(this);

            Message = new Message(this);
            Data = new Dictionary<string, object>();
            CrossBlockRegistry = new Dictionary<string, object>();
            Results = new Dictionary<string, object>();
        }

        public RequestTransaction HttpRequest { get; set; }
        public ResponseTransaction HttpResponse { get; set; }

        public Dictionary<string, object> Results { get; set; }
        
        public void Destroy()
        {
            Context.Dispose();
        }

        public static TapestryCOREobject Create(string userName, string appName)
        {
            TapestryCOREobject core = new TapestryCOREobject();
            core.Application = core.Context.Applications.SingleOrDefault(a => a.Name == appName);
            core.User = Persona.Persona.AuthenticateUser(userName, core.Application?.IsAllowedGuests ?? false);
            return core;
        }
    }
}
