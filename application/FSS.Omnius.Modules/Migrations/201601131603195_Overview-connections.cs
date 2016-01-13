namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Overviewconnections : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        TapestryDesignerItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Items", t => t.TapestryDesignerItem_Id)
                .Index(t => t.TapestryDesignerItem_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_MetablocksConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceType = c.Int(nullable: false),
                        TargetType = c.Int(nullable: false),
                        SourceId = c.Int(),
                        TargetId = c.Int(),
                        TapestryDesignerMetablock_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Metablocks", t => t.TapestryDesignerMetablock_Id)
                .Index(t => t.TapestryDesignerMetablock_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_Properties", "TapestryDesignerItem_Id", "dbo.TapestryDesigner_Items");
            DropIndex("dbo.TapestryDesigner_MetablocksConnections", new[] { "TapestryDesignerMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Properties", new[] { "TapestryDesignerItem_Id" });
            DropTable("dbo.TapestryDesigner_MetablocksConnections");
            DropTable("dbo.TapestryDesigner_Properties");
        }
    }
}
