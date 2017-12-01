namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tableApplication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "IsAllowedGuests", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "IsAllowedGuests");
        }
    }
}
