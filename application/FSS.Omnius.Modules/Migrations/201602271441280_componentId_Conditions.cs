namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class componentId_Conditions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentId", c => c.String());
            AddColumn("dbo.TapestryDesigner_WorkflowSymbols", "Condition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowSymbols", "Condition");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentId");
        }
    }
}
