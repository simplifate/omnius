namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailQueueOptionalApp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            AlterColumn("dbo.Hermes_Email_Queue", "ApplicationId", c => c.Int());
            CreateIndex("dbo.Hermes_Email_Queue", "ApplicationId");
            AddForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            AlterColumn("dbo.Hermes_Email_Queue", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Hermes_Email_Queue", "ApplicationId");
            AddForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
