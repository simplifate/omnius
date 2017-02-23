namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryWorkflowItemsNameandComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "Name", c => c.String());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "Comment");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "Name");
        }
    }
}
