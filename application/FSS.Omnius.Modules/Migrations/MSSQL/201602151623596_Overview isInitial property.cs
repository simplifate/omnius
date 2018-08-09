namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverviewisInitialproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Metablocks", "IsInitial", c => c.Boolean(nullable: false));
            AddColumn("dbo.TapestryDesigner_Blocks", "IsInitial", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "IsInitial");
            DropColumn("dbo.TapestryDesigner_Metablocks", "IsInitial");
        }
    }
}
