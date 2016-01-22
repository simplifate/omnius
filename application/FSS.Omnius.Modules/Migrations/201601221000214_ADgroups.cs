namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADgroups : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_AppRights", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_AppRights", "UserId", "dbo.Persona_Users");
            DropIndex("dbo.Persona_AppRights", new[] { "UserId" });
            DropIndex("dbo.Persona_AppRights", new[] { "ApplicationId" });
            DropTable("dbo.Persona_AppRights");

            CreateTable(
                "dbo.Persona_ADgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.Persona_ADgroup_User",
                c => new
                    {
                        ADgroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ADgroupId, t.UserId })
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Persona_ADgroups", t => t.ADgroupId, cascadeDelete: true)
                .Index(t => t.ADgroupId)
                .Index(t => t.UserId);   
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Persona_AppRights",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                        hasAccess = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.ApplicationId });
            
            DropForeignKey("dbo.Persona_ADgroups", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Persona_ADgroup_User", "ADgroupId", "dbo.Persona_ADgroups");
            DropForeignKey("dbo.Persona_ADgroup_User", "UserId", "dbo.Persona_Users");
            DropIndex("dbo.Persona_ADgroup_User", new[] { "UserId" });
            DropIndex("dbo.Persona_ADgroup_User", new[] { "ADgroupId" });
            DropIndex("dbo.Persona_ADgroups", new[] { "ApplicationId" });
            DropTable("dbo.Persona_ADgroup_User");
            DropTable("dbo.Persona_ADgroups");
            CreateIndex("dbo.Persona_AppRights", "ApplicationId");
            CreateIndex("dbo.Persona_AppRights", "UserId");
            AddForeignKey("dbo.Persona_AppRights", "UserId", "dbo.Persona_Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Persona_AppRights", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
