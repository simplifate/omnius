namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Merge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_ModuleAccessPermissions", "Babylon", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_ModuleAccessPermissions", "Babylon");
        }
    }
}
