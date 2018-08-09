namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncomingEmailRuleName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Incoming_Email_Rule", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Hermes_Incoming_Email_Rule", "BlockName", c => c.String(nullable: false));
            AlterColumn("dbo.Hermes_Incoming_Email_Rule", "WorkflowName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hermes_Incoming_Email_Rule", "WorkflowName", c => c.String());
            AlterColumn("dbo.Hermes_Incoming_Email_Rule", "BlockName", c => c.String());
            DropColumn("dbo.Hermes_Incoming_Email_Rule", "Name");
        }
    }
}
