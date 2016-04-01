namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requiredParent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentApp_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "ParentRuleId" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            DropIndex("dbo.TapestryDesigner_Conditions", new[] { "TapestryDesignerConditionSet_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_MetablocksConnections", new[] { "TapestryDesignerMetablock_Id" });
            RenameColumn(table: "dbo.TapestryDesigner_Metablocks", name: "ParentApp_Id", newName: "ParentAppId");
            AlterColumn("dbo.TapestryDesigner_Metablocks", "ParentAppId", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentAppId");
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id");
            CreateIndex("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceItems", "ParentRuleId");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            CreateIndex("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId");
            CreateIndex("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id");
            CreateIndex("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id");
            AddForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", "dbo.TapestryDesigner_Metablocks", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TapestryDesigner_Metablocks", "ParentAppId", "dbo.Master_Applications", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules", "Id", cascadeDelete: false);
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes");
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules");
            DropForeignKey("dbo.TapestryDesigner_Metablocks", "ParentAppId", "dbo.Master_Applications");
            DropForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropIndex("dbo.TapestryDesigner_MetablocksConnections", new[] { "TapestryDesignerMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "ParentSwimlaneId" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Conditions", new[] { "TapestryDesignerConditionSet_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "ParentRuleId" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentAppId" });
            AlterColumn("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Metablocks", "ParentAppId", c => c.Int());
            RenameColumn(table: "dbo.TapestryDesigner_Metablocks", name: "ParentAppId", newName: "ParentApp_Id");
            CreateIndex("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id");
            CreateIndex("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId");
            CreateIndex("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceItems", "ParentRuleId");
            CreateIndex("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id");
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id");
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentApp_Id");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "ParentSwimlaneId", "dbo.TapestryDesigner_Swimlanes", "Id");
            AddForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "ParentRuleId", "dbo.TapestryDesigner_ResourceRules", "Id");
            AddForeignKey("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", "dbo.Master_Applications", "Id");
            AddForeignKey("dbo.TapestryDesigner_MetablocksConnections", "TapestryDesignerMetablock_Id", "dbo.TapestryDesigner_Metablocks", "Id");
        }
    }
}
