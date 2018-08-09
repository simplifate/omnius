namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Actionvariables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "InputVariables", c => c.String());
            AddColumn("dbo.TapestryDesigner_WorkflowItems", "OutputVariables", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "OutputVariables");
            DropColumn("dbo.TapestryDesigner_WorkflowItems", "InputVariables");
        }
    }
}
