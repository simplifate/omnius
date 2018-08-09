namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wstype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_WS", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Nexus_WS", "REST_Base_Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_WS", "REST_Base_Url");
            DropColumn("dbo.Nexus_WS", "Type");
        }
    }
}
