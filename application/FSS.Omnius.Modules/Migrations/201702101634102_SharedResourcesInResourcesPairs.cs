namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SharedResourcesInResourcesPairs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "TargetIsShared");
            DropColumn("dbo.Tapestry_ResourceMappingPairs", "SourceIsShared");
        }
    }
}
