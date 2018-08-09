namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAjaxToWFItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "isAjaxAction", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "isAjaxAction");
        }
    }
}
