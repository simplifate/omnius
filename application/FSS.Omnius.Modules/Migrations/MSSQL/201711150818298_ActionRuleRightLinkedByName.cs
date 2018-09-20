namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActionRuleRightLinkedByName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_ActionRuleRights", "AppRoleId", "dbo.Persona_AppRoles");
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "AppRoleId" });
            DropIndex("dbo.Persona_AppRoles", new[] { "ApplicationId" });
            DropPrimaryKey("dbo.Persona_ActionRuleRights");
            AddColumn("dbo.Persona_ActionRuleRights", "ApplicationId", c => c.Int());
            AddColumn("dbo.Persona_ActionRuleRights", "AppRoleName", c => c.String(maxLength: 50));
            Sql("UPDATE arr SET arr.AppRoleName = ar.Name, arr.ApplicationId = ar.ApplicationId FROM [Persona_ActionRuleRights] arr JOIN[Persona_AppRoles] ar ON ar.Id = arr.AppRoleId");
            AlterColumn("dbo.Persona_ActionRuleRights", "ApplicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Persona_ActionRuleRights", "AppRoleName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Persona_AppRoles", "Name", c => c.String(maxLength: 50));
            AddPrimaryKey("dbo.Persona_ActionRuleRights", new[] { "ApplicationId", "AppRoleName", "ActionRuleId" });
            CreateIndex("dbo.Persona_AppRoles", new[] { "ApplicationId", "Name" }, unique: true, name: "PersonaAppRole_AppName");
            DropColumn("dbo.Persona_ActionRuleRights", "AppRoleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_ActionRuleRights", "AppRoleId", c => c.Int());
            DropIndex("dbo.Persona_AppRoles", "PersonaAppRole_AppName");
            DropPrimaryKey("dbo.Persona_ActionRuleRights");
            Sql("UPDATE arr SET arr.AppRoleId = ar.Id FROM [Persona_ActionRuleRights] arr JOIN[Persona_AppRoles] ar ON arr.AppRoleName = ar.Name AND arr.ApplicationId = ar.ApplicationId");
            AlterColumn("dbo.Persona_ActionRuleRights", "AppRoleId", c => c.Int(nullable: false));
            AlterColumn("dbo.Persona_AppRoles", "Name", c => c.String());
            DropColumn("dbo.Persona_ActionRuleRights", "AppRoleName");
            DropColumn("dbo.Persona_ActionRuleRights", "ApplicationId");
            AddPrimaryKey("dbo.Persona_ActionRuleRights", new[] { "AppRoleId", "ActionRuleId" });
            CreateIndex("dbo.Persona_AppRoles", "ApplicationId");
            CreateIndex("dbo.Persona_ActionRuleRights", "AppRoleId");
            AddForeignKey("dbo.Persona_ActionRuleRights", "AppRoleId", "dbo.Persona_AppRoles", "Id", cascadeDelete: true);
        }
    }
}
