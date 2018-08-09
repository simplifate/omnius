namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logout : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "CurrentLogin", c => c.DateTime(nullable: false));
            AddColumn("dbo.Persona_Users", "LastLogout", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "LastLogout");
            DropColumn("dbo.Persona_Users", "CurrentLogin");
        }
    }
}
