namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeOldWFDesignerModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Properties", "TapestryDesignerItem_Id", "dbo.TapestryDesigner_Items");
            DropForeignKey("dbo.TapestryDesigner_Operators", "ParentRule_Id", "dbo.TapestryDesigner_Rules");
            DropForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "TapestryDesignerRule_Id" });
            DropIndex("dbo.TapestryDesigner_Rules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Items", new[] { "ParentRule_Id" });
            DropIndex("dbo.TapestryDesigner_Properties", new[] { "TapestryDesignerItem_Id" });
            DropIndex("dbo.TapestryDesigner_Operators", new[] { "ParentRule_Id" });
            RenameColumn(table: "dbo.TapestryDesigner_Connections", name: "TapestryDesignerResourceRule_Id", newName: "ResourceRuleId");
            RenameColumn(table: "dbo.TapestryDesigner_Connections", name: "TapestryDesignerWorkflowRule_Id", newName: "WorkflowRuleId");
            RenameIndex(table: "dbo.TapestryDesigner_Connections", name: "IX_TapestryDesignerWorkflowRule_Id", newName: "IX_WorkflowRuleId");
            RenameIndex(table: "dbo.TapestryDesigner_Connections", name: "IX_TapestryDesignerResourceRule_Id", newName: "IX_ResourceRuleId");
            DropColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id");
            DropTable("dbo.TapestryDesigner_Rules");
            DropTable("dbo.TapestryDesigner_Items");
            DropTable("dbo.TapestryDesigner_Properties");
            DropTable("dbo.TapestryDesigner_Operators");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TapestryDesigner_Operators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        DialogType = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                        TapestryDesignerItem_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemReferenceId = c.Int(nullable: false),
                        Label = c.String(),
                        TypeClass = c.String(),
                        DialogType = c.String(),
                        IsDataSource = c.Boolean(nullable: false),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        ParentRule_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_Rules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        ParentBlockCommit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id", c => c.Int());
            RenameIndex(table: "dbo.TapestryDesigner_Connections", name: "IX_ResourceRuleId", newName: "IX_TapestryDesignerResourceRule_Id");
            RenameIndex(table: "dbo.TapestryDesigner_Connections", name: "IX_WorkflowRuleId", newName: "IX_TapestryDesignerWorkflowRule_Id");
            RenameColumn(table: "dbo.TapestryDesigner_Connections", name: "WorkflowRuleId", newName: "TapestryDesignerWorkflowRule_Id");
            RenameColumn(table: "dbo.TapestryDesigner_Connections", name: "ResourceRuleId", newName: "TapestryDesignerResourceRule_Id");
            CreateIndex("dbo.TapestryDesigner_Operators", "ParentRule_Id");
            CreateIndex("dbo.TapestryDesigner_Properties", "TapestryDesignerItem_Id");
            CreateIndex("dbo.TapestryDesigner_Items", "ParentRule_Id");
            CreateIndex("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id");
            AddForeignKey("dbo.TapestryDesigner_Rules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id");
            AddForeignKey("dbo.TapestryDesigner_Operators", "ParentRule_Id", "dbo.TapestryDesigner_Rules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Properties", "TapestryDesignerItem_Id", "dbo.TapestryDesigner_Items", "Id");
            AddForeignKey("dbo.TapestryDesigner_Items", "ParentRule_Id", "dbo.TapestryDesigner_Rules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Connections", "TapestryDesignerRule_Id", "dbo.TapestryDesigner_Rules", "Id");
        }
    }
}
