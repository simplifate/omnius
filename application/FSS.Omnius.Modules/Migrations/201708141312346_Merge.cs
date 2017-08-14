namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Merge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_Ext_DB", "DB_Alias", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_Ext_DB", "DB_Alias");
        }
    }
}
