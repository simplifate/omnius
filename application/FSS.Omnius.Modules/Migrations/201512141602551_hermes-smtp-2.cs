namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hermessmtp2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hermes_Smtp", "Auth_User", c => c.String(maxLength: 255));
            AlterColumn("dbo.Hermes_Smtp", "Auth_Password", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Hermes_Smtp", "Auth_Password", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Hermes_Smtp", "Auth_User", c => c.String(nullable: false, maxLength: 255));
        }
    }
}
