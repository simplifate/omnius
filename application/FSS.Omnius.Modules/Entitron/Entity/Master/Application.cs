namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using Tapestry;
    using Mozaic;
    using Entitron;
    using Persona;

    [Table("Master_Applications")]
    public partial class Application
    {
        public Application()
        {
            TileWidth = 2;
            TileHeight = 1;
            Color = 0;
            Icon = "fa-question";
            TitleFontSize = 20;

            Pages = new HashSet<Page>();
            WorkFlows = new HashSet<WorkFlow>();
            Tables = new HashSet<Table>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public string Icon { get; set; }
        public int TitleFontSize { get; set; }
        public int Color { get; set; }
        public string InnerHTML { get; set; }
        public string LaunchCommand { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool IsPublished { get; set; }
        public bool IsEnabled { get; set; }

        [StringLength(100)]
        public string DisplayName { get; set; } // Used by Entitron

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Page> Pages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkFlow> WorkFlows { get; set; }

        public virtual ICollection<AppRight> Rights { get; set; }

        public virtual ICollection<Table> Tables { get; set; }

        public virtual ICollection<PersonaAppRole> Roles { get; set; }
    }
}
