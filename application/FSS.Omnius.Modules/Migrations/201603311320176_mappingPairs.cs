namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mappingPairs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks");
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.Tapestry_ResourceMappingPairs", new[] { "Block_Id" });
            RenameColumn(table: "dbo.Tapestry_ResourceMappingPairs", name: "Block_Id", newName: "BlockId");
            AlterColumn("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "BlockId", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.Tapestry_ResourceMappingPairs", "BlockId");
            AddForeignKey("dbo.Tapestry_ResourceMappingPairs", "BlockId", "dbo.Tapestry_Blocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_ResourceMappingPairs", "BlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_ResourceMappingPairs", new[] { "BlockId" });
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            AlterColumn("dbo.Tapestry_ResourceMappingPairs", "BlockId", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", c => c.Int());
            RenameColumn(table: "dbo.Tapestry_ResourceMappingPairs", name: "BlockId", newName: "Block_Id");
            CreateIndex("dbo.Tapestry_ResourceMappingPairs", "Block_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id");
            AddForeignKey("dbo.Tapestry_ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks", "Id");
        }
    }
}
