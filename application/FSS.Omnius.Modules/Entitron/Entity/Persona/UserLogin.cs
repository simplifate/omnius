using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_UserLogin")]
    public class UserLogin : IdentityUserLogin<int>, IEntity
    {
        public int Id { get; set; }
    }
}
