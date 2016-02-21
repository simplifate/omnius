namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdentityUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_AppRoles", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Persona_AppRoles", new[] { "Application_Id" });
            DropIndex("dbo.Persona_Users", new[] { "username" });
            CreateTable(
                "dbo.Persona_UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_UserLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Persona_User_Role",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Persona_Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Persona_AppRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            AddColumn("dbo.Persona_AppRoles", "ADgroupId", c => c.Int(nullable: false));
            AddColumn("dbo.Persona_AppRoles", "Name", c => c.String());
            AddColumn("dbo.Persona_Users", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_Users", "PasswordHash", c => c.String());
            AddColumn("dbo.Persona_Users", "SecurityStamp", c => c.String());
            AddColumn("dbo.Persona_Users", "PhoneNumber", c => c.String());
            AddColumn("dbo.Persona_Users", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_Users", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_Users", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.Persona_Users", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Persona_Users", "AccessFailedCount", c => c.Int(nullable: false));
            AlterColumn("dbo.Persona_Users", "UserName", c => c.String());
            AlterColumn("dbo.Persona_Users", "Email", c => c.String());
            CreateIndex("dbo.Persona_AppRoles", "ADgroupId");
            AddForeignKey("dbo.Persona_AppRoles", "ADgroupId", "dbo.Persona_ADgroups", "Id", cascadeDelete: true);
            DropColumn("dbo.Persona_AppRoles", "RoleName");
            DropColumn("dbo.Persona_AppRoles", "MembersList");
            DropColumn("dbo.Persona_AppRoles", "Application_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_AppRoles", "Application_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Persona_AppRoles", "MembersList", c => c.String());
            AddColumn("dbo.Persona_AppRoles", "RoleName", c => c.String());
            DropForeignKey("dbo.Persona_User_Role", "RoleId", "dbo.Persona_AppRoles");
            DropForeignKey("dbo.Persona_AppRoles", "ADgroupId", "dbo.Persona_ADgroups");
            DropForeignKey("dbo.Persona_User_Role", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_UserLogin", "UserId", "dbo.Persona_Users");
            DropForeignKey("dbo.Persona_UserClaim", "UserId", "dbo.Persona_Users");
            DropIndex("dbo.Persona_User_Role", new[] { "RoleId" });
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_UserLogin", new[] { "UserId" });
            DropIndex("dbo.Persona_UserClaim", new[] { "UserId" });
            DropIndex("dbo.Persona_AppRoles", new[] { "ADgroupId" });
            AlterColumn("dbo.Persona_Users", "Email", c => c.String(maxLength: 100));
            AlterColumn("dbo.Persona_Users", "UserName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Persona_Users", "AccessFailedCount");
            DropColumn("dbo.Persona_Users", "LockoutEnabled");
            DropColumn("dbo.Persona_Users", "LockoutEndDateUtc");
            DropColumn("dbo.Persona_Users", "TwoFactorEnabled");
            DropColumn("dbo.Persona_Users", "PhoneNumberConfirmed");
            DropColumn("dbo.Persona_Users", "PhoneNumber");
            DropColumn("dbo.Persona_Users", "SecurityStamp");
            DropColumn("dbo.Persona_Users", "PasswordHash");
            DropColumn("dbo.Persona_Users", "EmailConfirmed");
            DropColumn("dbo.Persona_AppRoles", "Name");
            DropColumn("dbo.Persona_AppRoles", "ADgroupId");
            DropTable("dbo.Persona_User_Role");
            DropTable("dbo.Persona_UserLogin");
            DropTable("dbo.Persona_UserClaim");
            CreateIndex("dbo.Persona_Users", "username", unique: true);
            CreateIndex("dbo.Persona_AppRoles", "Application_Id");
            AddForeignKey("dbo.Persona_AppRoles", "Application_Id", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
