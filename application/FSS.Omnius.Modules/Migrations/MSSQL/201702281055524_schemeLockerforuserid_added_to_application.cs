namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schemeLockerforuserid_added_to_application : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "SchemeLockedForUserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "SchemeLockedForUserId");
        }
    }
}
