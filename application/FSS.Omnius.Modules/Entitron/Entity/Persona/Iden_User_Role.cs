using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    // DON'T USE, JUST FOR Identity
    [Table("Persona_Identity_UserRoles")]
    public class Iden_User_Role : IdentityUserRole<int>, IEntity
    {
        [Key]
        [Column(Order = 1)]
        public new virtual int UserId { get; set; }
        [Key]
        [Column(Order = 2)]
        public new virtual int RoleId { get; set; }
    }
}
