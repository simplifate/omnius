using System.ComponentModel.DataAnnotations.Schema;

using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_AppRoles")]
    public class PersonaAppRole : IdentityRole<int, User_Role>
    {
        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }

        public int ADgroupId { get; set; }
        public virtual ADgroup ADgroup { get; set; }

        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
