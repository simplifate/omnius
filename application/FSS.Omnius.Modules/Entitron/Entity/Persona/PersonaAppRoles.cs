using System.ComponentModel.DataAnnotations.Schema;

using FSS.Omnius.Modules.Entitron.Entity.Master;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_AppRoles")]
    public class PersonaAppRole : IdentityRole<int, User_Role>
    {
        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }
        
        public new string Name { get; set; }

        [JsonIgnore]
        public int ADgroupId { get; set; }
        public virtual ADgroup ADgroup { get; set; }
        [Required]
        public int Priority { get; set; }
        [JsonIgnore]
        public new virtual ICollection<User_Role> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
