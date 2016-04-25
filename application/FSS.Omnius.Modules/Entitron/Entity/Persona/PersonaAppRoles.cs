using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;

    [Table("Persona_AppRoles")]
    public class PersonaAppRole : IdentityRole<int, User_Role>, IEntity
    {
        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }
        
        public new string Name { get; set; }

        [JsonIgnore]
        public int ApplicationId { get; set; }
        [JsonIgnore]
        public virtual Application Application { get; set; }
        [Required]
        public int Priority { get; set; }
        [JsonIgnore]
        public new virtual ICollection<User_Role> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
