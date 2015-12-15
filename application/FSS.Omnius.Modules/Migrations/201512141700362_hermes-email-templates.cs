namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermesemailtemplates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Email_Template",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 255),
                    Is_HTML = c.Boolean(nullable: false),
                    Content = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.Hermes_Email_Placeholder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hermes_Email_Template_Id = c.Int(),
                        Prop_Name = c.String(nullable: false, maxLength: 255),
                        Description = c.String(nullable: false, maxLength: 255),
                        Num_Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hermes_Email_Template", t => t.Hermes_Email_Template_Id)
                .Index(t => t.Hermes_Email_Template_Id)
                .Index(t => t.Prop_Name);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropIndex("dbo.Hermes_Email_Template", new[] { "Name" });
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Prop_Name" });
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Hermes_Email_Template_Id" });
            DropTable("dbo.Hermes_Email_Template");
            DropTable("dbo.Hermes_Email_Placeholder");
        }
    }
}
