namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "DB_Type", c => c.Int(nullable: false));
            AddColumn("dbo.Master_Applications", "DB_ConnectionString", c => c.String());
            AddColumn("dbo.Master_Applications", "DBscheme_connectionString", c => c.String());
            DropColumn("dbo.Master_Applications", "connectionString_schema");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Master_Applications", "connectionString_schema", c => c.String());
            DropColumn("dbo.Master_Applications", "DBscheme_connectionString");
            DropColumn("dbo.Master_Applications", "DB_ConnectionString");
            DropColumn("dbo.Master_Applications", "DB_Type");
        }
    }
}
