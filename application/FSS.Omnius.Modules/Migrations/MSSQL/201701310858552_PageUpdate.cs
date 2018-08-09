namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "BootstrapPageId", c => c.Int());
            AddColumn("dbo.Mozaic_Pages", "IsBootstrap", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Mozaic_Pages", "IsBootstrap");
            DropColumn("dbo.Tapestry_Blocks", "BootstrapPageId");
        }
    }
}
