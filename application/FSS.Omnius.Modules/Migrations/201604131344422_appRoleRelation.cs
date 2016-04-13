namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appRoleRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_AppRoles", "ADgroupId", "dbo.Persona_ADgroups");
            DropIndex("dbo.Persona_AppRoles", new[] { "ADgroupId" });
            AddColumn("dbo.Persona_AppRoles", "ApplicationId", c => c.Int(nullable: false));
            Sql("UPDATE dbo.Persona_AppRoles SET ApplicationId = ad.ApplicationId FROM dbo.Persona_AppRoles roles INNER JOIN dbo.Persona_ADgroups ad ON roles.ADgroupId = ad.Id");
            CreateIndex("dbo.Persona_AppRoles", "ApplicationId");
            AddForeignKey("dbo.Persona_AppRoles", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
            DropColumn("dbo.Persona_AppRoles", "ADgroupId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_AppRoles", "ADgroupId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Persona_AppRoles", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Persona_AppRoles", new[] { "ApplicationId" });
            DropColumn("dbo.Persona_AppRoles", "ApplicationId");
            CreateIndex("dbo.Persona_AppRoles", "ADgroupId");
            AddForeignKey("dbo.Persona_AppRoles", "ADgroupId", "dbo.Persona_ADgroups", "Id", cascadeDelete: true);
        }
    }
}
