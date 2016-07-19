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
            Users = new HashSet<User_Role>();
        }

        [ImportExportIgnore(IsKey = true)]
        public new int Id { get; set; }
        public new string Name { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int ApplicationId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual Application Application { get; set; }
        [Required]
        public int Priority { get; set; }
        [ImportExportIgnore]
        public new virtual ICollection<User_Role> Users { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
