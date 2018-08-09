namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceMappingPairUpgrade1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "relationType", c => c.String(maxLength: 100));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "SourceComponentName", c => c.String(maxLength: 100));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "SourceTableName", c => c.String(maxLength: 100));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "SourceColumnName", c => c.String(maxLength: 100));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "TargetTableName", c => c.String(maxLength: 100));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "TargetColumnName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetName", c => c.String(maxLength: 100));
            DropColumn("dbo.TapestryDesigner_ResourceItems", "InputVariables");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "OutputVariables");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "OutputVariables", c => c.String());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "InputVariables", c => c.String());
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "TargetName", c => c.String());
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "TargetColumnName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "TargetTableName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "SourceColumnName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "SourceTableName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "SourceComponentName");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "relationType");
        }
    }
}
