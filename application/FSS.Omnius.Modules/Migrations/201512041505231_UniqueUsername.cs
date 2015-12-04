namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueUsername : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Persona_Users", "username", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Persona_Users", new[] { "username" });
        }
    }
}
