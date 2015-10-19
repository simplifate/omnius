using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CORE.Models
{
    [Table("Tapestry_Actors")]
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<ActionRole> ActionRoles { get; set; }
    }
}