namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HermesTemplateContent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Email_Template_Content",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hermes_Email_Template_Id = c.Int(),
                        LanguageId = c.Int(),
                        From_Name = c.String(maxLength: 255),
                        From_Email = c.String(maxLength: 1000),
                        Subject = c.String(maxLength: 1000),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hermes_Email_Template", t => t.Hermes_Email_Template_Id)
                .Index(t => t.Hermes_Email_Template_Id)
                .Index(t => t.LanguageId);
            
            DropColumn("dbo.Hermes_Email_Template", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Hermes_Email_Template", "Content", c => c.String());
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "LanguageId" });
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "Hermes_Email_Template_Id" });
            DropTable("dbo.Hermes_Email_Template_Content");
        }
    }
}
