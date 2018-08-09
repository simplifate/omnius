namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryModelsBootstrapPageSupport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "IsBootstrap", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "IsBootstrap", c => c.Boolean());
            AddColumn("dbo.TapestryDesigner_BlocksCommits", "AssociatedBootstrapPageIds", c => c.String());
            AddColumn("dbo.TapestryDesigner_ToolboxItems", "IsBootstrap", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ToolboxItems", "IsBootstrap");
            DropColumn("dbo.TapestryDesigner_BlocksCommits", "AssociatedBootstrapPageIds");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "IsBootstrap");
            DropColumn("dbo.TapestryDesigner_ResourceItems", "IsBootstrap");
        }
    }
}
