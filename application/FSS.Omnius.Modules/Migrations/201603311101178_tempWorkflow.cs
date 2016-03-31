namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempWorkflow : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tapestry_WorkFlow", "Unique_workflowNameApp");
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            AddColumn("dbo.Tapestry_WorkFlow", "IsTemp", c => c.Boolean(nullable: false, defaultValue: false));
            CreateIndex("dbo.Tapestry_WorkFlow", new[] { "ApplicationId", "Name", "IsTemp" }, unique: true, name: "Unique_workflowNameApp");
            CreateIndex("dbo.Tapestry_Blocks", new[] { "Name", "WorkFlowId" }, unique: true, name: "blockUniqueness");
            DropColumn("dbo.Tapestry_Blocks", "IsTemp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tapestry_Blocks", "IsTemp", c => c.Boolean(nullable: false));
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            DropIndex("dbo.Tapestry_WorkFlow", "Unique_workflowNameApp");
            DropColumn("dbo.Tapestry_WorkFlow", "IsTemp");
            CreateIndex("dbo.Tapestry_Blocks", new[] { "Name", "IsTemp", "WorkFlowId" }, unique: true, name: "blockUniqueness");
            CreateIndex("dbo.Tapestry_WorkFlow", new[] { "ApplicationId", "Name" }, unique: true, name: "Unique_workflowNameApp");
        }
    }
}
