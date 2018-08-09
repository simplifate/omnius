namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeletedBySynccolumntoPersona_Users : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "DeletedBySync", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "DeletedBySync");
        }
    }
}
