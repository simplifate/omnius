namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncomingEmail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Incoming_Email_Rule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IncomingEmailId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        BlockName = c.String(),
                        WorkflowName = c.String(),
                        Rule = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Hermes_Incoming_Email", t => t.IncomingEmailId, cascadeDelete: true)
                .Index(t => t.IncomingEmailId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Hermes_Incoming_Email",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        ImapServer = c.String(nullable: false),
                        ImapPort = c.Int(),
                        UserName = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Incoming_Email_Rule", "IncomingEmailId", "dbo.Hermes_Incoming_Email");
            DropForeignKey("dbo.Hermes_Incoming_Email_Rule", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Hermes_Incoming_Email_Rule", new[] { "ApplicationId" });
            DropIndex("dbo.Hermes_Incoming_Email_Rule", new[] { "IncomingEmailId" });
            DropTable("dbo.Hermes_Incoming_Email");
            DropTable("dbo.Hermes_Incoming_Email_Rule");
        }
    }
}
