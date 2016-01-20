namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermesemailattachment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Email_Queue", "AttachmentList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Email_Queue", "AttachmentList");
        }
    }
}
