using System.ComponentModel.DataAnnotations.Schema;

using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_AppRoles")]
    public class PersonaAppRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string MembersList { get; set; }

        public virtual Application Application { get; set; }
    }
}
