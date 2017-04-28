using FSS.Omnius.Modules.Entitron.Entity.Master;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_User_Role")]
    public partial class User_Role : IEntity
    {
        public int Id { get; set; }
        [Index]
        public int UserId { get; set; }
        [Index]
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }
        public int ApplicationId { get; set; }

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }
        public PersonaAppRole getAppRole(DBEntities e = null)
        {
            e = e ?? DBEntities.instance;
            return e.AppRoles.SingleOrDefault(r => r.Name == RoleName);
        }
    }
}
