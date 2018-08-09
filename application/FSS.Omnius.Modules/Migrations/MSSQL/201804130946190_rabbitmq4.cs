namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rabbitmq4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RabbitMQs", new[] { "ApplicationId" });
            AlterColumn("dbo.RabbitMQs", "BlockName", c => c.String());
            AlterColumn("dbo.RabbitMQs", "WorkflowName", c => c.String());
            AlterColumn("dbo.RabbitMQs", "ApplicationId", c => c.Int());
            CreateIndex("dbo.RabbitMQs", "ApplicationId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RabbitMQs", new[] { "ApplicationId" });
            AlterColumn("dbo.RabbitMQs", "ApplicationId", c => c.Int(nullable: false));
            AlterColumn("dbo.RabbitMQs", "WorkflowName", c => c.String(nullable: false));
            AlterColumn("dbo.RabbitMQs", "BlockName", c => c.String(nullable: false));
            CreateIndex("dbo.RabbitMQs", "ApplicationId");
        }
    }
}
