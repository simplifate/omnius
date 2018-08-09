namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CortexTask : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Master_UsersApplications", "IX_userApp");
            CreateTable(
                "dbo.Cortex_Task",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        Name = c.String(nullable: false, maxLength: 255),
                        Type = c.Int(nullable: false),
                        Url = c.String(nullable: false, maxLength: 400),
                        Minute_Repeat = c.Int(),
                        Hourly_Repeat = c.Int(),
                        Daily_Repeat = c.Int(),
                        Weekly_Repeat = c.Int(),
                        Weekly_Days = c.Int(),
                        Monthly_Months = c.Int(),
                        Monthly_Days = c.Int(),
                        Monthly_In_Modifiers = c.Int(),
                        Monthly_In_Days = c.Int(),
                        Idle_Time = c.Int(),
                        Start_Time = c.Time(nullable: false, precision: 7),
                        End_Time = c.Time(precision: 7),
                        Duration = c.Time(precision: 7),
                        Start_Date = c.DateTime(nullable: false, storeType: "date"),
                        End_Date = c.DateTime(storeType: "date"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.AppId)
                .Index(t => t.AppId);
            
            CreateIndex("dbo.Master_UsersApplications", "UserId");
            CreateIndex("dbo.Master_UsersApplications", "ApplicationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cortex_Task", "AppId", "dbo.Master_Applications");
            DropIndex("dbo.Cortex_Task", new[] { "AppId" });
            DropIndex("dbo.Master_UsersApplications", new[] { "ApplicationId" });
            DropIndex("dbo.Master_UsersApplications", new[] { "UserId" });
            DropTable("dbo.Cortex_Task");
            CreateIndex("dbo.Master_UsersApplications", new[] { "UserId", "ApplicationId" }, unique: true, name: "IX_userApp");
        }
    }
}
