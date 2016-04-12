namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cortexmontlytype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cortex_Task", "Monthly_Type", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cortex_Task", "Monthly_Type");
        }
    }
}
