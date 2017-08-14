namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dev_Grid_Work_Merge : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            CreateTable(
                "dbo.TapestryDesigner_Foreach",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Comment = c.String(),
                        CommentBottom = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        DataSource = c.String(),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId, cascadeDelete: true)
                .Index(t => t.ParentSwimlaneId);
            
            CreateTable(
                "dbo.TapestryDesigner_Subflow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Comment = c.String(),
                        CommentBottom = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId, cascadeDelete: true)
                .Index(t => t.ParentSwimlaneId);
            
            CreateTable(
                "dbo.Nexus_TCP_Socket_Listener",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Port = c.Int(nullable: false),
                        BufferSize = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        BlockName = c.String(nullable: false),
                        WorkflowName = c.String(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction", c => c.String());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd", c => c.Boolean());
            AddColumn("dbo.Master_Applications", "SchemeLockedForUserId", c => c.Int());
            AddColumn("dbo.Persona_ADgroups", "RoleForApplication", c => c.String());
            AddColumn("dbo.Persona_User_Role", "ApplicationName", c => c.String());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "CommentBottom", c => c.Boolean(nullable: false));
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd", c => c.Boolean());
            AlterColumn("dbo.Entitron_DbSchemeCommit", "Application_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach", "Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow", "Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id", cascadeDelete: true);
            DropColumn("dbo.Nexus_Ext_DB", "DB_Alias");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Nexus_Ext_DB", "DB_Alias", c => c.String(nullable: false, maxLength: 255));
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.Nexus_TCP_Socket_Listener", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach");
            DropForeignKey("dbo.TapestryDesigner_Subflow", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Foreach", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.Nexus_TCP_Socket_Listener", new[] { "ApplicationId" });
            DropIndex("dbo.TapestryDesigner_Subflow", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_Foreach", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentForeachId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSubflowId" });
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "Application_Id", c => c.Int());
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "CommentBottom");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            DropColumn("dbo.Persona_User_Role", "ApplicationName");
            DropColumn("dbo.Persona_ADgroups", "RoleForApplication");
            DropColumn("dbo.Master_Applications", "SchemeLockedForUserId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd");
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction");
            DropTable("dbo.Nexus_TCP_Socket_Listener");
            DropTable("dbo.TapestryDesigner_Subflow");
            DropTable("dbo.TapestryDesigner_Foreach");
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id");
        }
    }
}
