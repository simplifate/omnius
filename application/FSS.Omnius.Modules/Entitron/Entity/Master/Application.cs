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
    using Newtonsoft.Json;
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
            
            WorkFlows = new HashSet<WorkFlow>();
            Tables = new HashSet<Table>();
            ADgroups = new HashSet<ADgroup>();

            MozaicEditorPages = new List<MozaicEditorPage>();
            DatabaseDesignerSchemeCommits = new List<DbSchemeCommit>();
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
        [JsonIgnore]
        public virtual ICollection<WorkFlow> WorkFlows { get; set; }

        public virtual ICollection<ADgroup> ADgroups { get; set; }
        [JsonIgnore]
        public virtual ICollection<Table> Tables { get; set; }

        // Workflow, UI and DB designers' data
        public virtual ICollection<MozaicEditorPage> MozaicEditorPages { get; set; }
        public virtual ICollection<DbSchemeCommit> DatabaseDesignerSchemeCommits { get; set; }
        [JsonIgnore]
        public virtual TapestryDesignerMetablock TapestryDesignerRootMetablock { get; set; }
    }
}
