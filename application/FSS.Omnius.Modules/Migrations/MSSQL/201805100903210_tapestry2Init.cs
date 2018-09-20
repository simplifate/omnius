namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tapestry2Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "BlockName", c => c.String(nullable: true));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "ApplicationName", c => c.String(nullable: true));
            Sql("UPDATE rmp SET BlockName = b.Name, ApplicationName = app.Name FROM Tapestry_ResourceMappingPairs rmp JOIN Tapestry_Blocks b ON b.Id = rmp.BlockId JOIN Tapestry_WorkFlow w ON w.Id = b.WorkFlowId JOIN Master_Applications app ON app.Id = w.ApplicationId");
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "BlockName", c => c.String(nullable: false));
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "ApplicationName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "ApplicationName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "BlockName");
        }
    }
}
