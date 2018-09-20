namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbschemecommit_update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "Application_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "Application_Id", c => c.Int());
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id");
        }
    }
}
