namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "LastIp", c => c.String(maxLength: 50));
            AddColumn("dbo.Persona_Users", "LastAppCookie", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "LastAppCookie");
            DropColumn("dbo.Persona_Users", "LastIp");
        }
    }
}
