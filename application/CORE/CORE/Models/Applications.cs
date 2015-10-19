namespace CORE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Master_Applications")]
    public partial class Application
    {
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string DisplayName { get; set; }

        public virtual ICollection<WorkFlow> WorkFlows { get; set; }
    }
}
