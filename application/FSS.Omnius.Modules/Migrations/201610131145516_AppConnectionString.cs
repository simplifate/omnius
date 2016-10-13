namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AppConnectionString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "connectionString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "connectionString");
        }
    }
}
