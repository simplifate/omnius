namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class greatherMappingLimit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tapestry_ActionRule_Action", "InputVariablesMapping", c => c.String(maxLength: 2000));
            AlterColumn("dbo.Tapestry_ActionRule_Action", "OutputVariablesMapping", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tapestry_ActionRule_Action", "OutputVariablesMapping", c => c.String(maxLength: 200));
            AlterColumn("dbo.Tapestry_ActionRule_Action", "InputVariablesMapping", c => c.String(maxLength: 200));
        }
    }
}
