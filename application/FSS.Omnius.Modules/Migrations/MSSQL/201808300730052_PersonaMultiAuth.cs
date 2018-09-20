namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonaMultiAuth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "AuthTypeId", c => c.Int(nullable: false));
            Sql("UPDATE [dbo].[Persona_Users] SET [AuthTypeId] = CASE [isLocalUser] WHEN 1 THEN 0 WHEN 0 THEN 2 END");
            DropColumn("dbo.Persona_Users", "isLocalUser");
        }

        public override void Down()
        {
            AddColumn("dbo.Persona_Users", "isLocalUser", c => c.Boolean(nullable: false));
            Sql("UPDATE [dbo].[Persona_Users] SET [isLocalUser] = CASE [AuthTypeId] WHEN 0 THEN 1 WHEN 2 THEN 0 END");
            DropColumn("dbo.Persona_Users", "AuthTypeId");
        }
    }
}
