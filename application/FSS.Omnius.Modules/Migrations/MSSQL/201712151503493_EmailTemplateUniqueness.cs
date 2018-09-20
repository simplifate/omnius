namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailTemplateUniqueness : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Hermes_Email_Template", new[] { "AppId" });
            DropIndex("dbo.Hermes_Email_Template", new[] { "Name" });
            CreateIndex("dbo.Hermes_Email_Template", new[] { "AppId", "Name" }, unique: true, name: "EmailTemplateUnique");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hermes_Email_Template", "EmailTemplateUnique");
            CreateIndex("dbo.Hermes_Email_Template", "Name", unique: true);
            CreateIndex("dbo.Hermes_Email_Template", "AppId");
        }
    }
}
