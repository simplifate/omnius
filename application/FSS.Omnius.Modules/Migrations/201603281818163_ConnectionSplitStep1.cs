namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectionSplitStep1 : DbMigration
    {
        public override void Up()
        {
            // cascade with triggers
            DropForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.Master_Applications");
            AddForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.Master_Applications", "Id", cascadeDelete: false);
            Sql("DROP TRIGGER [dbo].[Trigger_Application_WorkFlow_Cascade];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted); UPDATE [dbo].[TapestryDesigner_MetaBlocks] SET [ParentMetaBlock_Id] = NULL WHERE [ParentApp_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE [ParentApp_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");

            DropForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks");
            AddForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks", "Id", cascadeDelete: false);
            Sql("CREATE TRIGGER [dbo].[Trigger_MetaBlockToBlocks] ON [dbo].[TapestryDesigner_MetaBlocks] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_Blocks] WHERE [ParentMetaBlock_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_MetaBlocks] WHERE Id IN (SELECT Id FROM deleted); END");

            DropForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks");
            AddForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks", "Id");
            Sql("CREATE TRIGGER [dbo].[Trigger_BlockToBlocksCommit] ON [dbo].[TapestryDesigner_Blocks] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_BlocksCommits] WHERE [ParentBlock_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_Blocks] WHERE Id IN (SELECT Id FROM deleted); END");

            DropForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            AddForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id");
            DropForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            AddForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id");
            Sql("CREATE TRIGGER [dbo].[Trigger_BlockCommitToRules] ON [dbo].[TapestryDesigner_BlocksCommits] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceRules] WHERE [ParentBlockCommit_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowRules] WHERE [ParentBlockCommit_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_BlocksCommits] WHERE Id IN (SELECT Id FROM deleted); END");

                DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_Connections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "WorkflowRuleId" });
            DropIndex("dbo.TapestryDesigner_Connections", new[] { "ResourceRuleId" });
            CreateIndex("dbo.TapestryDesigner_ResourceConnections", "SourceId");
            CreateIndex("dbo.TapestryDesigner_ResourceConnections", "TargetId");
            CreateIndex("dbo.TapestryDesigner_WorkflowConnections", "SourceId");
            CreateIndex("dbo.TapestryDesigner_WorkflowConnections", "TargetId");
            AddForeignKey("dbo.TapestryDesigner_ResourceConnections", "SourceId", "dbo.TapestryDesigner_ResourceItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ResourceConnections", "TargetId", "dbo.TapestryDesigner_ResourceItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowConnections", "SourceId", "dbo.TapestryDesigner_WorkflowItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowConnections", "TargetId", "dbo.TapestryDesigner_WorkflowItems", "Id");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceRulesToConnection] ON [dbo].[TapestryDesigner_ResourceRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE [ResourceRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceRules] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowRulesToConnection] ON [dbo].[TapestryDesigner_WorkflowRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE [WorkflowRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowRules] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget] ON [dbo].[TapestryDesigner_ResourceConnections] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceItems] WHERE [Id] IN (SELECT SourceId FROM deleted) OR [Id] IN (SELECT TargetId FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget] ON [dbo].[TapestryDesigner_WorkflowConnections] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowItems] WHERE [Id] IN (SELECT SourceId FROM deleted) OR [Id] IN (SELECT TargetId FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE Id IN (SELECT Id FROM deleted); END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[Trigger_ResourceRulesToConnection];");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowRulesToConnection];");
            Sql("DROP TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget];");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget];");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "TargetId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_WorkflowConnections", "SourceId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "TargetId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceConnections", "SourceId", "dbo.TapestryDesigner_ResourceItems");
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "TargetId" });
            DropIndex("dbo.TapestryDesigner_WorkflowConnections", new[] { "SourceId" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "TargetId" });
            DropIndex("dbo.TapestryDesigner_ResourceConnections", new[] { "SourceId" });
            CreateIndex("dbo.TapestryDesigner_Connections", "ResourceRuleId");
            CreateIndex("dbo.TapestryDesigner_Connections", "WorkflowRuleId");
            AddForeignKey("dbo.TapestryDesigner_Connections", "WorkflowRuleId", "dbo.TapestryDesigner_WorkflowRules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Connections", "ResourceRuleId", "dbo.TapestryDesigner_ResourceRules", "Id");

            Sql("DROP TRIGGER [dbo].[Trigger_BlockCommitToRules];");
            DropForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            AddForeignKey("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits");
            AddForeignKey("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", "dbo.TapestryDesigner_BlocksCommits", "Id", cascadeDelete: true);

            Sql("DROP TRIGGER [dbo].[Trigger_BlockToBlocksCommit];");
            DropForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks");
            AddForeignKey("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", "dbo.TapestryDesigner_Blocks", "Id", cascadeDelete: true);

            Sql("DROP TRIGGER [dbo].[Trigger_MetaBlockToBlocks];");
            DropForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks");
            AddForeignKey("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id", "dbo.TapestryDesigner_MetaBlocks", "Id", cascadeDelete: true);

            Sql("DROP TRIGGER [dbo].[Trigger_ApplicationsToMetaBlockANDWorkFlow];");
            Sql("CREATE TRIGGER [dbo].[Trigger_Application_WorkFlow_Cascade] ON [dbo].[Master_Applications] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[Tapestry_WorkFlow] WHERE [ApplicationId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[Master_Applications] WHERE Id IN (SELECT Id FROM deleted); END");
            DropForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.Master_Applications");
            AddForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
