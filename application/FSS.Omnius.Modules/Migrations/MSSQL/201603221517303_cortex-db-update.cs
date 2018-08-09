namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cortexdbupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Cortex_Task", "Monthly_Days", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Cortex_Task", "Monthly_Days", c => c.Int());
        }
    }
}
