namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sharedTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            AlterColumn("dbo.Entitron___META", "ApplicationId", c => c.Int());
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationId", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            AddForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            AlterColumn("dbo.Entitron___META", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationId", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            AddForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
