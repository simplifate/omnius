namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appnameInUserROle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_User_Role", "ApplicationName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_User_Role", "ApplicationName");
        }
    }
}
