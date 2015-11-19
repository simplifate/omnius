using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FSS.Omnius.Entitron.Entity.Tapestry;

namespace FSS.Omnius.Entitron.Entity.Persona
{
    [Table("Persona_ActionRights")]
    public class ActionRight
    {
        [Key]
        [Column(Order = 1)]
        public int GroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ActionId { get; set; }
        
        public bool Readable { get; set; }
        public bool Executable { get; set; }
        
        public virtual Group Group { get; set; }
        public virtual Action Action { get; set; }
    }
}
