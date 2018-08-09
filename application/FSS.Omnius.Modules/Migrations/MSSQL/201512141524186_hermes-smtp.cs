namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermessmtp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Smtp",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Server = c.String(nullable: false, maxLength: 255),
                        Auth_User = c.String(nullable: false, maxLength: 255),
                        Auth_Password = c.String(nullable: false, maxLength: 255),
                        Use_SSL = c.Boolean(nullable: false),
                        Is_Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Use_SSL)
                .Index(t => t.Is_Default);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hermes_Smtp", new[] { "Is_Default" });
            DropIndex("dbo.Hermes_Smtp", new[] { "Use_SSL" });
            DropIndex("dbo.Hermes_Smtp", new[] { "Name" });
            DropTable("dbo.Hermes_Smtp");
        }
    }
}
