namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TargetIdAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "TargetId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "TargetId");
        }
    }
}
