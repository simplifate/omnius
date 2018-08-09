namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DontUseIdentityRoles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_User_Role", "RoleId", "dbo.Persona_AppRoles");
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_User_Role", new[] { "RoleId" });
            DropPrimaryKey("dbo.Persona_User_Role");
            CreateTable(
                "dbo.Persona_Identity_UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Iden_Role_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Persona_Identity_Roles", t => t.Iden_Role_Id)
                .Index(t => t.UserId)
                .Index(t => t.Iden_Role_Id);
            
            CreateTable(
                "dbo.Persona_Identity_Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_User_Role", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Persona_User_Role", "RoleName", c => c.String(maxLength: 50));
            AddColumn("dbo.Persona_User_Role", "ApplicationId", c => c.Int());
            Sql("UPDATE ur SET ur.RoleName = ar.Name, ur.ApplicationId = ar.ApplicationId FROM dbo.Persona_User_Role ur JOIN dbo.Persona_AppRoles ar ON ur.RoleId = ar.Id");
            AlterColumn("dbo.Persona_User_Role", "RoleName", c => c.String(maxLength: 50, nullable: false));
            AlterColumn("dbo.Persona_User_Role", "ApplicationId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Persona_User_Role", "Id");
            CreateIndex("dbo.Persona_User_Role", "UserId");
            CreateIndex("dbo.Persona_User_Role", "RoleName");
            CreateIndex("dbo.Persona_User_Role", "ApplicationId");
            AddForeignKey("dbo.Persona_User_Role", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: false);
            DropColumn("dbo.Persona_User_Role", "RoleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Persona_Identity_UserRoles", "Iden_Role_Id", "dbo.Persona_Identity_Roles");
            DropForeignKey("dbo.Persona_User_Role", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Persona_User_Role", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_User_Role", new[] { "RoleName" });
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "Iden_Role_Id" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "UserId" });
            DropPrimaryKey("dbo.Persona_User_Role");
            AddColumn("dbo.Persona_User_Role", "RoleId", c => c.Int());
            Sql("UPDATE ur SET ur.RoleId = ar.Id FROM dbo.Persona_User_Role ur JOIN dbo.Persona_AppRoles ar ON ur.RoleName = ar.Name AND ur.ApplicationId = ar.ApplicationId");
            AlterColumn("dbo.Persona_User_Role", "RoleId", c => c.Int(nullable: false));
            DropColumn("dbo.Persona_User_Role", "ApplicationId");
            DropColumn("dbo.Persona_User_Role", "RoleName");
            DropColumn("dbo.Persona_User_Role", "Id");
            DropTable("dbo.Persona_Identity_Roles");
            DropTable("dbo.Persona_Identity_UserRoles");
            AddPrimaryKey("dbo.Persona_User_Role", new[] { "UserId", "RoleId" });
            CreateIndex("dbo.Persona_User_Role", "RoleId");
            CreateIndex("dbo.Persona_User_Role", "UserId");
            AddForeignKey("dbo.Persona_User_Role", "RoleId", "dbo.Persona_AppRoles", "Id", cascadeDelete: true);
        }
    }
}
