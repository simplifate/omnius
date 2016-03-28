namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class splitConnectionsRepair : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "TapestryDesignerResourceRule_Id", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "ResourceRuleId" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "TapestryDesignerResourceRule_Id" });
            DropColumn("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId");
            RenameColumn(table: "dbo.TapestryDesigner_ResourceConnections", name: "TapestryDesignerResourceRule_Id", newName: "ResourceRuleId");
            AlterColumn("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId");
            AddForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "ResourceRuleId" });
            AlterColumn("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", c => c.Int());
            RenameColumn(table: "dbo.TapestryDesigner_ResourceConnections", name: "ResourceRuleId", newName: "TapestryDesignerResourceRule_Id");
            AddColumn("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_ResourceConnections", "TapestryDesignerResourceRule_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId");
            AddForeignKey("dbo.TapestryDesigner_ResourceConnections", "TapestryDesignerResourceRule_Id", "dbo.TapestryDesigner_ResourceRules", "Id");
        }
    }
}
