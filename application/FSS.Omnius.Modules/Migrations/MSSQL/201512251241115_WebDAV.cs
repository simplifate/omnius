namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebDAV : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LogItems", newName: "Watchtower_LogItems");
            CreateTable(
                "dbo.Nexus_CachedFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Blob = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Nexus_FileMetadataRecords", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Nexus_FileMetadataRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(),
                        AppFolderName = c.String(),
                        TimeCreated = c.DateTime(nullable: false),
                        TimeChanged = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        WebDavServer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Nexus_WebDavServers", t => t.WebDavServer_Id)
                .Index(t => t.WebDavServer_Id);
            
            CreateTable(
                "dbo.Nexus_WebDavServers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UriBasePath = c.String(),
                        AnonymousMode = c.Boolean(nullable: false),
                        AuthUsername = c.String(),
                        AuthPassword = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Nexus_FileMetadataRecords", "WebDavServer_Id", "dbo.Nexus_WebDavServers");
            DropForeignKey("dbo.Nexus_CachedFiles", "Id", "dbo.Nexus_FileMetadataRecords");
            DropIndex("dbo.Nexus_FileMetadataRecords", new[] { "WebDavServer_Id" });
            DropIndex("dbo.Nexus_CachedFiles", new[] { "Id" });
            DropTable("dbo.Nexus_WebDavServers");
            DropTable("dbo.Nexus_FileMetadataRecords");
            DropTable("dbo.Nexus_CachedFiles");
            RenameTable(name: "dbo.Watchtower_LogItems", newName: "LogItems");
        }
    }
}
