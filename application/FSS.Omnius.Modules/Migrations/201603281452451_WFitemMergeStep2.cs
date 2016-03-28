namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WFitemMergeStep2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowSymbols", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.TapestryDesigner_WorkflowSymbols", new[] { "ParentSwimlane_Id" });
            RenameColumn("dbo.TapestryDesigner_Connections", "Source", "SourceId");
            RenameColumn("dbo.TapestryDesigner_Connections", "Target", "TargetId");
            DropColumn("dbo.TapestryDesigner_Connections", "SourceType");
            DropColumn("dbo.TapestryDesigner_Connections", "TargetType");
            DropTable("dbo.TapestryDesigner_WorkflowSymbols");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TapestryDesigner_WorkflowSymbols",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Condition = c.String(),
                        TypeClass = c.String(),
                        DialogType = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentSwimlane_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);

            RenameColumn("dbo.TapestryDesigner_Connections", "SourceId", "Source");
            RenameColumn("dbo.TapestryDesigner_Connections", "TargetId", "Target");
            AddColumn("dbo.TapestryDesigner_Connections", "TargetType", c => c.Int(nullable: false));
            AddColumn("dbo.TapestryDesigner_Connections", "SourceType", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_WorkflowSymbols", "ParentSwimlane_Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowSymbols", "ParentSwimlane_Id", "dbo.TapestryDesigner_Swimlanes", "Id");
        }
    }
}
