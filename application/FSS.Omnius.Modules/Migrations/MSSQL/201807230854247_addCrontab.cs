namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCrontab : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cortex_CrontabTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Schedule = c.String(nullable: false, maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ScheduleStart = c.Int(nullable: false),
                        BlockName = c.String(),
                        Executor = c.String(),
                        ModelId = c.Int(),
                        ApplicationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cortex_CrontabTasks", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Cortex_CrontabTasks", new[] { "ApplicationId" });
            DropTable("dbo.Cortex_CrontabTasks");
        }
    }
}
