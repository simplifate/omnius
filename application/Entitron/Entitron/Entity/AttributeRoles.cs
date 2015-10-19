namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tapestry_AttributeRoles")]
    public partial class AttributeRoles
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string InputName { get; set; }

        [Required]
        [StringLength(50)]
        public string AttributeName { get; set; }

        public int BlockId { get; set; }

        public virtual Blocks Tapestry_Blocks { get; set; }
    }
}
