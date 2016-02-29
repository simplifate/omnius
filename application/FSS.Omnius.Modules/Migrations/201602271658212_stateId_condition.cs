namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stateId_condition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "StateId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentId", c => c.String());
            AddColumn("dbo.TapestryDesigner_WorkflowSymbols", "Condition", c => c.String());
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "States");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "States", c => c.String());
            DropColumn("dbo.TapestryDesigner_WorkflowSymbols", "Condition");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentId");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "StateId");
        }
    }
}
