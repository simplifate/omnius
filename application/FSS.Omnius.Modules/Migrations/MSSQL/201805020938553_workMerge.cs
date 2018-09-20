namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workMerge : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            RenameIndex(table: "dbo.Hermes_Email_Template", name: "Index_Hermes_EmailTemplate_AppId_Name", newName: "HermesUniqueness");
            CreateTable(
                "dbo.RabbitMQs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        HostName = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        QueueName = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        BlockName = c.String(),
                        WorkflowName = c.String(),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Persona_BadLoginCount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IP = c.String(maxLength: 60),
                        AttemptsCount = c.Int(nullable: false),
                        LastAtempt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id", c => c.Int());
            AddColumn("dbo.Persona_Users", "LastIp", c => c.String(maxLength: 50));
            AddColumn("dbo.Persona_Users", "LastAppCookie", c => c.String());
            AddColumn("dbo.Nexus_FileMetadataRecords", "AbsoluteURL", c => c.String());
            AlterColumn("dbo.Hermes_Email_Queue", "ApplicationId", c => c.Int());
            CreateIndex("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id");
            CreateIndex("dbo.Hermes_Email_Queue", "ApplicationId");
            AddForeignKey("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id", "dbo.Persona_AppRoles", "Id");
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
            DropTable("dbo.Entitron___META");

            // local string to int
            AddColumn("dbo.Persona_Users", "TempLocal", c => c.Int());
            Sql("UPDATE dbo.Persona_Users SET TempLocal = 2 WHERE Locale = 'en'");
            Sql("UPDATE dbo.Persona_Users SET TempLocal = 1 WHERE TempLocal IS NULL");
            Sql("UPDATE dbo.Persona_Users SET Locale = ''");
            AlterColumn("dbo.Persona_Users", "Locale", c => c.Int(nullable: false, defaultValue: 1));
            Sql("UPDATE dbo.Persona_Users SET Locale = TempLocal");
            DropColumn("dbo.Persona_Users", "TempLocal");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Entitron___META",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ApplicationName = c.String(maxLength: 50),
                        tableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id", "dbo.Persona_AppRoles");
            DropForeignKey("dbo.RabbitMQs", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Email_Queue", new[] { "ApplicationId" });
            DropIndex("dbo.RabbitMQs", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "PersonaAppRole_Id" });
            AlterColumn("dbo.Hermes_Email_Queue", "ApplicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Persona_Users", "Locale", c => c.String(maxLength: 2));
            DropColumn("dbo.Nexus_FileMetadataRecords", "AbsoluteURL");
            DropColumn("dbo.Persona_Users", "LastAppCookie");
            DropColumn("dbo.Persona_Users", "LastIp");
            DropColumn("dbo.Persona_ActionRuleRights", "PersonaAppRole_Id");
            DropTable("dbo.Persona_BadLoginCount");
            DropTable("dbo.RabbitMQs");
            RenameIndex(table: "dbo.Hermes_Email_Template", name: "HermesUniqueness", newName: "Index_Hermes_EmailTemplate_AppId_Name");
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationName", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            CreateIndex("dbo.Hermes_Email_Queue", "ApplicationId");
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
        }
    }
}
