namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddmigrationforassigningapplicationrolesbyADgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_ADgroups", "RoleForApplication", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_ADgroups", "RoleForApplication");
        }
    }
}
