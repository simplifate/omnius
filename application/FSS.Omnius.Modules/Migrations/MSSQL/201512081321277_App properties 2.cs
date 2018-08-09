namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appproperties2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "IsPublished", c => c.Boolean(nullable: false));
            AddColumn("dbo.Master_Applications", "IsEnabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Master_Applications", "ShowInAppManager");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Master_Applications", "ShowInAppManager", c => c.Boolean(nullable: false));
            DropColumn("dbo.Master_Applications", "IsEnabled");
            DropColumn("dbo.Master_Applications", "IsPublished");
        }
    }
}
