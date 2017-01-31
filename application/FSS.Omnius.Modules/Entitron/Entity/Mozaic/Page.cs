namespace FSS.Omnius.Modules.Entitron.Entity.Mozaic
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tapestry;

    [Table("Mozaic_Pages")]
    public partial class Page : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Page()
        {
            Blocks = new HashSet<Block>();
            Css = new HashSet<Css>();
        }

        public int Id { get; set; }

        public string ViewName { get; set; }

        public string ViewPath { get; set; }

        public string ViewContent { get; set; }

        public bool IsBootstrap { get; set; }

        public virtual ICollection<Block> Blocks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Css> Css { get; set; }
    }
}
