namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestryattributenames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ComponentName", c => c.String());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "TableName", c => c.String());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ColumnName", c => c.String());
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ComponentId");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "TableId");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ColumnId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ColumnId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "TableId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ComponentId", c => c.Int());
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ColumnName");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "TableName");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ComponentName");
        }
    }
}
