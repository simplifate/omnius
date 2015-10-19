using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CORE.Models
{
    [Table("Tapestry_ActionCategories")]
    public class ActionCategory
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public virtual ActionCategory Parent { get; set; }
        public virtual ICollection<ActionCategory> Children { get; set; }
        public virtual ICollection<Action> Actions { get; set; }
    }
}