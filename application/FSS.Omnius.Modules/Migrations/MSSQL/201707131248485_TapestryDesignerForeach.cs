namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryDesignerForeach : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_Foreach",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Comment = c.String(),
                        CommentBottom = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId, cascadeDelete: true)
                .Index(t => t.ParentSwimlaneId);
            
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd", c => c.Boolean());
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId", "dbo.TapestryDesigner_Foreach");
            DropForeignKey("dbo.TapestryDesigner_Foreach", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.TapestryDesigner_Foreach", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentForeachId" });
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachEnd");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsForeachStart");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentForeachId");
            DropTable("dbo.TapestryDesigner_Foreach");
        }
    }
}
