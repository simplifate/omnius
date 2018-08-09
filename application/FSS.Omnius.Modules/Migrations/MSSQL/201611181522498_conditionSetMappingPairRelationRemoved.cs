namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conditionSetMappingPairRelationRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ResourceMappingPair_Id" });
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ResourceMappingPair_Id" });
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs", "Id");
        }
    }
}
