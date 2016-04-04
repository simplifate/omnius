namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class targetRelation : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TapestryDesigner_WorkflowItems", "TargetId");
            AddForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_WorkflowItems", "TargetId", "dbo.TapestryDesigner_Blocks");
            DropIndex("dbo.TapestryDesigner_WorkflowItems", new[] { "TargetId" });
        }
    }
}
