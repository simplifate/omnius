namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Entitron2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "DB_Type", c => c.Int(nullable: false));
            RenameColumn("dbo.Master_Applications", "connectionString_data", "DB_ConnectionString");
            RenameColumn("dbo.Master_Applications", "connectionString_schema", "DBscheme_connectionString");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Master_Applications", "DB_ConnectionString", "connectionString_data");
            RenameColumn("dbo.Master_Applications", "DBscheme_connectionString", "connectionString_schema");
            DropColumn("dbo.Master_Applications", "DB_Type");
        }
    }
}
