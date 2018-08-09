namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Hermesqueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Hermes_Email_Queue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Date_Send_After = c.DateTime(nullable: false),
                        Date_Inserted = c.DateTime(nullable: false),
                        Application_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Date_Send_After);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Hermes_Email_Queue", new[] { "Date_Send_After" });
            DropTable("dbo.Hermes_Email_Queue");
        }
    }
}
