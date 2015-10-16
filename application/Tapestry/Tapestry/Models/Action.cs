using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tapestry.Models
{
    [Table("Tapestry_Actions")]
    public class Action
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        public string MethodName { get; set; }
        [StringLength(200)]
        public string RequiredAttributes { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public virtual ActionCategory Category { get; set; }
        public virtual ICollection<ActionRole_Action> ActionRoles { get; set; }
    }
}