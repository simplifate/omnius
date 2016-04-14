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
            DatabaseDesignerSchemeCommits = new List<DbSchemeCommit>();
            DbSchemeLocked = false;
            ColumnMetadata = new HashSet<ColumnMetadata>();
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

        public bool IsPublished { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSystem { get; set; }

        [StringLength(100)]
        public string DisplayName { get; set; } // Used by Entitron
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkFlow> WorkFlows { get; set; }

        [JsonIgnore]
        public virtual ICollection<ADgroup> ADgroups { get; set; }
        public virtual ICollection<PersonaAppRole> Roles { get; set; }

        [JsonIgnore]
        public virtual ICollection<Table> Tables { get; set; }
        [JsonIgnore]
        public virtual ICollection<UsersApplications> UsersApplications {get;set;}
        // Workflow, UI and DB designers' data
        public virtual ICollection<MozaicEditorPage> MozaicEditorPages { get; set; }
        public virtual ICollection<DbSchemeCommit> DatabaseDesignerSchemeCommits { get; set; }
        public bool DbSchemeLocked { get; set; }
        public virtual ICollection<ColumnMetadata> ColumnMetadata { get; set; }
        public virtual ICollection<TapestryDesignerMetablock> TapestryDesignerMetablocks { get; set; }

        [JsonIgnore]
        public TapestryDesignerMetablock TapestryDesignerRootMetablock
        {
            get
            {
                return TapestryDesignerMetablocks.SingleOrDefault(mb => mb.ParentMetablock_Id == null);
            }
        }

        [JsonIgnore]
        public ICollection<User> DesignedBy { get; set; }
    }
}
