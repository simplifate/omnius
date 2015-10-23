using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Entity
{
    [Table("Persona_Rights")]
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
