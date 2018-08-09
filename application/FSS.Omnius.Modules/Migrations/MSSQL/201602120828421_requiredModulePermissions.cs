namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredModulePermissions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_ModuleAccessPermissions", "User_Id", "dbo.Persona_Users");
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "User_Id" });
            RenameColumn(table: "dbo.Persona_ModuleAccessPermissions", name: "User_Id", newName: "UserId");
            DropPrimaryKey("dbo.Persona_ModuleAccessPermissions");
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "UserId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Persona_ModuleAccessPermissions", "UserId");
            CreateIndex("dbo.Persona_ModuleAccessPermissions", "UserId");
            AddForeignKey("dbo.Persona_ModuleAccessPermissions", "UserId", "dbo.Persona_Users", "Id", cascadeDelete: true);
            DropColumn("dbo.Persona_ModuleAccessPermissions", "Id");

            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Core", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Master", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Tapestry", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Entitron", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Mozaic", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Persona", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Nexus", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Sentry", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Hermes", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Athena", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Watchtower", c => c.Boolean(nullable: false, defaultValue: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Cortex", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Persona_ModuleAccessPermissions", "UserId", "dbo.Persona_Users");
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "UserId" });
            DropPrimaryKey("dbo.Persona_ModuleAccessPermissions");
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "UserId", c => c.Int());
            AddPrimaryKey("dbo.Persona_ModuleAccessPermissions", "Id");
            RenameColumn(table: "dbo.Persona_ModuleAccessPermissions", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.Persona_ModuleAccessPermissions", "User_Id");
            AddForeignKey("dbo.Persona_ModuleAccessPermissions", "User_Id", "dbo.Persona_Users", "Id");
        }
    }
}
