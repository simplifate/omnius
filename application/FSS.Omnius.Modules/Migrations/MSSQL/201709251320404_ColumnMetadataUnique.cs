namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColumnMetadataUnique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Entitron_ColumnMetadata", new[] { "Application_Id" });
            RenameColumn(table: "dbo.Entitron_ColumnMetadata", name: "Application_Id", newName: "ApplicationId");
            AlterColumn("dbo.Entitron_ColumnMetadata", "TableName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Entitron_ColumnMetadata", "ColumnName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Entitron_ColumnMetadata", "ColumnDisplayName", c => c.String(maxLength: 100));
            CreateIndex("dbo.Entitron_ColumnMetadata", new[] { "ApplicationId", "TableName", "ColumnName" }, unique: true, name: "UX_Entitron_ColumnMetadata");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Entitron_ColumnMetadata", "UX_Entitron_ColumnMetadata");
            AlterColumn("dbo.Entitron_ColumnMetadata", "ColumnDisplayName", c => c.String());
            AlterColumn("dbo.Entitron_ColumnMetadata", "ColumnName", c => c.String());
            AlterColumn("dbo.Entitron_ColumnMetadata", "TableName", c => c.String());
            RenameColumn(table: "dbo.Entitron_ColumnMetadata", name: "ApplicationId", newName: "Application_Id");
            CreateIndex("dbo.Entitron_ColumnMetadata", "Application_Id");
        }
    }
}
