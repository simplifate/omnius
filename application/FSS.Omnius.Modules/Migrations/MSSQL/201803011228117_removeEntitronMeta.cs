namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeEntitronMeta : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            DropTable("dbo.Entitron___META");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Entitron___META",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ApplicationId = c.Int(),
                        tableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationId", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            AddForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications", "Id");
        }
    }
}
