namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeEntitronMetaCascade : DbMigration
    {
        public override void Up()
        {
            Sql("DROP TRIGGER [TRG_C:Master_Applications]");
            Sql("CREATE TRIGGER [TRG_C:Master_Applications] ON [Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [Persona_AppRoles] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_ColumnMetadata] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_DbSchemeCommit] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Template] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Incoming_Email_Rule] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicBootstrap_Page] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicEditor_Pages] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_ConditionSets] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_UsersApplications] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Queue] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Cortex_Task] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_ADgroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); UPDATE [Persona_Users] SET [DesignAppId] = NULL WHERE [DesignAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_Applications] WHERE [Id] IN (SELECT [Id] FROM deleted); END");
        }

        public override void Down()
        {
            Sql("DROP TRIGGER [TRG_C:Master_Applications]");
            Sql("CREATE TRIGGER [TRG_C:Master_Applications] ON [Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [Entitron___META] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_AppRoles] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_ColumnMetadata] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_DbSchemeCommit] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Template] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Incoming_Email_Rule] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicBootstrap_Page] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicEditor_Pages] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_ConditionSets] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_UsersApplications] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Queue] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Cortex_Task] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_ADgroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); UPDATE [Persona_Users] SET [DesignAppId] = NULL WHERE [DesignAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_Applications] WHERE [Id] IN (SELECT [Id] FROM deleted); END");
        }
    }
}
