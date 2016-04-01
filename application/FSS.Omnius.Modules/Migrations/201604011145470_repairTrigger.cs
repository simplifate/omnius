namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repairTrigger : DbMigration
    {
        public override void Up()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN " +
                "DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[TapestryDesigner_MetaBlocks] SET [ParentMetaBlock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Persona_ADgroups] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Entitron___META] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted); UPDATE [dbo].[TapestryDesigner_MetaBlocks] SET [ParentMetaBlock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
        }
    }
}
