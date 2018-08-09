namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestrydesignerattributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_BlocksCommits", "AssociatedTableIds", c => c.String());
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ColumnId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ColumnId");
            DropColumn("dbo.TapestryDesigner_BlocksCommits", "AssociatedTableIds");
        }
    }
}
