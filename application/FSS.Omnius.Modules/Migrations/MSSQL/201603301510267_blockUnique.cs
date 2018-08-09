namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blockUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "ApplicationId" });
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            DropIndex("dbo.Tapestry_Blocks", new[] { "WorkFlowId" });
            AddColumn("dbo.Tapestry_WorkFlow", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Tapestry_WorkFlow", new[] { "ApplicationId", "Name" }, unique: true, name: "Unique_workflowNameApp");
            CreateIndex("dbo.Tapestry_Blocks", new[] { "Name", "IsTemp", "WorkFlowId" }, unique: true, name: "blockUniqueness");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            DropIndex("dbo.Tapestry_WorkFlow", "Unique_workflowNameApp");
            DropColumn("dbo.Tapestry_WorkFlow", "Name");
            CreateIndex("dbo.Tapestry_Blocks", "WorkFlowId");
            CreateIndex("dbo.Tapestry_Blocks", new[] { "Name", "IsTemp" }, unique: true, name: "blockUniqueness");
            CreateIndex("dbo.Tapestry_WorkFlow", "ApplicationId");
        }
    }
}
