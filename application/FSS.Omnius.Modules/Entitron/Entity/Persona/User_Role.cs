using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity.Master;
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
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }

        private PersonaAppRole _appRole;
        public PersonaAppRole AppRole
        {
            get
            {
                if (_appRole == null)
                    _appRole = COREobject.i.Context.AppRoles.SingleOrDefault(r => r.Name == RoleName && r.ApplicationId == ApplicationId);

                return _appRole;
            }
        }
    }
}
