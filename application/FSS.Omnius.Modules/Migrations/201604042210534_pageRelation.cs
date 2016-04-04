namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pageRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages");
            DropIndex("dbo.MozaicEditor_Components", new[] { "MozaicEditorPage_Id" });
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "MozaicEditorPage_Id", newName: "MozaicEditorPageId");
            AlterColumn("dbo.MozaicEditor_Components", "MozaicEditorPageId", c => c.Int(nullable: false));
            CreateIndex("dbo.MozaicEditor_Components", "MozaicEditorPageId");
            CreateIndex("dbo.TapestryDesigner_ResourceItems", "PageId");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "PageId", "dbo.MozaicEditor_Pages", "Id");
            AddForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPageId", "dbo.MozaicEditor_Pages", "Id", cascadeDelete: false);

            DropForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications");
            AddForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications", "Id", cascadeDelete: false);
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN " +
                "DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[TapestryDesigner_MetaBlocks] SET [ParentMetaBlock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Persona_ADgroups] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Entitron___META] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[MozaicEditor_Pages] WHERE [ParentApp_Id] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_PageComponent_CASCADE] ON [dbo].[MozaicEditor_Pages] INSTEAD OF DELETE AS BEGIN " +
                "UPDATE [dbo].[MozaicEditor_Components] SET [ParentComponent_Id] = NULL WHERE [MozaicEditorPageId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[MozaicEditor_Components] WHERE [MozaicEditorPageId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[MozaicEditor_Pages] WHERE Id IN (SELECT Id FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_PageComponent_CASCADE];");
            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow_CASCADE] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN " +
                "DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[TapestryDesigner_MetaBlocks] SET [ParentMetaBlock_Id] = NULL WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE [ParentAppId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Entitron_DbSchemeCommit] WHERE [Application_Id] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Persona_ADgroups] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "UPDATE [dbo].[Entitron___META] SET [ApplicationId] = NULL WHERE [ApplicationId] IN (SELECT Id FROM deleted);" +
                "DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
            DropForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications");
            AddForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications", "Id", cascadeDelete: true);

            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPageId", "dbo.MozaicEditor_Pages");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "PageId", "dbo.MozaicEditor_Pages");
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "PageId" });
            DropIndex("dbo.MozaicEditor_Components", new[] { "MozaicEditorPageId" });
            AlterColumn("dbo.MozaicEditor_Components", "MozaicEditorPageId", c => c.Int());
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "MozaicEditorPageId", newName: "MozaicEditorPage_Id");
            CreateIndex("dbo.MozaicEditor_Components", "MozaicEditorPage_Id");
            AddForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages", "Id");
        }
    }
}
