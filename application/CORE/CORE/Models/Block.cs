using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CORE.Models
{
    [Table("Tapestry_Blocks")]
    public class Block
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string ModelName { get; set; }
        public bool IsVirtual { get; set; }
        public int? MozaicPageId { get; set; }

        public int WorkFlowId { get; set; }
        public virtual WorkFlow WorkFlow { get; set; }
        public virtual WorkFlow InitToWF { get; set; }
        public virtual ICollection<ActionRole> SourceToActions { get; set; }
        public virtual ICollection<ActionRole> TargetToACtions { get; set; }
        public virtual ICollection<AttributeRule> AttributeRules { get; set; }
    }
}