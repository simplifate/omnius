namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tapestry_WorkFlows")]
    public partial class WorkFlow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkFlow()
        {
            Tapestry_Blocks = new HashSet<Block>();
            Tapestry_WorkFlows1 = new HashSet<WorkFlow>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InitBlockId { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int ApplicationId { get; set; }

        public int TypeId { get; set; }

        public virtual Application Master_Applications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Block> Tapestry_Blocks { get; set; }

        public virtual Block Tapestry_Blocks1 { get; set; }

        public virtual WorkFlow_Type Tapestry_WorkFlow_Types { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkFlow> Tapestry_WorkFlows1 { get; set; }

        public virtual WorkFlow Tapestry_WorkFlows2 { get; set; }
    }
}
