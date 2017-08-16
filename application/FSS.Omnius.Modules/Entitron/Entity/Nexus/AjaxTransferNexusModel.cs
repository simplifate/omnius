using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Nexus
{
    public class AjaxTransferNexusModel : IEntity
    {
        public List<AjaxTransferNexusLDAP> Ldap { get; set; }
        public List<AjaxTrasferNexuWS> WS { get; set; }
        public List<AjaxTransferHermesSMTP> SMTP { get; set; }
        public List<AjaxTransferNexusWebDAV> WebDAV { get; set; }
        public List<AjaxTransferNexusExtDB> ExtDB { get; set; }

        public AjaxTransferNexusModel()
        {
            Ldap = new List<AjaxTransferNexusLDAP>();
            WS = new List<AjaxTrasferNexuWS>();
            SMTP = new List<AjaxTransferHermesSMTP>();
            WebDAV = new List<AjaxTransferNexusWebDAV>();
            ExtDB = new List<AjaxTransferNexusExtDB>();
        }
    }

    public class AjaxTransferNexusLDAP : IEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }

    public class AjaxTrasferNexuWS : IEntity
    {
        public int? Id { get; set; }
        public WSType Type { get; set; }
        public string Name { get; set; }
    }
    
    public class AjaxTransferNexusWebDAV : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AjaxTransferNexusExtDB : IEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public class AjaxTransferHermesSMTP : IEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
