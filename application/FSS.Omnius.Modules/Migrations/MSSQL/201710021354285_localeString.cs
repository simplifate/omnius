namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class localeString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "Locale", c => c.String(maxLength: 2));
            Sql("UPDATE dbo.Persona_Users SET Locale = IIF(LocaleId = 2, 'en', 'cs');");
            DropColumn("dbo.Persona_Users", "LocaleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Persona_Users", "LocaleId", c => c.Int());
            Sql("UPDATE dbo.Persona_Users SET LocaleId = IIF(Locale = 'en', 2, 1);");
            DropColumn("dbo.Persona_Users", "Locale");
        }
    }
}
