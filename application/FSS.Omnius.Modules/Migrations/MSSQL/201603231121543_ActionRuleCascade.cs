namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActionRuleCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications");
            AddForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: false);
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id", cascadeDelete: false);
            Sql("CREATE TRIGGER [dbo].[Trigger_Application_WorkFlow_Cascade] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkFlow_Blocks_Cascade] ON [dbo].[Tapestry_WorkFlow] INSTEAD OF DELETE AS BEGIN UPDATE [dbo].[Tapestry_WorkFlow] SET [InitBlockId] = NULL, [ParentId] = NULL WHERE [Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Tapestry_Blocks] WHERE [WorkFlowId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_BlockTargetTo] ON [dbo].[Tapestry_Blocks] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_ActionRule] WHERE [TargetBlockId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Tapestry_Blocks] WHERE Id IN (SELECT Id FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_Application_WorkFlow_Cascade];");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkFlow_Blocks_Cascade];");
            Sql("DROP TRIGGER [dbo].[Trigger_BlockTargetTo];");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow");
            AddForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlow", "Id", cascadeDelete: true);
            DropForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications");
            AddForeignKey("dbo.Tapestry_WorkFlow", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
