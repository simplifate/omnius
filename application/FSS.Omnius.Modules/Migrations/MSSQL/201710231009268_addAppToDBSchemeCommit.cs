namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAppToDBSchemeCommit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            RenameColumn(table: "dbo.Entitron_DbSchemeCommit", name: "Application_Id", newName: "ApplicationId");
            AlterColumn("dbo.Entitron_DbSchemeCommit", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Entitron_DbSchemeCommit", "ApplicationId");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "ApplicationId" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "ApplicationId", c => c.Int());
            RenameColumn(table: "dbo.Entitron_DbSchemeCommit", name: "ApplicationId", newName: "Application_Id");
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id");
        }
    }
}
