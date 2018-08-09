namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAbsoluteURLtofilemetadata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nexus_FileMetadataRecords", "AbsoluteURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Nexus_FileMetadataRecords", "AbsoluteURL");
        }
    }
}
