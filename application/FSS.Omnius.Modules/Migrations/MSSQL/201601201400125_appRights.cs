namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appRights : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_ActionRuleRights", "GroupId", "dbo.Persona_Groups");
            DropForeignKey("dbo.Persona_AppRights", "GroupId", "dbo.Persona_Groups");
            DropForeignKey("dbo.Persona_Groups_Users", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_Groups_Users", "GroupId", "dbo.Persona_Groups");
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "GroupId" });
            DropIndex("dbo.Persona_AppRights", new[] { "GroupId" });
            DropIndex("dbo.Persona_Groups_Users", new[] { "UserId" });
            DropIndex("dbo.Persona_Groups_Users", new[] { "GroupId" });
            RenameColumn("dbo.Persona_ActionRuleRights", "GroupId", "AppRoleId");
            RenameColumn("dbo.Persona_AppRights", "GroupId", "UserId");
            AddColumn("dbo.Persona_AppRights", "hasAccess", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Persona_ActionRuleRights", "AppRoleId");
            CreateIndex("dbo.Persona_AppRights", "UserId");
            AddForeignKey("dbo.Persona_ActionRuleRights", "AppRoleId", "dbo.Persona_AppRoles", "Id");
            AddForeignKey("dbo.Persona_AppRights", "UserId", "dbo.Persona_Users", "Id", cascadeDelete: true);
            DropColumn("dbo.Persona_ActionRuleRights", "Readable");
            DropColumn("dbo.Persona_AppRights", "Readable");
            DropColumn("dbo.Persona_AppRights", "Executable");
            DropTable("dbo.Persona_Groups");
            DropTable("dbo.Persona_Groups_Users");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Persona_Groups_Users",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId });
            
            CreateTable(
                "dbo.Persona_Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsFromAD = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_AppRights", "Executable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_AppRights", "Readable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_AppRights", "GroupId", c => c.Int(nullable: false));
            AddColumn("dbo.Persona_ActionRuleRights", "Readable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_ActionRuleRights", "GroupId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Persona_AppRights", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_ActionRuleRights", "AppRoleId", "dbo.Persona_AppRoles");
            DropIndex("dbo.Persona_AppRights", new[] { "UserId" });
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "AppRoleId" });
            DropPrimaryKey("dbo.Persona_AppRights");
            DropPrimaryKey("dbo.Persona_ActionRuleRights");
            DropColumn("dbo.Persona_AppRights", "hasAccess");
            DropColumn("dbo.Persona_AppRights", "UserId");
            DropColumn("dbo.Persona_ActionRuleRights", "AppRoleId");
            AddPrimaryKey("dbo.Persona_AppRights", new[] { "GroupId", "ApplicationId" });
            AddPrimaryKey("dbo.Persona_ActionRuleRights", new[] { "GroupId", "ActionRuleId" });
            CreateIndex("dbo.Persona_Groups_Users", "GroupId");
            CreateIndex("dbo.Persona_Groups_Users", "UserId");
            CreateIndex("dbo.Persona_AppRights", "GroupId");
            CreateIndex("dbo.Persona_ActionRuleRights", "GroupId");
            AddForeignKey("dbo.Persona_Groups_Users", "GroupId", "dbo.Persona_Groups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Persona_Groups_Users", "UserId", "dbo.Persona_Users", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Persona_AppRights", "GroupId", "dbo.Persona_Groups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Persona_ActionRuleRights", "GroupId", "dbo.Persona_Groups", "Id", cascadeDelete: true);
        }
    }
}
