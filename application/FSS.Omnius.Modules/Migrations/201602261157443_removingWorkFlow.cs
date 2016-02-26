namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingWorkFlow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks");
            AddForeignKey("dbo.ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks");
            AddForeignKey("dbo.ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks", "Id");
        }
    }
}
