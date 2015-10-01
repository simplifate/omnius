namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageRelationsMerged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pages", "Relations", c => c.String(nullable: false, defaultValue: ""));
            DropColumn("dbo.Pages", "PartialRelations");
            DropColumn("dbo.Pages", "DatasourceRelations");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pages", "DatasourceRelations", c => c.String(nullable: false));
            AddColumn("dbo.Pages", "PartialRelations", c => c.String(nullable: false));
            DropColumn("dbo.Pages", "Relations");
        }
    }
}
