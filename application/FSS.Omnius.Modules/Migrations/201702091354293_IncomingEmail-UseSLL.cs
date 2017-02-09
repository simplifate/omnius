namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncomingEmailUseSLL : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Incoming_Email", "ImapUseSSL", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Incoming_Email", "ImapUseSSL");
        }
    }
}
