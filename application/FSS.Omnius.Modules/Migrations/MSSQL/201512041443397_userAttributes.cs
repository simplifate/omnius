namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "DisplayName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Persona_Users", "Email", c => c.String(maxLength: 100));
            AddColumn("dbo.Persona_Users", "Company", c => c.String(maxLength: 100));
            AddColumn("dbo.Persona_Users", "Department", c => c.String(maxLength: 100));
            AddColumn("dbo.Persona_Users", "Team", c => c.String(maxLength: 100));
            AddColumn("dbo.Persona_Users", "WorkPhone", c => c.String(maxLength: 20));
            AddColumn("dbo.Persona_Users", "MobilPhone", c => c.String(maxLength: 20));
            AddColumn("dbo.Persona_Users", "Address", c => c.String(maxLength: 500));
            AddColumn("dbo.Persona_Users", "Job", c => c.String(maxLength: 100));
            AddColumn("dbo.Persona_Users", "LastLogin", c => c.DateTime(nullable: false));
            AddColumn("dbo.Persona_Users", "localExpiresAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Persona_Users", "passwordHash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_Users", "passwordHash", c => c.String(nullable: false));
            DropColumn("dbo.Persona_Users", "localExpiresAt");
            DropColumn("dbo.Persona_Users", "LastLogin");
            DropColumn("dbo.Persona_Users", "Job");
            DropColumn("dbo.Persona_Users", "Address");
            DropColumn("dbo.Persona_Users", "MobilPhone");
            DropColumn("dbo.Persona_Users", "WorkPhone");
            DropColumn("dbo.Persona_Users", "Team");
            DropColumn("dbo.Persona_Users", "Department");
            DropColumn("dbo.Persona_Users", "Company");
            DropColumn("dbo.Persona_Users", "Email");
            DropColumn("dbo.Persona_Users", "DisplayName");
        }
    }
}
