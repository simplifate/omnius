namespace FSS.Omnius.Modules.Entitron.Entity.Master
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tapestry;
    using Mozaic;
    using Entitron;
    using Persona;
    using System.Linq;
    using Mozaic.Bootstrap;
    using Hermes;
    using FSS.Omnius.Modules.Entitron.DB;
    using FSS.Omnius.Modules.Entitron.Entity.Nexus;
    using FSS.Omnius.Modules.CORE;

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
            DB_Type = Modules.Entitron.Entitron.DefaultDBType;

            WorkFlows = new HashSet<WorkFlow>();
            ADgroups = new HashSet<ADgroup>();
            DesignedBy = new HashSet<User>();
            UsersApplications = new HashSet<UsersApplications>();
            MozaicEditorPages = new List<MozaicEditorPage>();
            MozaicBootstrapPages = new List<MozaicBootstrapPage>();
            DatabaseDesignerSchemeCommits = new List<DbSchemeCommit>();
            TapestryDesignerMetablocks = new HashSet<TapestryDesignerMetablock>();
            TapestryDesignerConditionGroups = new HashSet<TapestryDesignerConditionGroup>();
            DbSchemeLocked = false;
            ColumnMetadata = new HashSet<ColumnMetadata>();
            EmailTemplates = new HashSet<EmailTemplate>();
            IncomingEmailRule = new List<IncomingEmailRule>();
            TCPListeners = new List<TCPSocketListener>();
            RabbitMQs = new List<RabbitMQ>();
        }
        
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

        public bool IsAllowedForAll { get; set; }
        public bool IsAllowedGuests { get; set; }

        public bool IsPublished { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSystem { get; set; }

        public ESqlType DB_Type { get; set; }
        public string DB_ConnectionString { get; set; }
        public string DBscheme_connectionString { get; set; }

        public bool TapestryChangedSinceLastBuild { get; set; }
        public bool MozaicChangedSinceLastBuild { get; set; }
        public bool EntitronChangedSinceLastBuild { get; set; }
        public bool MenuChangedSinceLastBuild { get; set; }
        public bool DbSchemeLocked { get; set; }
        public int? SchemeLockedForUserId { get; set; }
        
        // tapestry
        public virtual ICollection<WorkFlow> WorkFlows { get; set; }
        // tapestry - designer
        [ImportExport(ELinkType.Child, Branch = "TapestryMozaic")]
        public virtual ICollection<TapestryDesignerMetablock> TapestryDesignerMetablocks { get; set; }
        public TapestryDesignerMetablock TapestryDesignerRootMetablock
        {
            get { return TapestryDesignerMetablocks.SingleOrDefault(mb => mb.ParentMetablock_Id == null); }
        }
        [ImportExport(ELinkType.Child, Branch = "TapestryMozaic")]
        public virtual ICollection<TapestryDesignerConditionGroup> TapestryDesignerConditionGroups { get; set; }

        // persona
        public virtual ICollection<ADgroup> ADgroups { get; set; }
        #warning importing role removes links to user
        //[ImportExport(ELinkType.Child, Branch = "Persona")]
        public virtual ICollection<PersonaAppRole> Roles { get; set; }
        public virtual ICollection<User> DesignedBy { get; set; }
        public virtual ICollection<UsersApplications> UsersApplications { get; set; }

        // entitron
        [ImportExport(ELinkType.Child, Branch = "Entitron")]
        public virtual ICollection<ColumnMetadata> ColumnMetadata { get; set; }
        // entitron - designer
        [ImportExport(ELinkType.Child, Branch = "Entitron")]
        public virtual ICollection<DbSchemeCommit> DatabaseDesignerSchemeCommits { get; set; }

        // mozaic
        [ImportExport(ELinkType.Child, Branch = "TapestryMozaic")]
        public virtual ICollection<MozaicEditorPage> MozaicEditorPages { get; set; }
        [ImportExport(ELinkType.Child, Branch = "TapestryMozaic")]
        public virtual ICollection<MozaicBootstrapPage> MozaicBootstrapPages { get; set; }
        [ImportExport(ELinkType.Child, Branch = "TapestryMozaic")]
        public virtual ICollection<Js> Js { get; set; }

        // hermes
        [ImportExport(ELinkType.Child, Branch = "HermesOut")]
        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; }
        //[ImportExport(ELinkType.Child, Branch = "HermesIn")]
        public virtual ICollection<IncomingEmailRule> IncomingEmailRule { get; set; }

        // Nexus
        //[ImportExport(ELinkType.Child, Branch = "Nexus")]
        public virtual ICollection<TCPSocketListener> TCPListeners { get; set; }

        //[ImportExport(ELinkType.Child, Branch = "Nexus")]
        public virtual ICollection<RabbitMQ> RabbitMQs { get; set; }

        private Application _similarApp;
        public Application similarApp
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DBscheme_connectionString))
                    return this;

                if (_similarApp == null)
                    _similarApp = COREobject.i.Context.Applications.Find(Id);
                return _similarApp;
            }
        }
    }
}
