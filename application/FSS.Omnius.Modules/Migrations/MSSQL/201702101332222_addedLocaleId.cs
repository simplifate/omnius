namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLocaleId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "LocaleId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "LocaleId");
        }
    }
}
