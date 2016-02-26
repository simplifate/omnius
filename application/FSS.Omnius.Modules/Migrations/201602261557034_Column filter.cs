namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Columnfilter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceMappingPairs", "SourceColumnFilter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResourceMappingPairs", "SourceColumnFilter");
        }
    }
}
