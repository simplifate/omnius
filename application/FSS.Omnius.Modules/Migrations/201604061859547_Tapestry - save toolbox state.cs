namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestrysavetoolboxstate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_ToolboxStates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssociatedBlockCommit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_BlocksCommits", t => t.AssociatedBlockCommit_Id)
                .Index(t => t.AssociatedBlockCommit_Id);
            
            CreateTable(
                "dbo.TapestryDesigner_ToolboxItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeClass = c.String(),
                        Label = c.String(),
                        ActionId = c.Int(),
                        TableName = c.String(),
                        ColumnName = c.String(),
                        PageId = c.Int(),
                        ComponentName = c.String(),
                        StateId = c.Int(),
                        TargetName = c.String(),
                        TargetId = c.Int(),
                        TapestryDesignerToolboxState_Id = c.Int(),
                        TapestryDesignerToolboxState_Id1 = c.Int(),
                        TapestryDesignerToolboxState_Id2 = c.Int(),
                        TapestryDesignerToolboxState_Id3 = c.Int(),
                        TapestryDesignerToolboxState_Id4 = c.Int(),
                        TapestryDesignerToolboxState_Id5 = c.Int(),
                        TapestryDesignerToolboxState_Id6 = c.Int(),
                        TapestryDesignerToolboxState_Id7 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id1)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id2)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id3)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id4)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id5)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id6)
                .ForeignKey("dbo.TapestryDesigner_ToolboxStates", t => t.TapestryDesignerToolboxState_Id7)
                .Index(t => t.TapestryDesignerToolboxState_Id)
                .Index(t => t.TapestryDesignerToolboxState_Id1)
                .Index(t => t.TapestryDesignerToolboxState_Id2)
                .Index(t => t.TapestryDesignerToolboxState_Id3)
                .Index(t => t.TapestryDesignerToolboxState_Id4)
                .Index(t => t.TapestryDesignerToolboxState_Id5)
                .Index(t => t.TapestryDesignerToolboxState_Id6)
                .Index(t => t.TapestryDesignerToolboxState_Id7);
            
            AddColumn("dbo.TapestryDesigner_Blocks", "ToolboxState_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_Blocks", "ToolboxState_Id");
            AddForeignKey("dbo.TapestryDesigner_Blocks", "ToolboxState_Id", "dbo.TapestryDesigner_ToolboxStates", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_Blocks", "ToolboxState_Id", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id7", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id6", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id5", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id4", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id3", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id2", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id1", "dbo.TapestryDesigner_ToolboxStates");
            DropForeignKey("dbo.TapestryDesigner_ToolboxStates", "AssociatedBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropForeignKey("dbo.TapestryDesigner_ToolboxItems", "TapestryDesignerToolboxState_Id", "dbo.TapestryDesigner_ToolboxStates");
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id7" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id6" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id5" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id4" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id3" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id2" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id1" });
            DropIndex("dbo.TapestryDesigner_ToolboxItems", new[] { "TapestryDesignerToolboxState_Id" });
            DropIndex("dbo.TapestryDesigner_ToolboxStates", new[] { "AssociatedBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ToolboxState_Id" });
            DropColumn("dbo.TapestryDesigner_Blocks", "ToolboxState_Id");
            DropTable("dbo.TapestryDesigner_ToolboxItems");
            DropTable("dbo.TapestryDesigner_ToolboxStates");
        }
    }
}
