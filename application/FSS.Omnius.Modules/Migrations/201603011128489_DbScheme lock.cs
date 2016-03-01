namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbSchemelock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "DbSchemeLocked", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "DbSchemeLocked");
        }
    }
}
