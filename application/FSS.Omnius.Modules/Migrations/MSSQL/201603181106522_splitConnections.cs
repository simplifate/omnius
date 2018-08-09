namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class splitConnections : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_WorkflowConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                        TargetSlot = c.Int(nullable: false),
                        WorkflowRuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_WorkflowRules", t => t.WorkflowRuleId, cascadeDelete: true)
                .Index(t => t.WorkflowRuleId);
            
            CreateTable(
                "dbo.TapestryDesigner_ResourceConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                        TargetSlot = c.Int(nullable: false),
                        ResourceRuleId = c.Int(nullable: false),
                        TapestryDesignerResourceRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_WorkflowRules", t => t.ResourceRuleId, cascadeDelete: true)
                .ForeignKey("dbo.TapestryDesigner_ResourceRules", t => t.TapestryDesignerResourceRule_Id)
                .Index(t => t.ResourceRuleId)
                .Index(t => t.TapestryDesignerResourceRule_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "TapestryDesignerResourceRule_Id", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "TapestryDesignerResourceRule_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "ResourceRuleId" });
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "WorkflowRuleId" });
            DropTable("dbo.TapestryDesigner_ResourceConnections");
            DropTable("dbo.TapestryDesigner_WorkflowConnections");
        }
    }
}
