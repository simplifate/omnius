using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Mozaic.Models
{
    [Table("Mozaic_Css")]
    public class Css
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
        
        public virtual ICollection<Page> Pages { get; set; }
    }
}