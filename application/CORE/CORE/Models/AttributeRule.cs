using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CORE.Models
{
    [Table("Tapestry_AttributeRoles")]
    public class AttributeRule
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string InputName { get; set; }
        [Required]
        [StringLength(50)]
        public string AttributeName { get; set; }
        
        public int BlockId { get; set; }
        public Block Block { get; set; }
    }
}