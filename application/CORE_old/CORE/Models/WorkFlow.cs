using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CORE.Models
{
    [Table("Tapestry_WorkFlows")]
    public class WorkFlow
    {
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public virtual WorkFlow Parent { get; set; }
        public int ApplicationId { get; set; }
        public virtual Application Application { get; set; }
        public int TypeId { get; set; }
        public virtual WorkFlowType Type { get; set; }
        public int InitBlockId { get; set; }
        public virtual Block InitBlock { get; set; }

        public virtual ICollection<Block> Blocks { get; set; }
        public virtual ICollection<WorkFlow> Children { get; set; }
    }
}