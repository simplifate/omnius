using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tapestry.Models
{
    [Table("Tapestry_ActionRole_Action")]
    public class ActionRole_Action
    {
        [Required]
        public int Order { get; set; }
        [StringLength(200)]
        public string ResultVariables { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int ActionRoleId { get; set; }
        public virtual ActionRole ActionRole { get; set; }
        [Key]
        [Column(Order = 2)]
        [Required]
        public int ActionId { get; set; }
        public virtual Action Action { get; set; }
    }
}