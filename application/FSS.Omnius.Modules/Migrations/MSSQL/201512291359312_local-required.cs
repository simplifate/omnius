namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class localrequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "User_Id" });
            DropColumn("dbo.Persona_ModuleAccessPermissions", "Id");
            RenameColumn(table: "dbo.Persona_ModuleAccessPermissions", name: "User_Id", newName: "Id");
            DropPrimaryKey("dbo.Persona_ModuleAccessPermissions");
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Persona_ModuleAccessPermissions", "Id");
            CreateIndex("dbo.Persona_ModuleAccessPermissions", "Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "Id" });
            DropPrimaryKey("dbo.Persona_ModuleAccessPermissions");
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int());
            AlterColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Persona_ModuleAccessPermissions", "Id");
            RenameColumn(table: "dbo.Persona_ModuleAccessPermissions", name: "Id", newName: "User_Id");
            AddColumn("dbo.Persona_ModuleAccessPermissions", "Id", c => c.Int(nullable: false, identity: true));
            CreateIndex("dbo.Persona_ModuleAccessPermissions", "User_Id");
        }
    }
}
