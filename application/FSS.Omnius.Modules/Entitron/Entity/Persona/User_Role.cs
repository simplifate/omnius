using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_User_Role")]
    public partial class User_Role : IdentityUserRole<int>
    {
        [Key]
        [Column(Order = 1)]
        public new virtual int UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public new virtual int RoleId { get; set; }

        public virtual User User { get; set; }
        public virtual PersonaAppRole AppRole { get; set; }
    }
}
