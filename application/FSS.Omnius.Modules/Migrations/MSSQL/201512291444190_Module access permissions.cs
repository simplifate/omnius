namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Moduleaccesspermissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Persona_ModuleAccessPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Core = c.Boolean(nullable: false),
                        Master = c.Boolean(nullable: false),
                        Tapestry = c.Boolean(nullable: false),
                        Entitron = c.Boolean(nullable: false),
                        Mozaic = c.Boolean(nullable: false),
                        Persona = c.Boolean(nullable: false),
                        Nexus = c.Boolean(nullable: false),
                        Sentry = c.Boolean(nullable: false),
                        Hermes = c.Boolean(nullable: false),
                        Athena = c.Boolean(nullable: false),
                        Watchtower = c.Boolean(nullable: false),
                        Cortex = c.Boolean(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Persona_Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Persona_ModuleAccessPermissions", "User_Id", "dbo.Persona_Users");
            DropIndex("dbo.Persona_ModuleAccessPermissions", new[] { "User_Id" });
            DropTable("dbo.Persona_ModuleAccessPermissions");
        }
    }
}
