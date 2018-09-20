namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WfItemBlockCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            Sql("ALTER TABLE [dbo].[TapestryDesigner_WorkflowItems]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TapestryDesigner_WorkflowItems_dbo.TapestryDesigner_Blocks_TargetId] FOREIGN KEY([TargetId]) " +
                "REFERENCES[dbo].[TapestryDesigner_Blocks]([Id]) ON DELETE SET NULL;");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks", "Id");
        }
    }
}
