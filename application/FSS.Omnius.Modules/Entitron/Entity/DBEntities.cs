namespace FSS.Omnius.Modules.Entitron.Entity
{
    using System.Data.Entity;
    using CORE;
    using Master;
    using Nexus;
    using Entitron;
    using Tapestry;
    using Mozaic;
    using Persona;
    using Hermes;
    using Watchtower;
    using Cortex;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Reflection;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SqlClient;
    using System.Data.Entity.Infrastructure;
    using System.Data.Common;
    using Service;
    using System.Threading;
    using Mozaic.Bootstrap;
    //michal šebela:
    //TODO: doladit zámky, bude jich potøeba víc i na jiných místech..
    //TODO: doladit pøístup z vláken, které nejsou z Http request, dispose, log db

    public partial class DBEntities : IdentityDbContext<User, PersonaAppRole, int, UserLogin, User_Role, UserClaim>
    {
        private bool isDisposed = false;

        private HttpRequest _r;
        private static HttpRequest r
        {
            get
            {
                try
                {
                    if (HttpContext.Current == null)
                        return null;    //pro pøístup z jiného vlákna - viz. WebDavFileSyncService
                    return HttpContext.Current.Request;
                }
                catch (HttpException)
                {
                    return null;
                }
            }
        }

        private static Dictionary<HttpRequest, DBEntities> _instances = new Dictionary<HttpRequest, DBEntities>();
        private static DBEntities nullRequestInstance;

        public static DBEntities instance
        {
            get
            {
                if(r == null) {
                    if(nullRequestInstance == null || nullRequestInstance.isDisposed) {
                        nullRequestInstance = new DBEntities();
                    }
                    return nullRequestInstance;
                }
                else if (!_instances.ContainsKey(r) || _instances[r].isDisposed)
                {
                    if (_instances.ContainsKey(r))
                        _instances.Remove(r);
                    Create();
                }

                return _instances[r];
                //return new DBEntities();
            }
        }

        private static object _messagesLock = new object();
        private static Dictionary<HttpRequest, List<string>> _messages = new Dictionary<HttpRequest, List<string>>();
        public static List<string> messages
        {
            get
            {
                lock (_messagesLock)
                {
                    return _messages.ContainsKey(r) ? _messages[r] : new List<string>();
                }
            }
        }

        private static object _connectionsLock = new object();
        private static Dictionary<HttpRequest, DbConnection> _connections = new Dictionary<HttpRequest, DbConnection>();
        private static DbConnection connection
        {
            get
            {
                lock (_connectionsLock)
                {
                    if (!_connections.ContainsKey(r))
                    {
                        SqlConnectionFactory f = new SqlConnectionFactory(Omnius.Modules.Entitron.Entitron.connectionString);
                        _connections.Add(r, f.CreateConnection(Omnius.Modules.Entitron.Entitron.connectionString));
                        _connections[r].Open();
                    }
                    return _connections[r];
                }
            }
        }

        public static void Create()
        {
            _instances.Add(r, new DBEntities(r));
        }

        public static void Destroy()
        {
            lock (_connectionsLock)
            {
                _connections[r].Close();
                _connections[r].Dispose();
                _connections.Remove(r);
            }

            _instances[r].Dispose();

            _instances.Remove(r);

            lock (_messagesLock)
            {
                _messages.Remove(r);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (r != null)
            {
                _instances[r].isDisposed = true;
            }
        }

        public static DBEntities GetInstance()
        {
            return instance;
        }

        public DBEntities(HttpRequest req) : base(connection, false)
        {
            _r = req;
            Database.Log = s => Log(s);
        }

        public DBEntities() : base(FSS.Omnius.Modules.Entitron.Entitron.connectionString)
        {
        }

        public void Log(string message)
        {
            lock (_messagesLock)
            {
                if (!_messages.ContainsKey(_r))
                {
                    _messages.Add(_r, new List<string>());
                }
                _messages[_r].Add(message);
            }
        }

        // CORE
        public virtual DbSet<ConfigPair> ConfigPairs { get; set; }

        // Entitron
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<DbSchemeCommit> DBSchemeCommits { get; set; }
        public virtual DbSet<DbTable> DbTables { get; set; }
        public virtual DbSet<DbColumn> DbColumn { get; set; }
        public virtual DbSet<DbIndex> DbIndex { get; set; }
        public virtual DbSet<DbRelation> DbRelation { get; set; }
        public virtual DbSet<DbView> DbView { get; set; }
        public virtual DbSet<ColumnMetadata> ColumnMetadata { get; set; }

        // Hermes
        public virtual DbSet<EmailLog> EmailLogItems { get; set; }
        public virtual DbSet<EmailPlaceholder> EmailPlaceholders { get; set; }
        public virtual DbSet<EmailQueue> EmailQueueItems { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailTemplateContent> EmailContents { get; set; }
        public virtual DbSet<Smtp> SMTPs { get; set; }

        public virtual DbSet<IncomingEmail> IncomingEmail { get; set; }
        public virtual DbSet<IncomingEmailRule> IncomingEmailRule { get; set; }

        // Cortex
        public virtual DbSet<Task> Tasks { get; set; }

        // Master
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<UsersApplications> UsersApplications { get; set; }

        // Mozaic
        public virtual DbSet<Js> Js { get; set; }
        public virtual DbSet<Css> Css { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateCategory> TemplateCategories { get; set; }
        public virtual DbSet<MozaicEditorPage> MozaicEditorPages { get; set; }
        public virtual DbSet<MozaicEditorComponent> MozaicEditorComponents { get; set; }
        public virtual DbSet<MozaicCssTemplate> CssTemplates { get; set; }
        public virtual DbSet<MozaicBootstrapPage> MozaicBootstrapPages { get; set; }
        public virtual DbSet<MozaicBootstrapComponent> MozaicBootstrapComponents { get; set; }

        // Nexus
        public virtual DbSet<ExtDB> ExtDBs { get; set; }
        public virtual DbSet<WebDavServer> WebDavServers { get; set; }
        public virtual DbSet<FileMetadata> FileMetadataRecords { get; set; }
        public virtual DbSet<FileSyncCache> CachedFiles { get; set; }
        public virtual DbSet<Ldap> Ldaps { get; set; }
        public virtual DbSet<WS> WSs { get; set; }

        // Persona
        public virtual DbSet<ActionRuleRight> ActionRuleRights { get; set; }
        public virtual DbSet<ADgroup> ADgroups { get; set; }
        public virtual DbSet<ADgroup_User> ADgroup_Users { get; set; }
        public virtual DbSet<ModuleAccessPermission> ModuleAccessPermissions { get; set; }
        public virtual DbSet<User_Role> UserRoles { get; set; }

        //public virtual DbSet<PersonaAppRole> Roles { get; set; }

        // Tapestry
        public virtual DbSet<ActionRule> ActionRules { get; set; }
        public virtual DbSet<ActionRule_Action> ActionRule_Action { get; set; }
        public virtual DbSet<ActionSequence_Action> ActionSequence_Actions { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<AttributeRule> AttributeRules { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<PreBlockAction> PreBlockActions { get; set; }
        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowType> WorkFlowTypes { get; set; }
        public virtual DbSet<ResourceMappingPair> ResourceMappingPairs { get; set; }

        public virtual DbSet<TapestryDesignerMetablock> TapestryDesignerMetablocks { get; set; }
        public virtual DbSet<TapestryDesignerBlock> TapestryDesignerBlocks { get; set; }
        public virtual DbSet<TapestryDesignerMetablockConnection> TapestryDesignerMetablockConnections { get; set; }
        public virtual DbSet<TapestryDesignerWorkflowRule> TapestryDesignerWorkflowRules { get; set; }
        public virtual DbSet<TapestryDesignerWorkflowItem> TapestryDesignerWorkflowItems { get; set; }
        public virtual DbSet<TapestryDesignerResourceRule> TapestryDesignerResourceRules { get; set; }
        public virtual DbSet<TapestryDesignerResourceItem> TapestryDesignerResourceItems { get; set; }
        public virtual DbSet<TapestryDesignerConditionSet> TapestryDesignerConditionSets { get; set; }
        public virtual DbSet<TapestryDesignerCondition> TapestryDesignerConditions { get; set; }

        // Watchtower
        public virtual DbSet<LogItem> LogItems { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // CORE

            // Entitron
            modelBuilder.Entity<Application>()
                .HasMany<Table>(e => e.Tables)
                .WithOptional(e => e.Application)
                .HasForeignKey(e => e.ApplicationId);

            // Hermes
            modelBuilder.Entity<EmailTemplate>()
                .HasMany(s => s.PlaceholderList)
                .WithOptional(s => s.Hermes_Email_Template)
                .HasForeignKey(s => s.Hermes_Email_Template_Id);

            modelBuilder.Entity<EmailTemplate>()
                .HasMany(s => s.ContentList)
                .WithOptional(s => s.Hermes_Email_Template)
                .HasForeignKey(s => s.Hermes_Email_Template_Id);

            modelBuilder.Entity<IncomingEmailRule>()
                .HasRequired(r => r.Application)
                .WithMany(a => a.IncomingEmailRule)
                .HasForeignKey(r => r.ApplicationId);

            modelBuilder.Entity<IncomingEmailRule>()
                .HasRequired(r => r.IncomingEmail)
                .WithMany(e => e.IncomingEmailRule)
                .HasForeignKey(r => r.IncomingEmailId);

            // Cortex
            modelBuilder.Entity<Task>()
                .HasOptional(t => t.Application);

            // Master
            modelBuilder.Entity<Application>()
                .HasOptional(e => e.CssTemplate);

            // Mozaic
            //modelBuilder.Entity<Application>()
            //    .HasMany<Page>(e => e.Pages);
            //    .WithRequired(e => e.Application);

            modelBuilder.Entity<Page>()
                .HasMany<Block>(e => e.Blocks)
                .WithOptional(e => e.MozaicPage)
                .HasForeignKey(e => e.MozaicPageId);

            modelBuilder.Entity<Js>()
                .HasRequired(e => e.Application)
                .WithMany(e => e.Js)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Js>()
                .HasOptional(e => e.Page)
                .WithMany(e => e.Js)
                .HasForeignKey(e => e.MozaicBootstrapPageId);

            modelBuilder.Entity<Css>()
                .HasMany<Page>(e => e.Pages)
                .WithMany(e => e.Css)
                .Map(m => m.ToTable("Mozaic_CssPages").MapLeftKey("CssId").MapRightKey("PageId"));

            //           modelBuilder.Entity<Template>()
            //               .HasMany<Page>(e => e.Pages);
            ////               .WithRequired(e => e.MasterTemplate);

            modelBuilder.Entity<TemplateCategory>()
                .HasMany<Template>(e => e.Templates)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<TemplateCategory>()
                .HasMany<TemplateCategory>(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<MozaicEditorPage>()
                .HasMany(e => e.Components)
                .WithRequired(e => e.MozaicEditorPage);
            modelBuilder.Entity<MozaicEditorComponent>()
                .HasMany(e => e.ChildComponents)
                .WithOptional(e => e.ParentComponent)
                .HasForeignKey(e => e.ParentComponentId);

            modelBuilder.Entity<MozaicBootstrapPage>()
                .HasMany(e => e.Components)
                .WithRequired(e => e.MozaicBootstrapPage);
            modelBuilder.Entity<MozaicBootstrapComponent>()
                .HasMany(e => e.ChildComponents)
                .WithOptional(e => e.ParentComponent)
                .HasForeignKey(e => e.ParentComponentId);
            
            modelBuilder.Entity<Application>()
                .HasMany(e => e.MozaicEditorPages)
                .WithRequired(e => e.ParentApp);
            modelBuilder.Entity<Application>()
                .HasMany(e => e.MozaicBootstrapPages)
                .WithRequired(e => e.ParentApp);

            /*modelBuilder.Entity<TapestryDesignerBlock>()
                .HasMany(e => e.Pages)
                .WithOptional(e => e.AssociatedBlock);*/

            // Nexus
            modelBuilder.Entity<FileMetadata>()
                .HasOptional<WebDavServer>(s => s.WebDavServer);

            modelBuilder.Entity<FileMetadata>()
                .HasOptional<FileSyncCache>(s => s.CachedCopy)
                .WithRequired(s => s.FileMetadata);

            // Persona
            modelBuilder.Entity<PersonaAppRole>()
                .HasRequired<Application>(e => e.Application)
                .WithMany(e => e.Roles)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<ModuleAccessPermission>()
                .HasRequired(e => e.User)
                .WithOptional(e => e.ModuleAccessPermission)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ActionRule>()
                .HasMany<ActionRuleRight>(e => e.ActionRuleRights)
                .WithRequired(e => e.ActionRule)
                .HasForeignKey(e => e.ActionRuleId);

            modelBuilder.Entity<PersonaAppRole>()
                .HasMany<ActionRuleRight>(e => e.ActionRuleRights)
                .WithRequired(e => e.AppRole)
                .HasForeignKey(e => e.AppRoleId);

            modelBuilder.Entity<ADgroup>()
                .HasMany<ADgroup_User>(e => e.ADgroup_Users)
                .WithRequired(e => e.ADgroup)
                .HasForeignKey(e => e.ADgroupId);

            modelBuilder.Entity<User>()
                .HasMany<ADgroup_User>(e => e.ADgroup_Users)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<User>()
                .HasMany<User_Role>(e => e.Roles)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<PersonaAppRole>()
                .HasMany<User_Role>(e => e.Users)
                .WithRequired(e => e.AppRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<Application>()
                .HasMany<User>(e => e.DesignedBy)
                .WithOptional(e => e.DesignApp)
                .HasForeignKey(e => e.DesignAppId);

            // Tapestry
            modelBuilder.Entity<Application>()
                .HasMany<WorkFlow>(e => e.WorkFlows)
                .WithRequired(e => e.Application)
                .HasForeignKey(e => e.ApplicationId);

            modelBuilder.Entity<Application>()
                .HasMany<ColumnMetadata>(e => e.ColumnMetadata)
                .WithRequired(e => e.Application);

            modelBuilder.Entity<WorkFlowType>()
                .HasMany<WorkFlow>(e => e.WorkFlows)
                .WithRequired(e => e.Type)
                .HasForeignKey(e => e.TypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkFlow>()
                .HasMany<WorkFlow>(e => e.Children)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Block>()
                .HasMany<WorkFlow>(e => e.InitForWorkFlow)
                .WithOptional(e => e.InitBlock)
                .HasForeignKey(e => e.InitBlockId);

            modelBuilder.Entity<WorkFlow>()
                .HasMany<Block>(e => e.Blocks)
                .WithRequired(e => e.WorkFlow)
                .HasForeignKey(e => e.WorkFlowId);

            modelBuilder.Entity<Block>()
                .HasMany<AttributeRule>(e => e.AttributeRules)
                .WithRequired(e => e.Block)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Block>()
                .HasMany<ResourceMappingPair>(e => e.ResourceMappingPairs)
                .WithRequired(e => e.Block)
                .HasForeignKey(e => e.BlockId);

            modelBuilder.Entity<Block>()
                .HasMany<ActionRule>(e => e.SourceTo_ActionRules)
                .WithRequired(e => e.SourceBlock)
                .HasForeignKey(e => e.SourceBlockId);

            modelBuilder.Entity<Block>()
                .HasMany<ActionRule>(e => e.TargetTo_ActionRules)
                .WithRequired(e => e.TargetBlock)
                .HasForeignKey(e => e.TargetBlockId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ActionRule>()
                .HasMany<ActionRule_Action>(e => e.ActionRule_Actions)
                .WithRequired(e => e.ActionRule)
                .HasForeignKey(e => e.ActionRuleId);

            modelBuilder.Entity<Actor>()
                .HasMany<ActionRule>(e => e.ActionRules)
                .WithRequired(e => e.Actor)
                .HasForeignKey(e => e.ActorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ActionRule_Action>()
                .HasRequired<ActionRule>(a => a.ActionRule)
                .WithMany(a => a.ActionRule_Actions);

            modelBuilder.Entity<PreBlockAction>()
                .HasRequired<Block>(e => e.Block)
                .WithMany(e => e.PreBlockActions);

            modelBuilder.Entity<TapestryDesignerResourceRule>()
                .HasMany<TapestryDesignerResourceConnection>(e => e.Connections)
                .WithRequired(e => e.ResourceRule)
                .HasForeignKey(e => e.ResourceRuleId);
            modelBuilder.Entity<TapestryDesignerResourceConnection>()
                .HasRequired<TapestryDesignerResourceItem>(e => e.Source)
                .WithMany(e => e.SourceToConnection)
                .HasForeignKey(e => e.SourceId);
            modelBuilder.Entity<TapestryDesignerResourceConnection>()
                .HasRequired<TapestryDesignerResourceItem>(e => e.Target)
                .WithMany(e => e.TargetToConnection)
                .HasForeignKey(e => e.TargetId);

            modelBuilder.Entity<TapestryDesignerWorkflowRule>()
                .HasMany<TapestryDesignerWorkflowConnection>(e => e.Connections)
                .WithRequired(e => e.WorkflowRule)
                .HasForeignKey(e => e.WorkflowRuleId);
            modelBuilder.Entity<TapestryDesignerWorkflowConnection>()
                .HasRequired<TapestryDesignerWorkflowItem>(e => e.Source)
                .WithMany(e => e.SourceToConnection)
                .HasForeignKey(e => e.SourceId);
            modelBuilder.Entity<TapestryDesignerWorkflowConnection>()
                .HasRequired<TapestryDesignerWorkflowItem>(e => e.Target)
                .WithMany(e => e.TargetToConnection)
                .HasForeignKey(e => e.TargetId);

            modelBuilder.Entity<TapestryDesignerBlock>()
                .HasMany<TapestryDesignerWorkflowItem>(e => e.TargetFor)
                .WithOptional(e => e.Target)
                .HasForeignKey(e => e.TargetId);

            // Database Designer
            modelBuilder.Entity<DbTable>()
                        .HasMany(s => s.Columns)
                        .WithRequired(s => s.DbTable);
            modelBuilder.Entity<DbTable>()
                        .HasMany(s => s.Indices)
                        .WithRequired(s => s.DbTable);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Tables)
                        .WithRequired(s => s.DbSchemeCommit);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Relations)
                        .WithRequired(s => s.DbSchemeCommit);
            modelBuilder.Entity<DbSchemeCommit>()
                        .HasMany(s => s.Views)
                        .WithRequired(s => s.DbSchemeCommit);
            modelBuilder.Entity<Application>()
                        .HasMany(e => e.DatabaseDesignerSchemeCommits);

            modelBuilder.Entity<TapestryDesignerMetablock>()
                .HasRequired<Application>(s => s.ParentApp)
                .WithMany(s => s.TapestryDesignerMetablocks)
                .HasForeignKey(e => e.ParentAppId);
            modelBuilder.Entity<TapestryDesignerMetablock>()
                .HasMany(s => s.Metablocks)
                .WithOptional(s => s.ParentMetablock)
                .HasForeignKey(x => x.ParentMetablock_Id);
            modelBuilder.Entity<TapestryDesignerMetablock>()
                .HasMany(s => s.Blocks)
                .WithRequired(s => s.ParentMetablock)
                .HasForeignKey(x => x.ParentMetablock_Id)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<TapestryDesignerBlock>()
                .HasMany(s => s.BlockCommits)
                .WithRequired(s => s.ParentBlock)
                .HasForeignKey(x => x.ParentBlock_Id)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<TapestryDesignerBlockCommit>()
                .HasMany(s => s.ResourceRules)
                .WithRequired(s => s.ParentBlockCommit)
                .HasForeignKey(x => x.ParentBlockCommit_Id)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<TapestryDesignerBlockCommit>()
                .HasMany(s => s.WorkflowRules)
                .WithRequired(s => s.ParentBlockCommit)
                .HasForeignKey(x => x.ParentBlockCommit_Id)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<TapestryDesignerWorkflowRule>()
                .HasMany(s => s.Swimlanes)
                .WithRequired(s => s.ParentWorkflowRule)
                .HasForeignKey(x => x.ParentWorkflowRule_Id)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<TapestryDesignerSwimlane>()
                .HasMany(s => s.WorkflowItems);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Actions);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Attributes);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.UiComponents);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Roles);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.States);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Targets);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Templates);
            modelBuilder.Entity<TapestryDesignerToolboxState>()
                .HasMany(s => s.Integrations);

            // Watchtower
        }

        public void DiscardChanges()
        {
            var changedEntries = ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged).ToList();

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Modified))
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }

            foreach (var entry in changedEntries.Where(x => x.State == EntityState.Deleted))
            {
                entry.State = EntityState.Unchanged;
            }
        }

        //public void Remove(IEntity item)
        //{
        //    Entry(item).State = EntityState.Deleted;
        ////dynamic list = getList(item.GetType());

        ////list.Remove((dynamic)item);
        //}
        //public void RemoveRange(IEnumerable<IEntity> items)
        //{
        //    foreach (IEntity item in items)
        //        Remove(item);
        //}
        //private object getList(Type type)
        //{
        //    PropertyInfo prop = GetType().GetProperties().SingleOrDefault(p => p.PropertyType.GenericTypeArguments.FirstOrDefault() == type || p.PropertyType.GenericTypeArguments.FirstOrDefault() == type.BaseType);
        //    return prop.GetValue(this);
        //}
    }
}
