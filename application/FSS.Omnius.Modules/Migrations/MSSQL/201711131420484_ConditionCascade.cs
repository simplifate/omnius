namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConditionCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems");
            Sql("ALTER TABLE [dbo].[TapestryDesigner_ConditionGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TapestryDesigner_ConditionGroups_dbo.TapestryDesigner_ResourceItems_TapestryDesignerResourceItemId] FOREIGN KEY([TapestryDesignerResourceItemId]) " +
                "REFERENCES[dbo].[TapestryDesigner_ResourceItems]([Id]) ON DELETE SET NULL;");
            Sql("ALTER TABLE [dbo].[TapestryDesigner_ConditionGroups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TapestryDesigner_ConditionGroups_dbo.TapestryDesigner_WorkflowItems_TapestryDesignerWorkflowItemId] FOREIGN KEY([TapestryDesignerWorkflowItemId]) " +
                "REFERENCES[dbo].[TapestryDesigner_WorkflowItems]([Id]) ON DELETE SET NULL;");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems");
            AddForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems", "Id");
        }
    }
}
