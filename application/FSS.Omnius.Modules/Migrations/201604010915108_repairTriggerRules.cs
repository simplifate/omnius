namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repairTriggerRules : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "FK_dbo.TapestryDesigner_ResourceItems_dbo.TapestryDesigner_ResourceRules_ParentRule_Id");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: false);

            Sql("DROP TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget] ON [dbo].[TapestryDesigner_ResourceItems] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE [SourceId] IN (SELECT Id FROM deleted) OR [TargetId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceItems] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_ResourceRulesToConnection];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceRulesToConnection] ON [dbo].[TapestryDesigner_ResourceRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE [ResourceRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceItems] WHERE [ParentRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceRules] WHERE Id IN (SELECT Id FROM deleted); END");


            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "FK_dbo.TapestryDesigner_WorkflowItems_dbo.TapestryDesigner_Swimlanes_ParentSwimlane_Id");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes", "Id", cascadeDelete: false);
            DropForeignKey("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules");
            AddForeignKey("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", "dbo.TapestryDesigner_WorkflowRules", "Id", cascadeDelete: false);

            Sql("CREATE TRIGGER [dbo].[Trigger_SwimlaneWFItems_CASCADE] ON [dbo].[TapestryDesigner_Swimlanes] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowItems] WHERE [ParentSwimlaneId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_Swimlanes] WHERE [Id] IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget];");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget_CASCADE] ON [dbo].[TapestryDesigner_WorkflowItems] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE [SourceId] IN (SELECT Id FROM deleted) OR [TargetId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowItems] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowRulesToConnection];");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowRulesToConnectionSwimlane_CASCADE] ON [dbo].[TapestryDesigner_WorkflowRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE [WorkflowRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_Swimlanes] WHERE [ParentWorkflowRule_Id] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowRules] WHERE Id IN (SELECT Id FROM deleted); END");

            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            AddForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            AddForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: false);

            Sql("DROP TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceConnectionSourceTarget] ON [dbo].[TapestryDesigner_ResourceConnections] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceItems] WHERE [Id] IN (SELECT SourceId FROM deleted) OR [Id] IN (SELECT TargetId FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget_CASCADE];");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowConnectionSourceTarget] ON [dbo].[TapestryDesigner_WorkflowConnections] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowItems] WHERE [Id] IN (SELECT SourceId FROM deleted) OR [Id] IN (SELECT TargetId FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_ResourceRulesToConnection];");
            Sql("CREATE TRIGGER [dbo].[Trigger_ResourceRulesToConnection] ON [dbo].[TapestryDesigner_ResourceRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_ResourceConnections] WHERE [ResourceRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_ResourceRules] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_WorkflowRulesToConnection];");
            Sql("CREATE TRIGGER [dbo].[Trigger_WorkflowRulesToConnection] ON [dbo].[TapestryDesigner_WorkflowRules] INSTEAD OF DELETE AS BEGIN DELETE FROM [dbo].[TapestryDesigner_WorkflowConnections] WHERE [WorkflowRuleId] IN (SELECT Id FROM deleted); DELETE FROM [dbo].[TapestryDesigner_WorkflowRules] WHERE Id IN (SELECT Id FROM deleted); END");
            Sql("DROP TRIGGER [dbo].[Trigger_SwimlaneWFItems_CASCADE]");

            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: true);
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentRuleId", "dbo.TapestryDesigner_WorkflowRules");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentRuleId", "dbo.TapestryDesigner_WorkflowRules", "Id", cascadeDelete: true);
        }
    }
}
