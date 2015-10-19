namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tapestry_Blocks")]
    public partial class Block
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Block()
        {
            Tapestry_ActionRoles = new HashSet<ActionRole>();
            Tapestry_ActionRoles1 = new HashSet<ActionRole>();
            Tapestry_AttributeRoles = new HashSet<AttributeRole>();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRole> Tapestry_ActionRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActionRole> Tapestry_ActionRoles1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AttributeRole> Tapestry_AttributeRoles { get; set; }

        public virtual WorkFlow Tapestry_WorkFlows { get; set; }

        public virtual WorkFlow Tapestry_WorkFlows1 { get; set; }
    }
}
