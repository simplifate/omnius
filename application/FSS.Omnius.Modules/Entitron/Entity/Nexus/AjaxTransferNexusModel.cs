using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    public class AjaxTransferNexusModel
    {
        public List<AjaxTransferNexusLDAP> Ldap { get; set; }
        public List<AjaxTrasferNexuWS> WS { get; set; }
        public List<AjaxTransferHermesSMTP> SMTP { get; set; }
        public List<AjaxTransferNexusWebDAV> WebDAV { get; set; }

        public AjaxTransferNexusModel()
        {
            Ldap = new List<AjaxTransferNexusLDAP>();
            WS = new List<AjaxTrasferNexuWS>();
            SMTP = new List<AjaxTransferHermesSMTP>();
            WebDAV = new List<AjaxTransferNexusWebDAV>();
        }
    }

    public class AjaxTransferNexusLDAP
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }

    public class AjaxTrasferNexuWS
    {
        public int? Id { get; set; }
        public WSType Type { get; set; }
        public string Name { get; set; }
    }
    
    public class AjaxTransferNexusWebDAV
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AjaxTransferHermesSMTP
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
