using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    using FSS.Omnius.Modules.CORE;
    using Master;
    using System.Linq;

    [Table("Persona_AppRoles")]
    public class PersonaAppRole : IEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Index("PersonaAppRole_AppName", IsUnique = true, Order = 2)]
        public string Name { get; set; }
        public int Priority { get; set; }

        public virtual ICollection<ActionRuleRight> ActionRuleRights { get; set; }
        public IQueryable<User_Role> getUsers_roles(DBEntities e = null)
        {
            e = e ?? COREobject.i.Context;
            return e.Users_Roles.Where(r => r.RoleName == Name && r.ApplicationId == ApplicationId);
        }

        [ImportExport(ELinkType.Parent, typeof(Application))]
        [Index("PersonaAppRole_AppName", IsUnique = true, Order = 1)]
        public int ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }

        public PersonaAppRole()
        {
            ActionRuleRights = new HashSet<ActionRuleRight>();
        }
    }
}
