namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conditionApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ParentAppId", c => c.Int());
            Sql("DELETE FROM TapestryDesigner_ConditionSets WHERE TapestryDesignerResourceItem_Id IS NULL AND TapestryDesignerWorkflowItem_Id IS NULL");
            Sql("UPDATE cs SET cs.ParentAppId = mb.ParentAppId FROM TapestryDesigner_ConditionSets cs " +
                "left join TapestryDesigner_ResourceItems ri ON ri.Id = cs.TapestryDesignerResourceItem_Id " +
                "left join TapestryDesigner_ResourceRules rr ON rr.Id = ri.ParentRuleId " +
                "left join TapestryDesigner_WorkflowItems wi ON wi.Id = cs.TapestryDesignerWorkflowItem_Id " +
                "left join TapestryDesigner_Swimlanes s ON s.Id = wi.ParentSwimlaneId " +
                "left join TapestryDesigner_WorkflowRules wr ON wr.Id = s.ParentWorkflowRule_Id " +
                "left join TapestryDesigner_BlocksCommits bc ON bc.Id = rr.ParentBlockCommit_Id OR bc.Id = wr.ParentBlockCommit_Id " +
                "left join TapestryDesigner_Blocks b ON b.Id = bc.ParentBlock_Id " +
                "left join TapestryDesigner_MetaBlocks mb ON mb.Id = b.ParentMetablock_Id");
            AlterColumn("dbo.TapestryDesigner_ConditionSets", "ParentAppId", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ParentAppId");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "ParentAppId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ParentAppId", "dbo.Master_Applications");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ParentAppId" });
            DropColumn("dbo.TapestryDesigner_ConditionSets", "ParentAppId");
        }
    }
}
