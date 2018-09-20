namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestrySubflow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_Subflow",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Comment = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentSwimlaneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_Swimlanes", t => t.ParentSwimlaneId, cascadeDelete: true)
                .Index(t => t.ParentSwimlaneId);
            
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId", "dbo.TapestryDesigner_Subflow");
            DropForeignKey("dbo.TapestryDesigner_Subflow", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.TapestryDesigner_Subflow", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSubflowId" });
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSubflowId");
            DropTable("dbo.TapestryDesigner_Subflow");
        }
    }
}
