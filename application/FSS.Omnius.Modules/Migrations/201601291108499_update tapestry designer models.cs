namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetapestrydesignermodels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropIndex("dbo.TapestryDesigner_Rules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Items", new[] { "ParentRule_Id" });
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
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.ParentBlockCommit_Id, cascadeDelete: true)
                .Index(t => t.ParentBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_ResourceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        TypeClass = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceRules", t => t.ParentRule_Id)
                .Index(t => t.ParentRule_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowRules",
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
                "dbo.TapestryDesigner_Swimlanes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SwimlaneIndex = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Roles = c.String(),
                        ParentWorkflowRule_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_WorkflowRules", t => t.ParentWorkflowRule_Id, cascadeDelete: true)
                .Index(t => t.ParentWorkflowRule_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        TypeClass = c.String(),
                        DialogType = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentSwimlane_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlane_Id)
                .Index(t => t.ParentSwimlane_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_WorkflowSymbols",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        DialogType = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentSwimlane_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlane_Id)
                .Index(t => t.ParentSwimlane_Id);
            
            AddColumn("dbo.TapestryDesigner_Connections", "SourceType", c => c.Int(nullable: false));
            AddColumn("dbo.TapestryDesigner_Connections", "TargetType", c => c.Int(nullable: false));
            AddColumn("dbo.TapestryDesigner_Connections", "TargetSlot", c => c.Int(nullable: false));
            AddColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerResourceRule_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerWorkflowRule_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Items", "ParentRule_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_Connections", "TapestryDesignerResourceRule_Id");
            CreateIndex("dbo.TapestryDesigner_Connections", "TapestryDesignerWorkflowRule_Id");
            CreateIndex("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_Items", "ParentRule_Id");
            AddForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerResourceRule_Id", "dbo.TapestryDesigner_ResourceRules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowSymbols", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRule_Id", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerResourceRule_Id", "dbo.TapestryDesigner_ResourceRules");
            DropIndex("dbo.TapestryDesigner_Items", new[] { "ParentRule_Id" });
            DropIndex("dbo.TapestryDesigner_Rules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowSymbols", new[] { "ParentSwimlane_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSwimlane_Id" });
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "ParentRule_Id" });
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "TapestryDesignerWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "TapestryDesignerResourceRule_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            AlterColumn("dbo.TapestryDesigner_Items", "ParentRule_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", c => c.Int(nullable: false));
            DropColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerWorkflowRule_Id");
            DropColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerResourceRule_Id");
            DropColumn("dbo.TapestryDesigner_Connections", "TargetSlot");
            DropColumn("dbo.TapestryDesigner_Connections", "TargetType");
            DropColumn("dbo.TapestryDesigner_Connections", "SourceType");
            DropTable("dbo.TapestryDesigner_WorkflowSymbols");
            DropTable("dbo.TapestryDesigner_WorkflowItems");
            DropTable("dbo.TapestryDesigner_Swimlanes");
            DropTable("dbo.TapestryDesigner_WorkflowRules");
            DropTable("dbo.TapestryDesigner_ResourceItems");
            DropTable("dbo.TapestryDesigner_ResourceRules");
            CreateIndex("dbo.TapestryDesigner_Items", "ParentRule_Id");
            CreateIndex("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id");
            AddForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules", "Id", cascadeDelete: true);
        }
    }
}
