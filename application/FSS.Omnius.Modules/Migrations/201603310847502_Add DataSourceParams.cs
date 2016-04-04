namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataSourceParams : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "DataSourceParams", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "DataSourceParams");
        }
    }
}
