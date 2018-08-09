namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rabbitmq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RabbitMQs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        HostName = c.String(nullable: false),
                        QueueName = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        BlockName = c.String(nullable: false),
                        WorkflowName = c.String(nullable: false),
                        ApplicationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RabbitMQs", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.RabbitMQs", new[] { "ApplicationId" });
            DropTable("dbo.RabbitMQs");
        }
    }
}
