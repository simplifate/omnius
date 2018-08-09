namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WatchtowerLogItems : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Hermes_Email_Queue", "Application_Id", "ApplicationId");
            Sql("UPDATE dbo.Hermes_Email_Queue SET ApplicationId = (SELECT Id FROM dbo.Master_Applications WHERE IsSystem = 1) WHERE ApplicationId IS NULL;");
            AlterColumn("dbo.Hermes_Email_Queue", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Hermes_Email_Queue", "ApplicationId");
            AddForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);

            AddColumn("dbo.Watchtower_LogItems", "UserName", c => c.String(maxLength: 50));
            AddColumn("dbo.Watchtower_LogItems", "Server", c => c.String(maxLength: 50));
            AddColumn("dbo.Watchtower_LogItems", "Source", c => c.Int(nullable: false));
            AddColumn("dbo.Watchtower_LogItems", "Application", c => c.String(maxLength: 50));
            AddColumn("dbo.Watchtower_LogItems", "BlockName", c => c.String(maxLength: 100));
            AddColumn("dbo.Watchtower_LogItems", "ActionName", c => c.String(maxLength: 50));
            AddColumn("dbo.Watchtower_LogItems", "Vars", c => c.String());
            AddColumn("dbo.Watchtower_LogItems", "StackTrace", c => c.String());
            CreateIndex("dbo.Watchtower_LogItems", "Timestamp");
            CreateIndex("dbo.Watchtower_LogItems", "UserName");
            CreateIndex("dbo.Watchtower_LogItems", "Server");
            CreateIndex("dbo.Watchtower_LogItems", "Source");
            CreateIndex("dbo.Watchtower_LogItems", "Application");
            DropColumn("dbo.Watchtower_LogItems", "IsPlatformEvent");
            DropColumn("dbo.Watchtower_LogItems", "LogEventType");
            Sql("UPDATE W SET W.UserName = u.UserName, W.Application = a.Name FROM dbo.Watchtower_LogItems W JOIN dbo.Master_Applications a ON a.Id = W.AppId JOIN dbo.Persona_Users u ON u.Id = W.UserId");
            DropColumn("dbo.Watchtower_LogItems", "UserId");
            DropColumn("dbo.Watchtower_LogItems", "AppId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Email_Queue", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            RenameColumn("dbo.Hermes_Email_Queue", "ApplicationId", "Application_Id");
            AlterColumn("dbo.Hermes_Email_Queue", "Application_Id", c => c.Int());

            AddColumn("dbo.Watchtower_LogItems", "AppId", c => c.Int());
            AddColumn("dbo.Watchtower_LogItems", "UserId", c => c.Int());
            Sql("UPDATE W SET W.UserId = u.Id, W.AppId = a.Id FROM dbo.Watchtower_LogItems W JOIN dbo.Master_Applications a ON a.Name = W.Application JOIN dbo.Persona_Users u ON u.UserName = W.UserName");
            Sql("UPDATE dbo.Watchtower_LogItems SET UserId = 0 WHERE UserId IS NULL;");
            AlterColumn("dbo.Watchtower_LogItems", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.Watchtower_LogItems", "IsPlatformEvent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Watchtower_LogItems", "LogEventType", c => c.Int(nullable: false, defaultValue: 0));
            DropIndex("dbo.Watchtower_LogItems", new[] { "Application" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Source" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Server" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "UserName" });
            DropIndex("dbo.Watchtower_LogItems", new[] { "Timestamp" });
            DropColumn("dbo.Watchtower_LogItems", "StackTrace");
            DropColumn("dbo.Watchtower_LogItems", "Vars");
            DropColumn("dbo.Watchtower_LogItems", "ActionName");
            DropColumn("dbo.Watchtower_LogItems", "BlockName");
            DropColumn("dbo.Watchtower_LogItems", "Application");
            DropColumn("dbo.Watchtower_LogItems", "Source");
            DropColumn("dbo.Watchtower_LogItems", "Server");
            DropColumn("dbo.Watchtower_LogItems", "UserName");
        }
    }
}
