namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Hermesemaillog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Email_Log",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Hermes_Email_Log");
        }
    }
}
