namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EntitronMeta_applicationName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            AddColumn("dbo.Entitron___META", "ApplicationName", c => c.String(maxLength: 50));
            Sql("UPDATE e SET e.ApplicationName = a.Name FROM dbo.Entitron___META e JOIN dbo.Master_Applications a ON e.ApplicationId = a.Id");
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationName", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            DropColumn("dbo.Entitron___META", "ApplicationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Entitron___META", "ApplicationId", c => c.Int());
            Sql("UPDATE e SET e.ApplicationId = a.Id FROM dbo.Entitron___META e JOIN dbo.Master_Applications a ON e.ApplicationName = a.Name");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            DropColumn("dbo.Entitron___META", "ApplicationName");
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationId", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            AddForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications", "Id");
        }
    }
}
