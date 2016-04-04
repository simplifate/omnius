namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddpageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "PageId", c => c.Int());
            RenameColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentId", "ComponentName");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.TapestryDesigner_WorkflowItems", "ComponentName", "ComponentId");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "PageId");
        }
    }
}
