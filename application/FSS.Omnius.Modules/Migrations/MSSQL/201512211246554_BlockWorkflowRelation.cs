namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlockWorkflowRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropIndex("dbo.Tapestry_WorkFlow", new[] { "Id" });
            AlterColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(nullable: false, identity: true));
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow", "Id");
            DropColumn("dbo.Tapestry_WorkFlow", "InitBlockId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tapestry_WorkFlow", "InitBlockId", c => c.Int());
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            AlterColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks", "Id");
        }
    }
}
