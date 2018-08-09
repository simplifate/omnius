namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestrydesignerforeignkeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            AlterColumn("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id");
            CreateIndex("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TapestryDesigner_Swimlanes", new[] { "ParentWorkflowRule_Id" });
            DropIndex("dbo.TapestryDesigner_WorkflowRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_ResourceRules", new[] { "ParentBlockCommit_Id" });
            DropIndex("dbo.TapestryDesigner_BlocksCommits", new[] { "ParentBlock_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            AlterColumn("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_Swimlanes", "ParentWorkflowRule_Id");
            CreateIndex("dbo.TapestryDesigner_WorkflowRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_ResourceRules", "ParentBlockCommit_Id");
            CreateIndex("dbo.TapestryDesigner_BlocksCommits", "ParentBlock_Id");
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id");
        }
    }
}
