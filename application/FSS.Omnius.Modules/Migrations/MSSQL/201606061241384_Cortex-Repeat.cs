namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CortexRepeat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cortex_Task", "Repeat", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cortex_Task", "Repeat_Minute", c => c.Int());
            AddColumn("dbo.Cortex_Task", "Repeat_Duration", c => c.Int());
            DropColumn("dbo.Cortex_Task", "Minute_Repeat");
            DropColumn("dbo.Cortex_Task", "Hourly_Repeat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cortex_Task", "Hourly_Repeat", c => c.Int());
            AddColumn("dbo.Cortex_Task", "Minute_Repeat", c => c.Int());
            DropColumn("dbo.Cortex_Task", "Repeat_Duration");
            DropColumn("dbo.Cortex_Task", "Repeat_Minute");
            DropColumn("dbo.Cortex_Task", "Repeat");
        }
    }
}
