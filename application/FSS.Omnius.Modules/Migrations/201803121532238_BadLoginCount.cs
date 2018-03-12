namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BadLoginCount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Persona_BadLoginCount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IP = c.String(maxLength: 60),
                        AttemptsCount = c.Int(nullable: false),
                        LastAtempt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Persona_BadLoginCount");
        }
    }
}
