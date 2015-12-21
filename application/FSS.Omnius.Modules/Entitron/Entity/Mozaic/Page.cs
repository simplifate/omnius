namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Master;
    using Tapestry;

    [Table("Mozaic_Pages")]
    public partial class Page
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Page()
        {
            Blocks = new HashSet<Block>();
            Css = new HashSet<Css>();
        }

        public int Id { get; set; }

        [Required]
        public string Relations { get; set; }

        public int MasterTemplateId { get; set; }

        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }

        public virtual Template MasterTemplate { get; set; }

        public virtual ICollection<Block> Blocks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Css> Css { get; set; }
    }
}
