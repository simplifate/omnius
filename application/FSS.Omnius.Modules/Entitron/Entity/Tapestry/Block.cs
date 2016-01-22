using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Mozaic;

    [Table("Tapestry_Blocks")]
    public partial class Block
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Block()
        {
            SourceTo_ActionRules = new HashSet<ActionRule>();
            TargetTo_ActionRules = new HashSet<ActionRule>();
            AttributeRules = new HashSet<AttributeRule>();
            InitForWorkFlow = new HashSet<WorkFlow>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ModelName { get; set; }

        public bool IsVirtual { get; set; }
        
        public int WorkFlowId { get; set; }

        public int? MozaicPageId { get; set; }
        public virtual Page MozaicPage { get; set; }
        
        public virtual ICollection<PreBlockAction> PreBlockActions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRule> SourceTo_ActionRules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRule> TargetTo_ActionRules { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttributeRule> AttributeRules { get; set; }
        
        public virtual WorkFlow WorkFlow { get; set; }
        
        public virtual ICollection<WorkFlow> InitForWorkFlow { get; set; }
    }
}
