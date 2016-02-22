namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Columnfilterinresourcerules : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ColumnFilter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ColumnFilter");
        }
    }
}
