namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rabbitmq3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RabbitMQs", "UserName", c => c.String());
            AddColumn("dbo.RabbitMQs", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RabbitMQs", "Password");
            DropColumn("dbo.RabbitMQs", "UserName");
        }
    }
}
