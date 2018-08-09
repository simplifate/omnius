namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermesemailplaintemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Email_Template_Content", "Content_Plain", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Email_Template_Content", "Content_Plain");
        }
    }
}
