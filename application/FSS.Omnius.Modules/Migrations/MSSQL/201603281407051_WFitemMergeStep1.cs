namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WFitemMergeStep1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "Condition", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "Condition");
        }
    }
}
