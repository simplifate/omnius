namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addconditionsetstogateways : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "SymbolType", c => c.String());
            AlterColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerWorkflowItem_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            AlterColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int(nullable: false));
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "SymbolType");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: true);
        }
    }
}
