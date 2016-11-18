namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceMappingPairUpgrade2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_ResourceMappingPairs", "Source_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.Tapestry_ResourceMappingPairs", "Target_Id", "dbo.TapestryDesigner_ResourceItems");
            DropIndex("dbo.Tapestry_ResourceMappingPairs", new[] { "Source_Id" });
            DropIndex("dbo.Tapestry_ResourceMappingPairs", new[] { "Target_Id" });
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ResourceMappingPair_Id" });
            DropColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            CreateIndex("dbo.Tapestry_ResourceMappingPairs", "Target_Id");
            CreateIndex("dbo.Tapestry_ResourceMappingPairs", "Source_Id");
            AddForeignKey("dbo.Tapestry_ResourceMappingPairs", "Target_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
            AddForeignKey("dbo.Tapestry_ResourceMappingPairs", "Source_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
        }
    }
}
