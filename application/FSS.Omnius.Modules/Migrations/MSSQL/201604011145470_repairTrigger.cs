namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repairTrigger : DbMigration
    {
        public override void Up()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetablockANDWorkFlow];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetablockANDWorkFlow_CASCADE] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN " +
                "DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[TapestryDesigner_Metablocks] SET [ParentMetablock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[TapestryDesigner_Metablocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Persona_ADgroups] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Entitron___META] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetablockANDWorkFlow_CASCADE];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetablockANDWorkFlow] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted); UPDATE [dbo].[TapestryDesigner_Metablocks] SET [ParentMetablock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_Metablocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
        }
    }
}
