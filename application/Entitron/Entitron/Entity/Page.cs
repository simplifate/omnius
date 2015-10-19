namespace Entitron.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Mozaic_Pages")]
    public partial class Page
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Page()
        {
            Mozaic_Css = new HashSet<Css>();
        }

        public int Id { get; set; }

        [Required]
        public string Relations { get; set; }

        public int MasterTemplateId { get; set; }

        public int ApplicationId { get; set; }

        public virtual Application Master_Applications { get; set; }

        public virtual Template Mozaic_Template { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Css> Mozaic_Css { get; set; }
    }
}
