namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class states : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "States", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "States");
        }
    }
}
