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
        public int Id { get; set; }

        public string Name { get; set; }
        public int Priority { get; set; }
        
        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
        public IQueryable<User_Role> getUsers_roles(DBEntities e = null)
        {
            e = e ?? DBEntities.instance;
            return e.Users_Roles.Where(r => r.RoleName == Name);
        }

        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }

        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }
    }
}
