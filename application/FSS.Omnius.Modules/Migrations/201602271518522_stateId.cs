namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stateId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "StateId", c => c.Int());
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "States");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "States", c => c.String());
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "StateId");
        }
    }
}
