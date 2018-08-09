namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersApplications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Master_UsersApplications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ApplicationId);
            
            DropColumn("dbo.Master_Applications", "PositionX");
            DropColumn("dbo.Master_Applications", "PositionY");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Master_Applications", "PositionY", c => c.Int(nullable: false));
            AddColumn("dbo.Master_Applications", "PositionX", c => c.Int(nullable: false));
            DropForeignKey("dbo.Master_UsersApplications", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Master_UsersApplications", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Master_UsersApplications", new[] { "ApplicationId" });
            DropIndex("dbo.Master_UsersApplications", new[] { "UserId" });
            DropTable("dbo.Master_UsersApplications");
        }
    }
}
