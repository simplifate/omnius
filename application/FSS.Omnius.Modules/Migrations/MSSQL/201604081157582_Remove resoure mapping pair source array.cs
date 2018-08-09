namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removeresouremappingpairsourcearray : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs");
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "ResourceMappingPair_Id" });
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ResourceMappingPair_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ResourceMappingPair_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ResourceItems", "ResourceMappingPair_Id");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ResourceMappingPair_Id", "dbo.Tapestry_ResourceMappingPairs", "Id");
        }
    }
}
