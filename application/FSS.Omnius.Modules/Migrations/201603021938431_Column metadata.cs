namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Columnmetadata : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ResourceMappingPairs", newName: "Tapestry_ResourceMappingPairs");
            CreateTable(
                "dbo.Entitron_ColumnMetadata",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(),
                        ColumnName = c.String(),
                        ColumnDisplayName = c.String(),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.Application_Id, cascadeDelete: true)
                .Index(t => t.Application_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron_ColumnMetadata", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.Entitron_ColumnMetadata", new[] { "Application_Id" });
            DropTable("dbo.Entitron_ColumnMetadata");
            RenameTable(name: "dbo.Tapestry_ResourceMappingPairs", newName: "ResourceMappingPairs");
        }
    }
}
