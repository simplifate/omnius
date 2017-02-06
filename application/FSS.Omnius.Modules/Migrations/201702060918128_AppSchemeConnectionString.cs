namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppSchemeConnectionString : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Master_Applications", "connectionString", "connectionString_data");
            AddColumn("dbo.Master_Applications", "connectionString_schema", c => c.String());
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Master_Applications", "connectionString_data", "connectionString");
            DropColumn("dbo.Master_Applications", "connectionString_schema");
        }
    }
}
