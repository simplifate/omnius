namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RabbbitMqMove : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RabbitMQs", newName: "Nexus_RabbitMQ");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Nexus_RabbitMQ", newName: "RabbitMQs");
        }
    }
}
