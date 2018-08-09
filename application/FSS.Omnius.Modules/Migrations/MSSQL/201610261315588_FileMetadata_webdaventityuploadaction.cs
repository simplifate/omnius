namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileMetadata_webdaventityuploadaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_FileMetadataRecords", "Description", c => c.String());
            AddColumn("dbo.Nexus_FileMetadataRecords", "ModelEntityName", c => c.String());
            AddColumn("dbo.Nexus_FileMetadataRecords", "ModelEntityId", c => c.Int(nullable: false));
            AddColumn("dbo.Nexus_FileMetadataRecords", "Tag", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_FileMetadataRecords", "Tag");
            DropColumn("dbo.Nexus_FileMetadataRecords", "ModelEntityId");
            DropColumn("dbo.Nexus_FileMetadataRecords", "ModelEntityName");
            DropColumn("dbo.Nexus_FileMetadataRecords", "Description");
        }
    }
}
