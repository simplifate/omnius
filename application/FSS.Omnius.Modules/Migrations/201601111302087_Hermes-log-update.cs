namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Hermeslogupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Email_Log", "DateSend", c => c.DateTime(nullable: false));
            AddColumn("dbo.Hermes_Email_Log", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Hermes_Email_Log", "SMTP_Error", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Email_Log", "SMTP_Error");
            DropColumn("dbo.Hermes_Email_Log", "Status");
            DropColumn("dbo.Hermes_Email_Log", "DateSend");
        }
    }
}
