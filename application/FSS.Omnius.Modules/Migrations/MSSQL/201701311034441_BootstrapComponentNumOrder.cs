namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BootstrapComponentNumOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicBootstrap_Components", "NumOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicBootstrap_Components", "NumOrder");
        }
    }
}
