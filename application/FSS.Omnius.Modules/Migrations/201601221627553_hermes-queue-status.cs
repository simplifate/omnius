namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermesqueuestatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Email_Queue", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Email_Queue", "Status");
        }
    }
}
