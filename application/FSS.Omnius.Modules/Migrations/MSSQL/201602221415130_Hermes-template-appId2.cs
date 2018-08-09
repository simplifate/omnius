namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HermestemplateappId2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Hermes_Email_Template", "AppId");
            AddForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Email_Template", new[] { "AppId" });
        }
    }
}
