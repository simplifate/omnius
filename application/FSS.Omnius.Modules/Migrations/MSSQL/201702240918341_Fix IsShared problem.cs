namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixIsSharedproblem : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "IsShared", c => c.Boolean());
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared", c => c.Boolean());
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared", c => c.Boolean(nullable: false));
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "IsShared", c => c.Boolean(nullable: false));
        }
    }
}
