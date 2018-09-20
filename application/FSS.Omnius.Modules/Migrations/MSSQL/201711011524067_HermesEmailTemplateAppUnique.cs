namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HermesEmailTemplateAppUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Hermes_Email_Template", new[] { "AppId" });
            DropIndex("dbo.Hermes_Email_Template", new[] { "Name" });
            CreateIndex("dbo.Hermes_Email_Template", new[] { "AppId", "Name" }, unique: true, name: "Index_Hermes_EmailTemplate_AppId_Name");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hermes_Email_Template", "Index_Hermes_EmailTemplate_AppId_Name");
            CreateIndex("dbo.Hermes_Email_Template", "Name", unique: true);
            CreateIndex("dbo.Hermes_Email_Template", "AppId");
        }
    }
}
