namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class renameBootstrap : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MozaicBootstrap_Components", "ParentComponentId", "MozaicBootstrap_Components");
            DropForeignKey("dbo.MozaicBootstrap_Components", "MozaicBootstrapPageId", "MozaicBootstrap_Page");
            DropForeignKey("dbo.MozaicBootstrap_Page", "ParentApp_Id", "Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "MozaicBootstrap_Page");

            RenameTable(name: "dbo.MozaicBootstrap_Page", newName: "MozaicB_Page");
            RenameTable(name: "dbo.MozaicBootstrap_Components", newName: "MozaicB_Components");

            AddForeignKey("dbo.MozaicB_Components", "ParentComponentId", "MozaicB_Components");
            AddForeignKey("dbo.MozaicB_Components", "MozaicBootstrapPageId", "MozaicB_Page");
            AddForeignKey("dbo.MozaicB_Page", "ParentApp_Id", "Master_Applications");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "MozaicB_Page");

            Sql("DROP TRIGGER [TRG_C:Master_Applications]");
            Sql("CREATE TRIGGER [TRG_C:Master_Applications] ON [Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [Persona_AppRoles] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_User_Role] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_ColumnMetadata] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Template] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Incoming_Email_Rule] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Mozaic_Js] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicB_Page] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicEditor_Pages] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_ConditionGroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_UsersApplications] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Queue] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Cortex_Task] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_ADgroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); UPDATE [Persona_Users] SET [DesignAppId] = NULL WHERE [DesignAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_Applications] WHERE [Id] IN (SELECT [Id] FROM deleted); END");

            Sql("DROP TRIGGER [TRG_C:MozaicBootstrap_Components]");
            Sql("CREATE TRIGGER [TRG_C:MozaicB_Components] ON [MozaicB_Components] INSTEAD OF DELETE AS BEGIN DELETE FROM [MozaicB_Components] WHERE [ParentComponentId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicB_Components] WHERE [Id] IN (SELECT [Id] FROM deleted); END");

            Sql("DROP TRIGGER [TRG_C:MozaicBootstrap_Page]");
            Sql("CREATE TRIGGER [TRG_C:MozaicB_Page] ON [MozaicB_Page] INSTEAD OF DELETE AS BEGIN DELETE FROM [MozaicB_Components] WHERE [MozaicBootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [Mozaic_Js] SET [MozaicBootstrapPageId] = NULL WHERE [MozaicBootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [TapestryDesigner_ResourceItems] SET [BootstrapPageId] = NULL WHERE [BootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [Tapestry_Blocks] SET [BootstrapPageId] = NULL WHERE [BootstrapPageId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicB_Page] WHERE [Id] IN (SELECT [Id] FROM deleted); END");
        }

        public override void Down()
        {
            DropForeignKey("dbo.MozaicB_Components", "ParentComponentId", "MozaicB_Components");
            DropForeignKey("dbo.MozaicB_Components", "MozaicBootstrapPageId", "MozaicB_Page");
            DropForeignKey("dbo.MozaicB_Page", "ParentApp_Id", "Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "MozaicB_Page");

            RenameTable(name: "dbo.MozaicB_Components", newName: "MozaicBootstrap_Components");
            RenameTable(name: "dbo.MozaicB_Page", newName: "MozaicBootstrap_Page");

            AddForeignKey("dbo.MozaicBootstrap_Components", "ParentComponentId", "MozaicBootstrap_Components");
            AddForeignKey("dbo.MozaicBootstrap_Components", "MozaicBootstrapPageId", "MozaicBootstrap_Page");
            AddForeignKey("dbo.MozaicBootstrap_Page", "ParentApp_Id", "Master_Applications");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "MozaicBootstrap_Page");

            Sql("DROP TRIGGER [TRG_C:Master_Applications]");
            Sql("CREATE TRIGGER [TRG_C:Master_Applications] ON [Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [Persona_AppRoles] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_User_Role] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_ColumnMetadata] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Template] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Incoming_Email_Rule] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Mozaic_Js] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicBootstrap_Page] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicEditor_Pages] WHERE [ParentApp_Id] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_ConditionGroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_UsersApplications] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Hermes_Email_Queue] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); DELETE FROM [Cortex_Task] WHERE [AppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Persona_ADgroups] WHERE [ApplicationId] IN (SELECT [Id] FROM deleted); UPDATE [Persona_Users] SET [DesignAppId] = NULL WHERE [DesignAppId] IN (SELECT [Id] FROM deleted); DELETE FROM [Master_Applications] WHERE [Id] IN (SELECT [Id] FROM deleted); END");

            Sql("DROP TRIGGER [TRG_C:MozaicB_Components]");
            Sql("CREATE TRIGGER [TRG_C:MozaicBootstrap_Components] ON [MozaicBootstrap_Components] INSTEAD OF DELETE AS BEGIN DELETE FROM [MozaicBootstrap_Components] WHERE [ParentComponentId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicBootstrap_Components] WHERE [Id] IN (SELECT [Id] FROM deleted); END");

            Sql("DROP TRIGGER [TRG_C:MozaicB_Page]");
            Sql("CREATE TRIGGER [TRG_C:MozaicBootstrap_Page] ON [MozaicBootstrap_Page] INSTEAD OF DELETE AS BEGIN DELETE FROM [MozaicBootstrap_Components] WHERE [MozaicBootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [Mozaic_Js] SET [MozaicBootstrapPageId] = NULL WHERE [MozaicBootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [TapestryDesigner_ResourceItems] SET [BootstrapPageId] = NULL WHERE [BootstrapPageId] IN (SELECT [Id] FROM deleted); UPDATE [Tapestry_Blocks] SET [BootstrapPageId] = NULL WHERE [BootstrapPageId] IN (SELECT [Id] FROM deleted); DELETE FROM [MozaicBootstrap_Page] WHERE [Id] IN (SELECT [Id] FROM deleted); END");
        }
    }
}
