namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeEntitronMeta : DbMigration
    {
        public override void Up()
        {
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
                        ApplicationName = c.String(maxLength: 50),
                        tableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationName", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
        }
    }
}
