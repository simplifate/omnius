namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascadeDelete2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications");
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "Hermes_Email_Template_Id" });
            DropIndex("dbo.Hermes_Email_Template", new[] { "AppId" });
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Hermes_Email_Template_Id" });
            AlterColumn("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Hermes_Email_Template", "AppId", c => c.Int(nullable: false));
            AlterColumn("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Hermes_Email_Template", "AppId");
            CreateIndex("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id");
            CreateIndex("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id");
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id", cascadeDelete: true);
            Sql("ALTER TABLE dbo.TapestryDesigner_ConditionSets ADD CONSTRAINT [FK_dbo.TapestryDesigner_ConditionSets_dbo.TapestryDesigner_ResourceItems_TapestryDesignerResourceItem_Id] FOREIGN KEY (TapestryDesignerResourceItem_Id) REFERENCES dbo.TapestryDesigner_ResourceItems (Id) ON DELETE SET NULL;");
            Sql("ALTER TABLE dbo.TapestryDesigner_ConditionSets ADD CONSTRAINT [FK_dbo.TapestryDesigner_ConditionSets_dbo.TapestryDesigner_WorkflowItems_TapestryDesignerWorkflowItem_Id] FOREIGN KEY (TapestryDesignerWorkflowItem_Id) REFERENCES dbo.TapestryDesigner_WorkflowItems (Id) ON DELETE SET NULL;");
            Sql("ALTER TABLE dbo.TapestryDesigner_WorkflowItems ADD CONSTRAINT [FK_dbo.TapestryDesigner_WorkflowItems_dbo.TapestryDesigner_Blocks_TargetId] FOREIGN KEY (TargetId) REFERENCES dbo.TapestryDesigner_Blocks (Id) ON DELETE SET NULL;");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            DropForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications");
            DropForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template");
            DropIndex("dbo.Hermes_Email_Placeholder", new[] { "Hermes_Email_Template_Id" });
            DropIndex("dbo.Hermes_Email_Template_Content", new[] { "Hermes_Email_Template_Id" });
            DropIndex("dbo.Hermes_Email_Template", new[] { "AppId" });
            AlterColumn("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", c => c.Int());
            AlterColumn("dbo.Hermes_Email_Template", "AppId", c => c.Int());
            AlterColumn("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", c => c.Int());
            CreateIndex("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id");
            CreateIndex("dbo.Hermes_Email_Template", "AppId");
            CreateIndex("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id");
            AddForeignKey("dbo.Hermes_Email_Placeholder", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
            AddForeignKey("dbo.Hermes_Email_Template", "AppId", "dbo.Master_Applications", "Id");
            AddForeignKey("dbo.Hermes_Email_Template_Content", "Hermes_Email_Template_Id", "dbo.Hermes_Email_Template", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks", "Id");
        }
    }
}
