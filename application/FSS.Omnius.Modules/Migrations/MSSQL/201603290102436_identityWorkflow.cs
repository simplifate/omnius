namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identityWorkflow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropPrimaryKey("dbo.Tapestry_WorkFlow");
            DropColumn("dbo.Tapestry_WorkFlow", "Id");
            AddColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(identity: true));
            AddPrimaryKey("dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            DropForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow");
            DropPrimaryKey("dbo.Tapestry_WorkFlow");
            DropColumn("dbo.Tapestry_WorkFlow", "Id");
            DropColumn("dbo.Tapestry_WorkFlow", "Id");
            AddColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id");
            AddForeignKey("dbo.Tapestry_WorkFlow", "ParentId", "dbo.Tapestry_WorkFlow", "Id");
        }
    }
}
