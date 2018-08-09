namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlockWorkFlowStep2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_WorkFlow", "InitBlockId", c => c.Int());
            CreateIndex("dbo.Tapestry_WorkFlow", "InitBlockId");
            AddForeignKey("dbo.Tapestry_WorkFlow", "InitBlockId", "dbo.Tapestry_Blocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_WorkFlow", "InitBlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "InitBlockId" });
            DropColumn("dbo.Tapestry_WorkFlow", "InitBlockId");
        }
    }
}
