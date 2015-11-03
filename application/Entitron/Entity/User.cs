using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitron.Entity
{
    [Table("Persona_Users")]
    public partial class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string username { get; set; }
        [Required]
        public string passwordHash { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
