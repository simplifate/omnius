namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Watchtower_log : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Watchtower_LogItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        LogEventType = c.Int(nullable: false),
                        LogLevel = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        IsPlatformEvent = c.Boolean(nullable: false),
                        AppId = c.Int(),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Watchtower_LogItems");
        }
    }
}
