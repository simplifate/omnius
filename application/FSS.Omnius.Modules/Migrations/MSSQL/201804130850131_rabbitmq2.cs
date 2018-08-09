namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rabbitmq2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RabbitMQs", "Port", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RabbitMQs", "Port");
        }
    }
}
