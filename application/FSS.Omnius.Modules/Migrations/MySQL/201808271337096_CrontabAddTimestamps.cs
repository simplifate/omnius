namespace FSS.Omnius.Modules.Migrations.MySQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrontabAddTimestamps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cortex_CrontabTasks", "LastStartTask", c => c.DateTime(precision: 0));
            AddColumn("dbo.Cortex_CrontabTasks", "LastEndTask", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cortex_CrontabTasks", "LastEndTask");
            DropColumn("dbo.Cortex_CrontabTasks", "LastStartTask");
        }
    }
}
