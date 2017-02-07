namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tapestry;
    using Mozaic;
    using Entitron;
    using Persona;
    using Newtonsoft.Json;
    using System.Linq;
    using Mozaic.Bootstrap;
    [Table("Master_Applications")]
    public partial class Application : IEntity
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
            DesignedBy = new HashSet<User>();
            UsersApplications = new HashSet<UsersApplications>();
            MozaicEditorPages = new List<MozaicEditorPage>();
            MozaicBootstrapPages = new List<MozaicBootstrapPage>();
            DatabaseDesignerSchemeCommits = new List<DbSchemeCommit>();
            TapestryDesignerMetablocks = new HashSet<TapestryDesignerMetablock>();
            DbSchemeLocked = false;
            ColumnMetadata = new HashSet<ColumnMetadata>();
        }

        [ImportExportIgnore(IsKey = true)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [StringLength(100)]
        public string DisplayName { get; set; }
        public string Icon { get; set; }
        public int TitleFontSize { get; set; }
        public int Color { get; set; }
        public string InnerHTML { get; set; }
        public string LaunchCommand { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public bool IsPublished { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSystem { get; set; }

        // empty for default, database name or full connection string
        public string connectionString_data { get; set; }
        public string connectionString_schema { get; set; }

        public bool TapestryChangedSinceLastBuild { get; set; }
        public bool MozaicChangedSinceLastBuild { get; set; }
        public bool EntitronChangedSinceLastBuild { get; set; }
        public bool MenuChangedSinceLastBuild { get; set; }
        public bool DbSchemeLocked { get; set; }

        [ImportExportIgnore]
        public virtual MozaicCssTemplate CssTemplate { get; set; }

        // tapestry
        [ImportExportIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkFlow> WorkFlows { get; set; }
        // tapestry - designer
        public virtual ICollection<TapestryDesignerMetablock> TapestryDesignerMetablocks { get; set; }
        [ImportExportIgnore]
        public TapestryDesignerMetablock TapestryDesignerRootMetablock
        {
            get { return TapestryDesignerMetablocks.SingleOrDefault(mb => mb.ParentMetablock_Id == null); }
        }

        // persona
        [ImportExportIgnore]
        public virtual ICollection<ADgroup> ADgroups { get; set; }
        public virtual ICollection<PersonaAppRole> Roles { get; set; }
        [ImportExportIgnore]
        public ICollection<User> DesignedBy { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<UsersApplications> UsersApplications { get; set; }

        // entitron
        [ImportExportIgnore]
        public virtual ICollection<Table> Tables { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<ColumnMetadata> ColumnMetadata { get; set; }
        // entitron - designer
        public virtual ICollection<DbSchemeCommit> DatabaseDesignerSchemeCommits { get; set; }
        
        // mozaic
        public virtual ICollection<MozaicEditorPage> MozaicEditorPages { get; set; }
        public virtual ICollection<MozaicBootstrapPage> MozaicBootstrapPages { get; set; }
        public virtual ICollection<Js> Js { get; set; }
    }
}
