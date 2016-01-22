using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Entity.Persona
{
    [Table("Persona_ADgroup_User")]
    public partial class ADgroup_User
    {
        [Key]
        [Column(Order = 1)]
        public int ADgroupId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int UserId { get; set; }

        public virtual ADgroup ADgroup { get; set; }
        public virtual User User { get; set; }
    }
}
