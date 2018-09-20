namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class parallelForeach : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "HasParallelLock", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.TapestryDesigner_Foreach", "IsParallel", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Foreach", "IsParallel");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "HasParallelLock");
        }
    }
}
