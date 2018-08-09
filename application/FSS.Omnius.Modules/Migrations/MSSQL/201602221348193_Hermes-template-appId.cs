namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HermestemplateappId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hermes_Email_Template", "AppId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hermes_Email_Template", "AppId");
        }
    }
}
