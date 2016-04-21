using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_UserClaim")]
    public class UserClaim : IdentityUserClaim<int>, IEntity
    {
    }
}
