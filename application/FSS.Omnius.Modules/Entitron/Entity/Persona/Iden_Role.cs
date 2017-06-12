using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    // DON'T USE, JUST FOR Identity
    [Table("Persona_Identity_Roles")]
    public class Iden_Role : IdentityRole<int, Iden_User_Role>
    {
        public new int Id { get; set; }
    }
}
