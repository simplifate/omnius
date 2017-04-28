using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using Master;
    using System.Linq;

    [Table("Persona_AppRoles")]
    public class PersonaAppRole : IEntity
    {
        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }

        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }

        [ImportExportIgnore(IsParentKey = true)]
        public int ApplicationId { get; set; }
        [ImportExportIgnore(IsParent = true)]
        public virtual Application Application { get; set; }
        [Required]
        public int Priority { get; set; }
        public IQueryable<User_Role> getUsers_roles(DBEntities e = null)
        {
            e = e ?? DBEntities.instance;
            return e.Users_Roles.Where(r => r.RoleName == Name);
        }

        [ImportExportIgnore]
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
    }
}
