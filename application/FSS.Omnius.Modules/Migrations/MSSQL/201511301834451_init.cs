namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Persona_ActionRights",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Readable = c.Boolean(nullable: false),
                        Executable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.ActionId })
                .ForeignKey("dbo.Persona_Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Persona_Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Persona_AppRights",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        Readable = c.Boolean(nullable: false),
                        Executable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.ApplicationId })
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Persona_Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Master_Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Icon = c.String(),
                        TitleFontSize = c.Int(nullable: false),
                        Color = c.Int(nullable: false),
                        InnerHTML = c.String(),
                        LaunchCommand = c.String(),
                        TileWidth = c.Int(nullable: false),
                        TileHeight = c.Int(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Relations = c.String(nullable: false),
                        MasterTemplateId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_Template", t => t.MasterTemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.MasterTemplateId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Tapestry_Blocks",
                c => new
                    {
                        MozaicPageId = c.Int(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ModelName = c.String(maxLength: 50),
                        IsVirtual = c.Boolean(nullable: false),
                        WorkFlowId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MozaicPageId)
                .ForeignKey("dbo.Tapestry_WorkFlow", t => t.WorkFlowId, cascadeDelete: true)
                .ForeignKey("dbo.Mozaic_Pages", t => t.MozaicPageId)
                .Index(t => t.MozaicPageId)
                .Index(t => t.WorkFlowId);
            
            CreateTable(
                "dbo.Tapestry_AttributeRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InputName = c.String(nullable: false, maxLength: 50),
                        AttributeName = c.String(nullable: false, maxLength: 50),
                        BlockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.BlockId, cascadeDelete: true)
                .Index(t => t.BlockId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InitBlockId = c.Int(nullable: false),
                        ParentId = c.Int(),
                        ApplicationId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_WorkFlow", t => t.ParentId)
                .ForeignKey("dbo.Tapestry_WorkFlow_Types", t => t.TypeId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.ParentId)
                .Index(t => t.ApplicationId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlow_Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tapestry_ActionRule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PreFunctionCount = c.Int(nullable: false),
                        Condition = c.String(),
                        SourceBlockId = c.Int(nullable: false),
                        TargetBlockId = c.Int(nullable: false),
                        ActorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Actors", t => t.ActorId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.SourceBlockId, cascadeDelete: true)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.TargetBlockId)
                .Index(t => t.SourceBlockId)
                .Index(t => t.TargetBlockId)
                .Index(t => t.ActorId);
            
            CreateTable(
                "dbo.Tapestry_ActionRule_Action",
                c => new
                    {
                        ActionRuleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        InputVariablesMapping = c.String(maxLength: 200),
                        OutputVariablesMapping = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => new { t.ActionRuleId, t.ActionId })
                .ForeignKey("dbo.Tapestry_ActionRule", t => t.ActionRuleId, cascadeDelete: true)
                .Index(t => t.ActionRuleId);
            
            CreateTable(
                "dbo.Tapestry_Actors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_Css",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Html = c.String(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_TemplateCategories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Mozaic_TemplateCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_TemplateCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Entitron___META",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ApplicationId = c.Int(nullable: false),
                        tableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => new { t.ApplicationId, t.Name }, unique: true, name: "UNIQUE_Entitron___META_Name");
            
            CreateTable(
                "dbo.Persona_Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        username = c.String(nullable: false, maxLength: 50),
                        passwordHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Entitron_DbColumn",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PrimaryKey = c.Boolean(nullable: false),
                        Unique = c.Boolean(nullable: false),
                        AllowNull = c.Boolean(nullable: false),
                        Type = c.String(),
                        ColumnLength = c.Int(nullable: false),
                        ColumnLengthIsMax = c.Boolean(nullable: false),
                        DefaultValue = c.String(),
                        DbTableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbTable", t => t.DbTableId, cascadeDelete: true)
                .Index(t => t.DbTableId);
            
            CreateTable(
                "dbo.Entitron_DbTable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId, cascadeDelete: true)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Entitron_DbSchemeCommit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommitMessage = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Entitron_DbRelation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        LeftTable = c.Int(nullable: false),
                        LeftColumn = c.Int(nullable: false),
                        RightTable = c.Int(nullable: false),
                        RightColumn = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId, cascadeDelete: true)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Entitron_DbView",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Query = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        DbSchemeCommitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbSchemeCommit", t => t.DbSchemeCommitId, cascadeDelete: true)
                .Index(t => t.DbSchemeCommitId);
            
            CreateTable(
                "dbo.Entitron_DbIndex",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Unique = c.Boolean(nullable: false),
                        ColumnNames = c.String(),
                        DbTableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Entitron_DbTable", t => t.DbTableId, cascadeDelete: true)
                .Index(t => t.DbTableId);
            
            CreateTable(
                "dbo.Nexus_Ldap",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Domain_Ntlm = c.String(maxLength: 50),
                        Domain_Kerberos = c.String(maxLength: 255),
                        Domain_Server = c.String(nullable: false),
                        Bind_User = c.String(nullable: false),
                        Bind_Password = c.String(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Use_SSL = c.Boolean(nullable: false),
                        Is_Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CORE_Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Apps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_MetaBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentMetaBlock_Id = c.Int(),
                        ParentApp_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_MetaBlocks", t => t.ParentMetaBlock_Id)
                .ForeignKey("dbo.TapestryDesigner_Apps", t => t.ParentApp_Id)
                .Index(t => t.ParentMetaBlock_Id)
                .Index(t => t.ParentApp_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AssociatedTableName = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentMetaBlock_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_MetaBlocks", t => t.ParentMetaBlock_Id, cascadeDelete: true)
                .Index(t => t.ParentMetaBlock_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_BlocksCommits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AssociatedTableName = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        CommitMessage = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        ParentBlock_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Blocks", t => t.ParentBlock_Id, cascadeDelete: true)
                .Index(t => t.ParentBlock_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Rules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentBlockCommit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.ParentBlockCommit_Id, cascadeDelete: true)
                .Index(t => t.ParentBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Connections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Source = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        Target = c.Int(nullable: false),
                        TapestryDesignerRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Rules", t => t.TapestryDesignerRule_Id)
                .Index(t => t.TapestryDesignerRule_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemReferenceId = c.Int(nullable: false),
                        Label = c.String(),
                        TypeClass = c.String(),
                        DialogType = c.String(),
                        IsDataSource = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentRule_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Rules", t => t.ParentRule_Id, cascadeDelete: true)
                .Index(t => t.ParentRule_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Operators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        DialogType = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Rules", t => t.ParentRule_Id)
                .Index(t => t.ParentRule_Id);
            
            CreateTable(
                "dbo.Mozaic_CssPages",
                c => new
                    {
                        CssId = c.Int(nullable: false),
                        PageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CssId, t.PageId })
                .ForeignKey("dbo.Mozaic_Css", t => t.CssId, cascadeDelete: true)
                .ForeignKey("dbo.Mozaic_Pages", t => t.PageId, cascadeDelete: true)
                .Index(t => t.CssId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.Persona_Groups_Users",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId })
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Persona_Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.TapestryDesigner_Apps");
            DropForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks");
            DropForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks");
            DropForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks");
            DropForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_Operators", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.Entitron_DbIndex", "DbTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbView", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbTable", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbRelation", "DbSchemeCommitId", "dbo.Entitron_DbSchemeCommit");
            DropForeignKey("dbo.Entitron_DbColumn", "DbTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Persona_ActionRights", "GroupId", "dbo.Persona_Groups");
            DropForeignKey("dbo.Persona_Groups_Users", "GroupId", "dbo.Persona_Groups");
            DropForeignKey("dbo.Persona_Groups_Users", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_AppRights", "GroupId", "dbo.Persona_Groups");
            DropForeignKey("dbo.Persona_AppRights", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Pages", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Pages", "MasterTemplateId", "dbo.Mozaic_Template");
            DropForeignKey("dbo.Mozaic_Template", "CategoryId", "dbo.Mozaic_TemplateCategories");
            DropForeignKey("dbo.Mozaic_TemplateCategories", "ParentId", "dbo.Mozaic_TemplateCategories");
            DropForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Mozaic_CssPages", "CssId", "dbo.Mozaic_Css");
            DropForeignKey("dbo.Tapestry_Blocks", "MozaicPageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "ActorId", "dbo.Tapestry_Actors");
            DropForeignKey("dbo.Tapestry_ActionRule_Action", "ActionRuleId", "dbo.Tapestry_ActionRule");
            DropForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_WorkFlow", "TypeId", "dbo.Tapestry_WorkFlow_Types");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_AttributeRoles", "BlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Persona_Groups_Users", new[] { "GroupId" });
            DropIndex("dbo.Persona_Groups_Users", new[] { "UserId" });
            DropIndex("dbo.Mozaic_CssPages", new[] { "PageId" });
            DropIndex("dbo.Mozaic_CssPages", new[] { "CssId" });
            DropIndex("dbo.TapestryDesigner_Operators", new[] { "ParentRule_Id" });
            DropIndex("dbo.TapestryDesigner_Items", new[] { "ParentRule_Id" });
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "TapestryDesignerRule_Id" });
            DropIndex("dbo.TapestryDesigner_Rules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetaBlock_Id" });
            DropIndex("dbo.TapestryDesigner_MetaBlocks", new[] { "ParentApp_Id" });
            DropIndex("dbo.TapestryDesigner_MetaBlocks", new[] { "ParentMetaBlock_Id" });
            DropIndex("dbo.Entitron_DbIndex", new[] { "DbTableId" });
            DropIndex("dbo.Entitron_DbView", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbTable", new[] { "DbSchemeCommitId" });
            DropIndex("dbo.Entitron_DbColumn", new[] { "DbTableId" });
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            DropIndex("dbo.Mozaic_TemplateCategories", new[] { "ParentId" });
            DropIndex("dbo.Mozaic_Template", new[] { "CategoryId" });
            DropIndex("dbo.Tapestry_ActionRule_Action", new[] { "ActionRuleId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "ActorId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "TargetBlockId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "SourceBlockId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "TypeId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "ApplicationId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "ParentId" });
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "Id" });
            DropIndex("dbo.Tapestry_AttributeRoles", new[] { "BlockId" });
            DropIndex("dbo.Tapestry_Blocks", new[] { "WorkFlowId" });
            DropIndex("dbo.Tapestry_Blocks", new[] { "MozaicPageId" });
            DropIndex("dbo.Mozaic_Pages", new[] { "ApplicationId" });
            DropIndex("dbo.Mozaic_Pages", new[] { "MasterTemplateId" });
            DropIndex("dbo.Persona_AppRights", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_AppRights", new[] { "GroupId" });
            DropIndex("dbo.Persona_ActionRights", new[] { "GroupId" });
            DropTable("dbo.Persona_Groups_Users");
            DropTable("dbo.Mozaic_CssPages");
            DropTable("dbo.TapestryDesigner_Operators");
            DropTable("dbo.TapestryDesigner_Items");
            DropTable("dbo.TapestryDesigner_Connections");
            DropTable("dbo.TapestryDesigner_Rules");
            DropTable("dbo.TapestryDesigner_BlocksCommits");
            DropTable("dbo.TapestryDesigner_Blocks");
            DropTable("dbo.TapestryDesigner_MetaBlocks");
            DropTable("dbo.TapestryDesigner_Apps");
            DropTable("dbo.CORE_Modules");
            DropTable("dbo.Nexus_Ldap");
            DropTable("dbo.Entitron_DbIndex");
            DropTable("dbo.Entitron_DbView");
            DropTable("dbo.Entitron_DbRelation");
            DropTable("dbo.Entitron_DbSchemeCommit");
            DropTable("dbo.Entitron_DbTable");
            DropTable("dbo.Entitron_DbColumn");
            DropTable("dbo.Persona_Users");
            DropTable("dbo.Entitron___META");
            DropTable("dbo.Mozaic_TemplateCategories");
            DropTable("dbo.Mozaic_Template");
            DropTable("dbo.Mozaic_Css");
            DropTable("dbo.Tapestry_Actors");
            DropTable("dbo.Tapestry_ActionRule_Action");
            DropTable("dbo.Tapestry_ActionRule");
            DropTable("dbo.Tapestry_WorkFlow_Types");
            DropTable("dbo.Tapestry_WorkFlow");
            DropTable("dbo.Tapestry_AttributeRoles");
            DropTable("dbo.Tapestry_Blocks");
            DropTable("dbo.Mozaic_Pages");
            DropTable("dbo.Master_Applications");
            DropTable("dbo.Persona_AppRights");
            DropTable("dbo.Persona_Groups");
            DropTable("dbo.Persona_ActionRights");
        }
    }
}
