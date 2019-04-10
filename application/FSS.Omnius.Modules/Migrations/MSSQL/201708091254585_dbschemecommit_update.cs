namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbschemecommit_update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "ApplicationId" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Entitron_DbSchemeCommit", "ApplicationId");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "ApplicationId" });
            AlterColumn("dbo.Entitron_DbSchemeCommit", "ApplicationId", c => c.Int());
            CreateIndex("dbo.Entitron_DbSchemeCommit", "ApplicationId");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "ApplicationId", "dbo.Master_Applications", "Id");
        }
    }
}
