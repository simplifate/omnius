namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryResourceMappingPairs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResourceMappingPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TargetName = c.String(),
                        TargetType = c.String(),
                        Source_Id = c.Int(),
                        Target_Id = c.Int(),
                        Block_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.Source_Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.Target_Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.Block_Id)
                .Index(t => t.Source_Id)
                .Index(t => t.Target_Id)
                .Index(t => t.Block_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResourceMappingPairs", "Block_Id", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.ResourceMappingPairs", "Target_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.ResourceMappingPairs", "Source_Id", "dbo.TapestryDesigner_ResourceItems");
            DropIndex("dbo.ResourceMappingPairs", new[] { "Block_Id" });
            DropIndex("dbo.ResourceMappingPairs", new[] { "Target_Id" });
            DropIndex("dbo.ResourceMappingPairs", new[] { "Source_Id" });
            DropTable("dbo.ResourceMappingPairs");
        }
    }
}
