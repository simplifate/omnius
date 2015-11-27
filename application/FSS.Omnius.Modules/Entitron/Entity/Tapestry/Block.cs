using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Mozaic;

    [Table("Tapestry_Run_Blocks")]
    public partial class Block
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Block()
        {
            SourceTo_ActionRoles = new HashSet<ActionRule>();
            TargetTo_ActionRoles = new HashSet<ActionRule>();
            AttributeRoles = new HashSet<AttributeRole>();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRule> SourceTo_ActionRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRule> TargetTo_ActionRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttributeRole> AttributeRoles { get; set; }

        public virtual WF WorkFlow { get; set; }

        public virtual WF InitForWorkFlow { get; set; }
    }
}
