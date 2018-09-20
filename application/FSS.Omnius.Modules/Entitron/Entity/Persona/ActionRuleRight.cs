using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ActionRuleRights")]
    public partial class ActionRuleRight : IEntity
    {
        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string AppRoleName { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ApplicationId { get; set; }
        [Key]
        [Column(Order = 3)]
        public int ActionRuleId { get; set; }
        
        public bool Executable { get; set; }
        
        public virtual ActionRule ActionRule { get; set; }
    }
}
