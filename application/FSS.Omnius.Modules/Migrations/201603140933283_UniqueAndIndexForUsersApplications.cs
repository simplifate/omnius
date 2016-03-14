namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueAndIndexForUsersApplications : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Master_UsersApplications", new[] { "UserId" });
            DropIndex("dbo.Master_UsersApplications", new[] { "ApplicationId" });
            CreateIndex("dbo.Master_UsersApplications", new[] { "UserId", "ApplicationId" }, unique: true, name: "IX_userApp");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Master_UsersApplications", "IX_userApp");
            CreateIndex("dbo.Master_UsersApplications", "ApplicationId");
            CreateIndex("dbo.Master_UsersApplications", "UserId");
        }
    }
}
