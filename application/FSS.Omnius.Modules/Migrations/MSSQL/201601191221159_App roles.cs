namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Approles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Persona_AppRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        MembersList = c.String(),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.Application_Id, cascadeDelete: true)
                .Index(t => t.Application_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Persona_AppRoles", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Persona_AppRoles", new[] { "Application_Id" });
            DropTable("dbo.Persona_AppRoles");
        }
    }
}
