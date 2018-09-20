namespace FSS.Omnius.Modules.Migrations.MySQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tapestry_ActionRule_Action",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActionRuleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        VirtualAction = c.String(unicode: false),
                        VirtualItemId = c.Int(),
                        VirtualParentId = c.Int(),
                        IsForeachStart = c.Boolean(),
                        IsForeachEnd = c.Boolean(),
                        InputVariablesMapping = c.String(maxLength: 2000, storeType: "nvarchar"),
                        OutputVariablesMapping = c.String(maxLength: 2000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_ActionRule", t => t.ActionRuleId)
                .Index(t => t.ActionRuleId);
            
            CreateTable(
                "dbo.Tapestry_ActionRule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        PreFunctionCount = c.Int(nullable: false),
                        ConditionGroupId = c.Int(),
                        isDefault = c.Boolean(nullable: false),
                        SourceBlockId = c.Int(nullable: false),
                        TargetBlockId = c.Int(nullable: false),
                        ExecutedBy = c.String(unicode: false),
                        ActorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Actors", t => t.ActorId)
                .ForeignKey("dbo.TapestryDesigner_ConditionGroups", t => t.ConditionGroupId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.SourceBlockId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.TargetBlockId)
                .Index(t => t.ConditionGroupId)
                .Index(t => t.SourceBlockId)
                .Index(t => t.TargetBlockId)
                .Index(t => t.ActorId);
            
            CreateTable(
                "dbo.Persona_ActionRuleRights",
                c => new
                    {
                        ApplicationId = c.Int(nullable: false),
                        AppRoleName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ActionRuleId = c.Int(nullable: false),
                        Executable = c.Boolean(nullable: false),
                        PersonaAppRole_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.ApplicationId, t.AppRoleName, t.ActionRuleId })
                .ForeignKey("dbo.Tapestry_ActionRule", t => t.ActionRuleId)
                .ForeignKey("dbo.Persona_AppRoles", t => t.PersonaAppRole_Id)
                .Index(t => t.ActionRuleId)
                .Index(t => t.PersonaAppRole_Id);
            
            CreateTable(
                "dbo.Tapestry_Actors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.TapestryDesigner_ConditionGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceMappingPairId = c.Int(),
                        TapestryDesignerResourceItemId = c.Int(),
                        TapestryDesignerWorkflowItemId = c.Int(),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.TapestryDesignerResourceItemId)
                .ForeignKey("dbo.TapestryDesigner_WorkflowItems", t => t.TapestryDesignerWorkflowItemId)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ResourceMappingPairId)
                .Index(t => t.TapestryDesignerResourceItemId)
                .Index(t => t.TapestryDesignerWorkflowItemId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Master_Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        DisplayName = c.String(maxLength: 100, storeType: "nvarchar"),
                        Icon = c.String(unicode: false),
                        TitleFontSize = c.Int(nullable: false),
                        Color = c.Int(nullable: false),
                        InnerHTML = c.String(unicode: false),
                        LaunchCommand = c.String(unicode: false),
                        TileWidth = c.Int(nullable: false),
                        TileHeight = c.Int(nullable: false),
                        IsAllowedForAll = c.Boolean(nullable: false),
                        IsAllowedGuests = c.Boolean(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        IsEnabled = c.Boolean(nullable: false),
                        IsSystem = c.Boolean(nullable: false),
                        DB_Type = c.Int(nullable: false),
                        DB_ConnectionString = c.String(unicode: false),
                        DBscheme_connectionString = c.String(unicode: false),
                        TapestryChangedSinceLastBuild = c.Boolean(nullable: false),
                        MozaicChangedSinceLastBuild = c.Boolean(nullable: false),
                        EntitronChangedSinceLastBuild = c.Boolean(nullable: false),
                        MenuChangedSinceLastBuild = c.Boolean(nullable: false),
                        DbSchemeLocked = c.Boolean(nullable: false),
                        SchemeLockedForUserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Persona_ADgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        isAdmin = c.Boolean(nullable: false),
                        RoleForApplication = c.String(unicode: false),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Persona_ADgroup_User",
                c => new
                    {
                        ADgroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ADgroupId, t.UserId })
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .ForeignKey("dbo.Persona_ADgroups", t => t.ADgroupId)
                .Index(t => t.ADgroupId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Company = c.String(maxLength: 100, storeType: "nvarchar"),
                        Department = c.String(maxLength: 100, storeType: "nvarchar"),
                        Team = c.String(maxLength: 100, storeType: "nvarchar"),
                        WorkPhone = c.String(maxLength: 20, storeType: "nvarchar"),
                        MobilPhone = c.String(maxLength: 20, storeType: "nvarchar"),
                        Address = c.String(maxLength: 500, storeType: "nvarchar"),
                        Job = c.String(maxLength: 100, storeType: "nvarchar"),
                        isLocalUser = c.Boolean(nullable: false),
                        Locale = c.Int(nullable: false),
                        CurrentLogin = c.DateTime(nullable: false, precision: 0),
                        LastLogin = c.DateTime(nullable: false, precision: 0),
                        LastLogout = c.DateTime(precision: 0),
                        LastAction = c.DateTime(precision: 0),
                        DeletedBySync = c.DateTime(precision: 0),
                        LastIp = c.String(maxLength: 50, storeType: "nvarchar"),
                        LastAppCookie = c.String(unicode: false),
                        localExpiresAt = c.DateTime(nullable: false, precision: 0),
                        isActive = c.Boolean(nullable: false),
                        DesignAppId = c.Int(),
                        Email = c.String(unicode: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        PhoneNumber = c.String(unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(precision: 0),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.DesignAppId)
                .Index(t => t.DesignAppId);
            
            CreateTable(
                "dbo.Persona_UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_UserLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(unicode: false),
                        ProviderKey = c.String(unicode: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_ModuleAccessPermissions",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        Core = c.Boolean(nullable: false),
                        Master = c.Boolean(nullable: false),
                        Tapestry = c.Boolean(nullable: false),
                        Entitron = c.Boolean(nullable: false),
                        Mozaic = c.Boolean(nullable: false),
                        Persona = c.Boolean(nullable: false),
                        Nexus = c.Boolean(nullable: false),
                        Sentry = c.Boolean(nullable: false),
                        Hermes = c.Boolean(nullable: false),
                        Athena = c.Boolean(nullable: false),
                        Watchtower = c.Boolean(nullable: false),
                        Cortex = c.Boolean(nullable: false),
                        Babylon = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_Identity_UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Iden_Role_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .ForeignKey("dbo.Persona_Identity_Roles", t => t.Iden_Role_Id)
                .Index(t => t.UserId)
                .Index(t => t.Iden_Role_Id);
            
            CreateTable(
                "dbo.Persona_User_Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ApplicationName = c.String(unicode: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleName)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Master_UsersApplications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .ForeignKey("dbo.Persona_Users", t => t.UserId)
                .Index(t => new { t.UserId, t.ApplicationId }, unique: true, name: "IX_userApp");
            
            CreateTable(
                "dbo.Entitron_ColumnMetadata",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ColumnName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ColumnDisplayName = c.String(maxLength: 100, storeType: "nvarchar"),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => new { t.ApplicationId, t.TableName, t.ColumnName }, unique: true, name: "UX_Entitron_ColumnMetadata");
            
            CreateTable(
                "dbo.Entitron_DbSchemeCommit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommitMessage = c.String(unicode: false),
                        Timestamp = c.DateTime(nullable: false, precision: 0),
                        IsComplete = c.Boolean(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Entitron_DbRelation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Type = c.Int(nullable: false),
                        SourceTableId = c.Int(nullable: false),
                        SourceColumnId = c.Int(nullable: false),
                        TargetTableId = c.Int(nullable: false),
                        TargetColumnId = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbColumn", t => t.SourceColumnId)
                .ForeignKey("dbo.Entitron_DbTable", t => t.SourceTableId)
                .ForeignKey("dbo.Entitron_DbColumn", t => t.TargetColumnId)
                .ForeignKey("dbo.Entitron_DbTable", t => t.TargetTableId)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId)
                .Index(t => t.SourceTableId)
                .Index(t => t.SourceColumnId)
                .Index(t => t.TargetTableId)
                .Index(t => t.TargetColumnId)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Entitron_DbColumn",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        DisplayName = c.String(unicode: false),
                        PrimaryKey = c.Boolean(nullable: false),
                        Unique = c.Boolean(nullable: false),
                        AllowNull = c.Boolean(nullable: false),
                        Type = c.String(unicode: false),
                        ColumnLength = c.Int(nullable: false),
                        ColumnLengthIsMax = c.Boolean(nullable: false),
                        DefaultValue = c.String(unicode: false),
                        DbTableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbTable", t => t.DbTableId)
                .Index(t => t.DbTableId);
            
            CreateTable(
                "dbo.Entitron_DbTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Entitron_DbIndex",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Unique = c.Boolean(nullable: false),
                        ColumnNames = c.String(unicode: false),
                        DbTableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbTable", t => t.DbTableId)
                .Index(t => t.DbTableId);
            
            CreateTable(
                "dbo.Entitron_DbView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Query = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Hermes_Email_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Is_HTML = c.Boolean(nullable: false),
                        AppId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.AppId)
                .Index(t => new { t.AppId, t.Name }, unique: true, name: "HermesUniqueness");
            
            CreateTable(
                "dbo.Hermes_Email_Template_Content",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(),
                        From_Name = c.String(maxLength: 255, storeType: "nvarchar"),
                        From_Email = c.String(maxLength: 1000, storeType: "nvarchar"),
                        Subject = c.String(maxLength: 1000, storeType: "nvarchar"),
                        Content = c.String(unicode: false),
                        Content_Plain = c.String(unicode: false),
                        Hermes_Email_Template_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hermes_Email_Template", t => t.Hermes_Email_Template_Id)
                .Index(t => t.LanguageId)
                .Index(t => t.Hermes_Email_Template_Id);
            
            CreateTable(
                "dbo.Hermes_Email_Placeholder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Prop_Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Description = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Num_Order = c.Int(nullable: false),
                        Hermes_Email_Template_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hermes_Email_Template", t => t.Hermes_Email_Template_Id)
                .Index(t => t.Prop_Name)
                .Index(t => t.Hermes_Email_Template_Id);
            
            CreateTable(
                "dbo.Hermes_Incoming_Email_Rule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BlockName = c.String(nullable: false, unicode: false),
                        WorkflowName = c.String(nullable: false, unicode: false),
                        Name = c.String(nullable: false, unicode: false),
                        Rule = c.String(unicode: false),
                        IncomingEmailId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .ForeignKey("dbo.Hermes_Incoming_Email", t => t.IncomingEmailId)
                .Index(t => t.IncomingEmailId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Hermes_Incoming_Email",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        ImapServer = c.String(nullable: false, unicode: false),
                        ImapPort = c.Int(),
                        ImapUseSSL = c.Boolean(nullable: false),
                        UserName = c.String(nullable: false, unicode: false),
                        Password = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_Js",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Value = c.String(nullable: false, unicode: false),
                        MozaicBootstrapPageId = c.Int(),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .ForeignKey("dbo.MozaicBootstrap_Page", t => t.MozaicBootstrapPageId)
                .Index(t => t.MozaicBootstrapPageId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.MozaicBootstrap_Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Content = c.String(unicode: false),
                        CompiledPartialView = c.String(unicode: false),
                        CompiledPageId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ParentApp_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ParentApp_Id)
                .Index(t => t.ParentApp_Id);
            
            CreateTable(
                "dbo.MozaicBootstrap_Components",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ElmId = c.String(unicode: false),
                        Tag = c.String(unicode: false),
                        UIC = c.String(unicode: false),
                        NumOrder = c.Int(nullable: false),
                        Attributes = c.String(unicode: false),
                        Properties = c.String(unicode: false),
                        Content = c.String(unicode: false),
                        ParentComponentId = c.Int(),
                        MozaicBootstrapPageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MozaicBootstrap_Components", t => t.ParentComponentId)
                .ForeignKey("dbo.MozaicBootstrap_Page", t => t.MozaicBootstrapPageId)
                .Index(t => t.ParentComponentId)
                .Index(t => t.MozaicBootstrapPageId);
            
            CreateTable(
                "dbo.MozaicEditor_Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        IsModal = c.Boolean(nullable: false),
                        ModalWidth = c.Int(),
                        ModalHeight = c.Int(),
                        CompiledPartialView = c.String(unicode: false),
                        CompiledPageId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ParentApp_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ParentApp_Id)
                .Index(t => t.ParentApp_Id);
            
            CreateTable(
                "dbo.MozaicEditor_Components",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Type = c.String(unicode: false),
                        PositionX = c.String(unicode: false),
                        PositionY = c.String(unicode: false),
                        Width = c.String(unicode: false),
                        Height = c.String(unicode: false),
                        Tag = c.String(unicode: false),
                        Attributes = c.String(unicode: false),
                        Classes = c.String(unicode: false),
                        BootstrapClasses = c.String(unicode: false),
                        Styles = c.String(unicode: false),
                        Content = c.String(unicode: false),
                        Label = c.String(unicode: false),
                        Placeholder = c.String(unicode: false),
                        TabIndex = c.String(unicode: false),
                        Properties = c.String(unicode: false),
                        ParentComponentId = c.Int(),
                        MozaicEditorPageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MozaicEditor_Components", t => t.ParentComponentId)
                .ForeignKey("dbo.MozaicEditor_Pages", t => t.MozaicEditorPageId)
                .Index(t => t.ParentComponentId)
                .Index(t => t.MozaicEditorPageId);
            
            CreateTable(
                "dbo.TapestryDesigner_ResourceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(unicode: false),
                        TypeClass = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ActionId = c.Int(),
                        StateId = c.Int(),
                        ComponentName = c.String(unicode: false),
                        IsBootstrap = c.Boolean(),
                        TableName = c.String(unicode: false),
                        IsShared = c.Boolean(),
                        ColumnName = c.String(unicode: false),
                        ColumnFilter = c.String(unicode: false),
                        PageId = c.Int(),
                        BootstrapPageId = c.Int(),
                        ParentRuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MozaicBootstrap_Page", t => t.BootstrapPageId)
                .ForeignKey("dbo.MozaicEditor_Pages", t => t.PageId)
                .ForeignKey("dbo.TapestryDesigner_ResourceRules", t => t.ParentRuleId)
                .Index(t => t.PageId)
                .Index(t => t.BootstrapPageId)
                .Index(t => t.ParentRuleId);
            
            CreateTable(
                "dbo.TapestryDesigner_ResourceRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentBlockCommit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.ParentBlockCommit_Id)
                .Index(t => t.ParentBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_ResourceConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                        ResourceRuleId = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        TargetSlot = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.SourceId)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.TargetId)
                .ForeignKey("dbo.TapestryDesigner_ResourceRules", t => t.ResourceRuleId)
                .Index(t => t.SourceId)
                .Index(t => t.TargetId)
                .Index(t => t.ResourceRuleId);
            
            CreateTable(
                "dbo.TapestryDesigner_BlocksCommits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        AssociatedTableName = c.String(unicode: false),
                        ModelTableName = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        CommitMessage = c.String(unicode: false),
                        Timestamp = c.DateTime(nullable: false, precision: 0),
                        AssociatedTableIds = c.String(unicode: false),
                        RoleWhitelist = c.String(unicode: false),
                        AssociatedPageIds = c.String(unicode: false),
                        AssociatedBootstrapPageIds = c.String(unicode: false),
                        ParentBlock_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Blocks", t => t.ParentBlock_Id)
                .Index(t => t.ParentBlock_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        AssociatedTableName = c.String(unicode: false),
                        AssociatedTableId = c.Int(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        MenuOrder = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        IsInMenu = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsChanged = c.Boolean(nullable: false),
                        BuiltBlockId = c.Int(),
                        LockedForUserId = c.Int(),
                        ParentMetablock_Id = c.Int(nullable: false),
                        ToolboxState_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Metablocks", t => t.ParentMetablock_Id)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.ToolboxState_Id)
                .Index(t => t.ParentMetablock_Id)
                .Index(t => t.ToolboxState_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Metablocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        MenuOrder = c.Int(nullable: false),
                        IsInitial = c.Boolean(nullable: false),
                        IsInMenu = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ParentMetablock_Id = c.Int(),
                        ParentAppId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Metablocks", t => t.ParentMetablock_Id)
                .ForeignKey("dbo.Master_Applications", t => t.ParentAppId)
                .Index(t => t.ParentMetablock_Id)
                .Index(t => t.ParentAppId);
            
            CreateTable(
                "dbo.TapestryDesigner_MetablocksConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceType = c.Int(nullable: false),
                        SourceId = c.Int(),
                        TargetType = c.Int(nullable: false),
                        TargetId = c.Int(),
                        TapestryDesignerMetablockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Metablocks", t => t.TapestryDesignerMetablockId)
                .Index(t => t.TapestryDesignerMetablockId);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeClass = c.String(unicode: false),
                        DialogType = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Label = c.String(unicode: false),
                        Name = c.String(unicode: false),
                        Comment = c.String(unicode: false),
                        CommentBottom = c.Boolean(nullable: false),
                        ActionId = c.Int(),
                        InputVariables = c.String(unicode: false),
                        OutputVariables = c.String(unicode: false),
                        StateId = c.Int(),
                        TargetName = c.String(unicode: false),
                        PageId = c.Int(),
                        ComponentName = c.String(unicode: false),
                        IsBootstrap = c.Boolean(),
                        isAjaxAction = c.Boolean(),
                        IsForeachStart = c.Boolean(),
                        IsForeachEnd = c.Boolean(),
                        Condition = c.String(unicode: false),
                        SymbolType = c.String(unicode: false),
                        HasParallelLock = c.Boolean(nullable: false),
                        ParentSubflowId = c.Int(),
                        ParentForeachId = c.Int(),
                        TargetId = c.Int(),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId)
                .ForeignKey("dbo.TapestryDesigner_Foreach", t => t.ParentForeachId)
                .ForeignKey("dbo.TapestryDesigner_Subflow", t => t.ParentSubflowId)
                .ForeignKey("dbo.TapestryDesigner_Blocks", t => t.TargetId)
                .Index(t => t.ParentSubflowId)
                .Index(t => t.ParentForeachId)
                .Index(t => t.TargetId)
                .Index(t => t.ParentSwimlaneId);
            
            CreateTable(
                "dbo.TapestryDesigner_Foreach",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, storeType: "nvarchar"),
                        Comment = c.String(unicode: false),
                        CommentBottom = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        DataSource = c.String(maxLength: 100, storeType: "nvarchar"),
                        ItemName = c.String(maxLength: 50, storeType: "nvarchar"),
                        IsParallel = c.Boolean(nullable: false),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId)
                .Index(t => t.ParentSwimlaneId);
            
            CreateTable(
                "dbo.TapestryDesigner_Swimlanes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SwimlaneIndex = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Roles = c.String(unicode: false),
                        ParentWorkflowRule_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_WorkflowRules", t => t.ParentWorkflowRule_Id)
                .Index(t => t.ParentWorkflowRule_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentBlockCommit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.ParentBlockCommit_Id)
                .Index(t => t.ParentBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                        WorkflowRuleId = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        TargetSlot = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_WorkflowItems", t => t.SourceId)
                .ForeignKey("dbo.TapestryDesigner_WorkflowItems", t => t.TargetId)
                .ForeignKey("dbo.TapestryDesigner_WorkflowRules", t => t.WorkflowRuleId)
                .Index(t => t.SourceId)
                .Index(t => t.TargetId)
                .Index(t => t.WorkflowRuleId);
            
            CreateTable(
                "dbo.TapestryDesigner_Subflow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        Comment = c.String(unicode: false),
                        CommentBottom = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId)
                .Index(t => t.ParentSwimlaneId);
            
            CreateTable(
                "dbo.TapestryDesigner_ToolboxStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociatedBlockCommit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.AssociatedBlockCommit_Id)
                .Index(t => t.AssociatedBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_ToolboxItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeClass = c.String(unicode: false),
                        Label = c.String(unicode: false),
                        ActionId = c.Int(),
                        TableName = c.String(unicode: false),
                        ColumnName = c.String(unicode: false),
                        PageId = c.Int(),
                        ComponentName = c.String(unicode: false),
                        IsBootstrap = c.Boolean(),
                        StateId = c.Int(),
                        TargetName = c.String(unicode: false),
                        TargetId = c.Int(),
                        TapestryDesignerToolboxState_Id = c.Int(),
                        TapestryDesignerToolboxState_Id1 = c.Int(),
                        TapestryDesignerToolboxState_Id2 = c.Int(),
                        TapestryDesignerToolboxState_Id3 = c.Int(),
                        TapestryDesignerToolboxState_Id4 = c.Int(),
                        TapestryDesignerToolboxState_Id5 = c.Int(),
                        TapestryDesignerToolboxState_Id6 = c.Int(),
                        TapestryDesignerToolboxState_Id7 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id1)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id2)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id3)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id4)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id5)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id6)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id7)
                .Index(t => t.TapestryDesignerToolboxState_Id)
                .Index(t => t.TapestryDesignerToolboxState_Id1)
                .Index(t => t.TapestryDesignerToolboxState_Id2)
                .Index(t => t.TapestryDesignerToolboxState_Id3)
                .Index(t => t.TapestryDesignerToolboxState_Id4)
                .Index(t => t.TapestryDesignerToolboxState_Id5)
                .Index(t => t.TapestryDesignerToolboxState_Id6)
                .Index(t => t.TapestryDesignerToolboxState_Id7);
            
            CreateTable(
                "dbo.RabbitMQs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, unicode: false),
                        HostName = c.String(nullable: false, unicode: false),
                        Port = c.Int(nullable: false),
                        QueueName = c.String(nullable: false, unicode: false),
                        Type = c.Int(nullable: false),
                        UserName = c.String(unicode: false),
                        Password = c.String(unicode: false),
                        BlockName = c.String(unicode: false),
                        WorkflowName = c.String(unicode: false),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Persona_AppRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50, storeType: "nvarchar"),
                        Priority = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => new { t.ApplicationId, t.Name }, unique: true, name: "PersonaAppRole_AppName");
            
            CreateTable(
                "dbo.Nexus_TCP_Socket_Listener",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Port = c.Int(nullable: false),
                        BufferSize = c.Int(nullable: false),
                        BlockName = c.String(nullable: false, unicode: false),
                        WorkflowName = c.String(nullable: false, unicode: false),
                        Name = c.String(nullable: false, unicode: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        InitBlockId = c.Int(),
                        ParentId = c.Int(),
                        ApplicationId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        IsInMenu = c.Boolean(nullable: false),
                        IsTemp = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.InitBlockId)
                .ForeignKey("dbo.Tapestry_WorkFlow", t => t.ParentId)
                .ForeignKey("dbo.Tapestry_WorkFlow_Types", t => t.TypeId)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => new { t.ApplicationId, t.Name, t.IsTemp }, unique: true, name: "Unique_workflowNameApp")
                .Index(t => t.InitBlockId)
                .Index(t => t.ParentId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.Tapestry_Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        ModelName = c.String(maxLength: 50, storeType: "nvarchar"),
                        DisplayName = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        IsVirtualForBlockId = c.Int(),
                        IsInMenu = c.Boolean(nullable: false),
                        WorkFlowId = c.Int(nullable: false),
                        MozaicPageId = c.Int(),
                        EditorPageId = c.Int(),
                        BootstrapPageId = c.Int(),
                        RoleWhitelist = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.IsVirtualForBlockId)
                .ForeignKey("dbo.Mozaic_Pages", t => t.MozaicPageId)
                .ForeignKey("dbo.Tapestry_WorkFlow", t => t.WorkFlowId)
                .Index(t => new { t.Name, t.WorkFlowId }, unique: true, name: "blockUniqueness")
                .Index(t => t.IsVirtualForBlockId)
                .Index(t => t.MozaicPageId);
            
            CreateTable(
                "dbo.Mozaic_Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ViewName = c.String(unicode: false),
                        ViewPath = c.String(unicode: false),
                        ViewContent = c.String(unicode: false),
                        IsBootstrap = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tapestry_ResourceMappingPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        relationType = c.String(maxLength: 100, storeType: "nvarchar"),
                        SourceComponentName = c.String(maxLength: 100, storeType: "nvarchar"),
                        SourceTableName = c.String(maxLength: 100, storeType: "nvarchar"),
                        SourceIsShared = c.Boolean(),
                        SourceColumnName = c.String(maxLength: 100, storeType: "nvarchar"),
                        SourceColumnFilter = c.String(unicode: false),
                        TargetName = c.String(maxLength: 100, storeType: "nvarchar"),
                        TargetTableName = c.String(maxLength: 100, storeType: "nvarchar"),
                        TargetIsShared = c.Boolean(),
                        TargetColumnName = c.String(maxLength: 100, storeType: "nvarchar"),
                        DataSourceParams = c.String(unicode: false),
                        BlockId = c.Int(nullable: false),
                        BlockName = c.String(nullable: false, unicode: false),
                        ApplicationName = c.String(nullable: false, unicode: false),
                        TargetType = c.String(unicode: false),
                        Source_Id = c.Int(),
                        Target_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.BlockId)
                .Index(t => t.BlockId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlow_Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.TapestryDesigner_ConditionSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SetIndex = c.Int(nullable: false),
                        SetRelation = c.String(unicode: false),
                        ConditionGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ConditionGroups", t => t.ConditionGroupId)
                .Index(t => t.ConditionGroupId);
            
            CreateTable(
                "dbo.TapestryDesigner_Conditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        Relation = c.String(unicode: false),
                        Variable = c.String(unicode: false),
                        Operator = c.String(unicode: false),
                        Value = c.String(unicode: false),
                        TapestryDesignerConditionSet_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ConditionSets", t => t.TapestryDesignerConditionSet_Id)
                .Index(t => t.TapestryDesignerConditionSet_Id);
            
            CreateTable(
                "dbo.Nexus_API",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, storeType: "nvarchar"),
                        Definition = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Persona_BadLoginCount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IP = c.String(maxLength: 60, storeType: "nvarchar"),
                        AttemptsCount = c.Int(nullable: false),
                        LastAtempt = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Nexus_CachedFiles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Blob = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Nexus_FileMetadataRecords", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Nexus_FileMetadataRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(unicode: false),
                        AppFolderName = c.String(unicode: false),
                        TimeCreated = c.DateTime(nullable: false, precision: 0),
                        TimeChanged = c.DateTime(nullable: false, precision: 0),
                        Version = c.Int(nullable: false),
                        AbsoluteURL = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        ModelEntityName = c.String(unicode: false),
                        ModelEntityId = c.Int(nullable: false),
                        Tag = c.String(unicode: false),
                        WebDavServer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Nexus_WebDavServers", t => t.WebDavServer_Id)
                .Index(t => t.WebDavServer_Id);
            
            CreateTable(
                "dbo.Nexus_WebDavServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                        UriBasePath = c.String(unicode: false),
                        AnonymousMode = c.Boolean(nullable: false),
                        AuthUsername = c.String(unicode: false),
                        AuthPassword = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CORE_ConfigPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Value = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true);
            
            CreateTable(
                "dbo.Hermes_Email_Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(unicode: false),
                        DateSend = c.DateTime(nullable: false, precision: 0),
                        Status = c.Int(nullable: false),
                        SMTP_Error = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hermes_Email_Queue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(unicode: false),
                        Date_Send_After = c.DateTime(nullable: false, precision: 0),
                        Date_Inserted = c.DateTime(nullable: false, precision: 0),
                        AttachmentList = c.String(unicode: false),
                        ApplicationId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.Date_Send_After)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Nexus_Ext_DB",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DB_Type = c.Int(nullable: false),
                        DB_Server = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        DB_Port = c.String(nullable: false, maxLength: 6, storeType: "nvarchar"),
                        DB_Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        DB_User = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        DB_Password = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        DB_Alias = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Athena_Graph",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Ident = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Js = c.String(nullable: false, unicode: false),
                        Css = c.String(unicode: false),
                        DemoData = c.String(unicode: false),
                        Html = c.String(unicode: false),
                        Library = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Ident, unique: true);
            
            CreateTable(
                "dbo.Nexus_Ldap",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Domain_Ntlm = c.String(maxLength: 50, storeType: "nvarchar"),
                        Domain_Kerberos = c.String(maxLength: 255, storeType: "nvarchar"),
                        Domain_Server = c.String(nullable: false, unicode: false),
                        Bind_User = c.String(nullable: false, unicode: false),
                        Bind_Password = c.String(nullable: false, unicode: false),
                        Active = c.Boolean(nullable: false),
                        Use_SSL = c.Boolean(nullable: false),
                        Is_Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Watchtower_LogItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false, precision: 0),
                        LogLevel = c.Int(nullable: false),
                        UserName = c.String(maxLength: 50, storeType: "nvarchar"),
                        Server = c.String(maxLength: 50, storeType: "nvarchar"),
                        Source = c.Int(nullable: false),
                        Application = c.String(maxLength: 50, storeType: "nvarchar"),
                        BlockName = c.String(maxLength: 100, storeType: "nvarchar"),
                        ActionName = c.String(maxLength: 50, storeType: "nvarchar"),
                        Message = c.String(unicode: false),
                        Vars = c.String(unicode: false),
                        StackTrace = c.String(unicode: false),
                        ParentLogItemId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Watchtower_LogItems", t => t.ParentLogItemId)
                .Index(t => t.Timestamp)
                .Index(t => t.UserName)
                .Index(t => t.Server)
                .Index(t => t.Source)
                .Index(t => t.Application)
                .Index(t => t.ParentLogItemId);
            
            CreateTable(
                "dbo.Persona_Identity_Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hermes_Smtp",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        Server = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Auth_User = c.String(maxLength: 255, storeType: "nvarchar"),
                        Auth_Password = c.String(maxLength: 255, storeType: "nvarchar"),
                        Use_SSL = c.Boolean(nullable: false),
                        Is_Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Use_SSL)
                .Index(t => t.Is_Default);
            
            CreateTable(
                "dbo.Cortex_Task",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                        Url = c.String(nullable: false, maxLength: 400, storeType: "nvarchar"),
                        Repeat = c.Boolean(nullable: false),
                        Repeat_Minute = c.Int(),
                        Repeat_Duration = c.Int(),
                        Daily_Repeat = c.Int(),
                        Weekly_Repeat = c.Int(),
                        Weekly_Days = c.Int(),
                        Monthly_Type = c.Int(),
                        Monthly_Months = c.Int(),
                        Monthly_Days = c.Long(),
                        Monthly_In_Modifiers = c.Int(),
                        Monthly_In_Days = c.Int(),
                        Idle_Time = c.Int(),
                        Start_Time = c.Time(nullable: false, precision: 0),
                        End_Time = c.Time(precision: 0),
                        Duration = c.Time(precision: 0),
                        Start_Date = c.DateTime(nullable: false, storeType: "date"),
                        End_Date = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.AppId)
                .Index(t => t.AppId);
            
            CreateTable(
                "dbo.Nexus_WS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(maxLength: 255, storeType: "nvarchar"),
                        WSDL_Url = c.String(maxLength: 255, storeType: "nvarchar"),
                        WSDL_File = c.Binary(),
                        REST_Base_Url = c.String(unicode: false),
                        Auth_User = c.String(unicode: false),
                        Auth_Password = c.String(unicode: false),
                        SOAP_Endpoint = c.String(unicode: false),
                        SOAP_XML_NS = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cortex_Task", "AppId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_Identity_UserRoles", "Iden_Role_Id", "dbo.Persona_Identity_Roles");
            DropForeignKey("dbo.Watchtower_LogItems", "ParentLogItemId", "dbo.Watchtower_LogItems");
            DropForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Nexus_FileMetadataRecords", "WebDavServer_Id", "dbo.Nexus_WebDavServers");
            DropForeignKey("dbo.Nexus_CachedFiles", "Id", "dbo.Nexus_FileMetadataRecords");
            DropForeignKey("dbo.Tapestry_ActionRule_Action", "ActionRuleId", "dbo.Tapestry_ActionRule");
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Tapestry_WorkFlow", "TypeId", "dbo.Tapestry_WorkFlow_Types");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ResourceMappingPairs", "BlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_Blocks", "MozaicPageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Tapestry_Blocks", "IsVirtualForBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_WorkFlow", "InitBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Nexus_TCP_Socket_Listener", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_AppRoles", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id", "dbo.Persona_AppRoles");
            DropForeignKey("dbo.RabbitMQs", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_Blocks", "ToolboxState_Id", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id7", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id6", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id5", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id4", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id3", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id2", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id1", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxStates", "AssociatedBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Subflow", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "TargetId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "SourceId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_Foreach", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_Metablocks", "ParentAppId", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_Metablocks", "ParentMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablockId", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "TargetId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "SourceId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "PageId", "dbo.MozaicEditor_Pages");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPageId", "dbo.MozaicEditor_Pages");
            DropForeignKey("dbo.MozaicEditor_Components", "ParentComponentId", "dbo.MozaicEditor_Components");
            DropForeignKey("dbo.MozaicBootstrap_Page", "ParentApp_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropForeignKey("dbo.MozaicBootstrap_Components", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropForeignKey("dbo.MozaicBootstrap_Components", "ParentComponentId", "dbo.MozaicBootstrap_Components");
            DropForeignKey("dbo.Mozaic_Js", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Hermes_Incoming_Email_Rule", "IncomingEmailId", "dbo.Hermes_Incoming_Email");
            DropForeignKey("dbo.Hermes_Incoming_Email_Rule", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications");
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Persona_Users", "DesignAppId", "dbo.Master_Applications");
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Entitron_DbView", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbTable", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbRelation", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbRelation", "TargetTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbRelation", "TargetColumnId", "dbo.Entitron_DbColumn");
            DropForeignKey("dbo.Entitron_DbRelation", "SourceTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbRelation", "SourceColumnId", "dbo.Entitron_DbColumn");
            DropForeignKey("dbo.Entitron_DbIndex", "DbTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbColumn", "DbTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_ColumnMetadata", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_ADgroups", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_ADgroup_User", "ADgroupId", "dbo.Persona_ADgroups");
            DropForeignKey("dbo.Master_UsersApplications", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Master_UsersApplications", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_User_Role", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_User_Role", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_Identity_UserRoles", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_ModuleAccessPermissions", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_UserLogin", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_UserClaim", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_ADgroup_User", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Tapestry_ActionRule", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropForeignKey("dbo.Tapestry_ActionRule", "ActorId", "dbo.Tapestry_Actors");
            DropForeignKey("dbo.Persona_ActionRuleRights", "ActionRuleId", "dbo.Tapestry_ActionRule");
            DropIndex("dbo.Cortex_Task", new[] { "AppId" });
            DropIndex("dbo.Hermes_Smtp", new[] { "Is_Default" });
            DropIndex("dbo.Hermes_Smtp", new[] { "Use_SSL" });
            DropIndex("dbo.Hermes_Smtp", new[] { "Name" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "ParentLogItemId" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Application" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Source" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Server" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "UserName" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Timestamp" });
            DropIndex("dbo.Athena_Graph", new[] { "Ident" });
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            DropIndex("dbo.Hermes_Email_Queue", new[] { "Date_Send_After" });
            DropIndex("dbo.CORE_ConfigPairs", new[] { "Key" });
            DropIndex("dbo.Nexus_FileMetadataRecords", new[] { "WebDavServer_Id" });
            DropIndex("dbo.Nexus_CachedFiles", new[] { "Id" });
            DropIndex("dbo.TapestryDesigner_Conditions", new[] { "TapestryDesignerConditionSet_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ConditionGroupId" });
            DropIndex("dbo.Tapestry_WorkFlow_Types", new[] { "Name" });
            DropIndex("dbo.Tapestry_ResourceMappingPairs", new[] { "BlockId" });
            DropIndex("dbo.Tapestry_Blocks", new[] { "MozaicPageId" });
            DropIndex("dbo.Tapestry_Blocks", new[] { "IsVirtualForBlockId" });
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "TypeId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "ParentId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "InitBlockId" });
            DropIndex("dbo.Tapestry_WorkFlow", "Unique_workflowNameApp");
            DropIndex("dbo.Nexus_TCP_Socket_Listener", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_AppRoles", "PersonaAppRole_AppName");
            DropIndex("dbo.RabbitMQs", new[] { "ApplicationId" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id7" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id6" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id5" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id4" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id3" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id2" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id1" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id" });
            DropIndex("dbo.TapestryDesigner_ToolboxStates", new[] { "AssociatedBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Subflow", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "WorkflowRuleId" });
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "TargetId" });
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "SourceId" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_Foreach", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "TargetId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentForeachId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSubflowId" });
            DropIndex("dbo.TapestryDesigner_MetablocksConnections", new[] { "TapestryDesignerMetablockId" });
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentAppId" });
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ToolboxState_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "ResourceRuleId" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "TargetId" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "SourceId" });
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "ParentRuleId" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "BootstrapPageId" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "PageId" });
            DropIndex("dbo.MozaicEditor_Components", new[] { "MozaicEditorPageId" });
            DropIndex("dbo.MozaicEditor_Components", new[] { "ParentComponentId" });
            DropIndex("dbo.MozaicEditor_Pages", new[] { "ParentApp_Id" });
            DropIndex("dbo.MozaicBootstrap_Components", new[] { "MozaicBootstrapPageId" });
            DropIndex("dbo.MozaicBootstrap_Components", new[] { "ParentComponentId" });
            DropIndex("dbo.MozaicBootstrap_Page", new[] { "ParentApp_Id" });
            DropIndex("dbo.Mozaic_Js", new[] { "ApplicationId" });
            DropIndex("dbo.Mozaic_Js", new[] { "MozaicBootstrapPageId" });
            DropIndex("dbo.Hermes_Incoming_Email_Rule", new[] { "ApplicationId" });
            DropIndex("dbo.Hermes_Incoming_Email_Rule", new[] { "IncomingEmailId" });
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Hermes_Email_Template_Id" });
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Prop_Name" });
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "Hermes_Email_Template_Id" });
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "LanguageId" });
            DropIndex("dbo.Hermes_Email_Template", "HermesUniqueness");
            DropIndex("dbo.Entitron_DbView", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbIndex", new[] { "DbTableId" });
            DropIndex("dbo.Entitron_DbTable", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbColumn", new[] { "DbTableId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "TargetColumnId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "TargetTableId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "SourceColumnId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "SourceTableId" });
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "ApplicationId" });
            DropIndex("dbo.Entitron_ColumnMetadata", "UX_Entitron_ColumnMetadata");
            DropIndex("dbo.Master_UsersApplications", "IX_userApp");
            DropIndex("dbo.Persona_User_Role", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_User_Role", new[] { "RoleName" });
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "Iden_Role_Id" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "UserId" });
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "UserId" });
            DropIndex("dbo.Persona_UserLogin", new[] { "UserId" });
            DropIndex("dbo.Persona_UserClaim", new[] { "UserId" });
            DropIndex("dbo.Persona_Users", new[] { "DesignAppId" });
            DropIndex("dbo.Persona_ADgroup_User", new[] { "UserId" });
            DropIndex("dbo.Persona_ADgroup_User", new[] { "ADgroupId" });
            DropIndex("dbo.Persona_ADgroups", new[] { "ApplicationId" });
            DropIndex("dbo.Master_Applications", new[] { "Name" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "ApplicationId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "TapestryDesignerWorkflowItemId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "TapestryDesignerResourceItemId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "ResourceMappingPairId" });
            DropIndex("dbo.Tapestry_Actors", new[] { "Name" });
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "PersonaAppRole_Id" });
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "ActionRuleId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "ActorId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "TargetBlockId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "SourceBlockId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "ConditionGroupId" });
            DropIndex("dbo.Tapestry_ActionRule_Action", new[] { "ActionRuleId" });
            DropTable("dbo.Nexus_WS");
            DropTable("dbo.Cortex_Task");
            DropTable("dbo.Hermes_Smtp");
            DropTable("dbo.Persona_Identity_Roles");
            DropTable("dbo.Watchtower_LogItems");
            DropTable("dbo.Nexus_Ldap");
            DropTable("dbo.Athena_Graph");
            DropTable("dbo.Nexus_Ext_DB");
            DropTable("dbo.Hermes_Email_Queue");
            DropTable("dbo.Hermes_Email_Log");
            DropTable("dbo.CORE_ConfigPairs");
            DropTable("dbo.Nexus_WebDavServers");
            DropTable("dbo.Nexus_FileMetadataRecords");
            DropTable("dbo.Nexus_CachedFiles");
            DropTable("dbo.Persona_BadLoginCount");
            DropTable("dbo.Nexus_API");
            DropTable("dbo.TapestryDesigner_Conditions");
            DropTable("dbo.TapestryDesigner_ConditionSets");
            DropTable("dbo.Tapestry_WorkFlow_Types");
            DropTable("dbo.Tapestry_ResourceMappingPairs");
            DropTable("dbo.Mozaic_Pages");
            DropTable("dbo.Tapestry_Blocks");
            DropTable("dbo.Tapestry_WorkFlow");
            DropTable("dbo.Nexus_TCP_Socket_Listener");
            DropTable("dbo.Persona_AppRoles");
            DropTable("dbo.RabbitMQs");
            DropTable("dbo.TapestryDesigner_ToolboxItems");
            DropTable("dbo.TapestryDesigner_ToolboxStates");
            DropTable("dbo.TapestryDesigner_Subflow");
            DropTable("dbo.TapestryDesigner_WorkflowConnections");
            DropTable("dbo.TapestryDesigner_WorkflowRules");
            DropTable("dbo.TapestryDesigner_Swimlanes");
            DropTable("dbo.TapestryDesigner_Foreach");
            DropTable("dbo.TapestryDesigner_WorkflowItems");
            DropTable("dbo.TapestryDesigner_MetablocksConnections");
            DropTable("dbo.TapestryDesigner_Metablocks");
            DropTable("dbo.TapestryDesigner_Blocks");
            DropTable("dbo.TapestryDesigner_BlocksCommits");
            DropTable("dbo.TapestryDesigner_ResourceConnections");
            DropTable("dbo.TapestryDesigner_ResourceRules");
            DropTable("dbo.TapestryDesigner_ResourceItems");
            DropTable("dbo.MozaicEditor_Components");
            DropTable("dbo.MozaicEditor_Pages");
            DropTable("dbo.MozaicBootstrap_Components");
            DropTable("dbo.MozaicBootstrap_Page");
            DropTable("dbo.Mozaic_Js");
            DropTable("dbo.Hermes_Incoming_Email");
            DropTable("dbo.Hermes_Incoming_Email_Rule");
            DropTable("dbo.Hermes_Email_Placeholder");
            DropTable("dbo.Hermes_Email_Template_Content");
            DropTable("dbo.Hermes_Email_Template");
            DropTable("dbo.Entitron_DbView");
            DropTable("dbo.Entitron_DbIndex");
            DropTable("dbo.Entitron_DbTable");
            DropTable("dbo.Entitron_DbColumn");
            DropTable("dbo.Entitron_DbRelation");
            DropTable("dbo.Entitron_DbSchemeCommit");
            DropTable("dbo.Entitron_ColumnMetadata");
            DropTable("dbo.Master_UsersApplications");
            DropTable("dbo.Persona_User_Role");
            DropTable("dbo.Persona_Identity_UserRoles");
            DropTable("dbo.Persona_ModuleAccessPermissions");
            DropTable("dbo.Persona_UserLogin");
            DropTable("dbo.Persona_UserClaim");
            DropTable("dbo.Persona_Users");
            DropTable("dbo.Persona_ADgroup_User");
            DropTable("dbo.Persona_ADgroups");
            DropTable("dbo.Master_Applications");
            DropTable("dbo.TapestryDesigner_ConditionGroups");
            DropTable("dbo.Tapestry_Actors");
            DropTable("dbo.Persona_ActionRuleRights");
            DropTable("dbo.Tapestry_ActionRule");
            DropTable("dbo.Tapestry_ActionRule_Action");
        }
    }
}
