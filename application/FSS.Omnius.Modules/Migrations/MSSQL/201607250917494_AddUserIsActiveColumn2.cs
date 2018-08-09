namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIsActiveColumn2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "isActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "isActive");
        }
    }
}
