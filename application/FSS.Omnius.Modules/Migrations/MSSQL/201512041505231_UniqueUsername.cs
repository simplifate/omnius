namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueUsername : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Persona_Users", "UserName", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Persona_Users", new[] { "UserName" });
        }
    }
}
