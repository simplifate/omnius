namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dev_Grid_Merge : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction", c => c.String());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "CommentBottom", c => c.Boolean(nullable: false));
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd", c => c.Boolean());
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach", "Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach");
            DropForeignKey("dbo.TapestryDesigner_Subflow", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Foreach", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.TapestryDesigner_Subflow", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_Foreach", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentForeachId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSubflowId" });
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "CommentBottom");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd");
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction");
            DropTable("dbo.TapestryDesigner_Subflow");
            DropTable("dbo.TapestryDesigner_Foreach");
        }
    }
}
