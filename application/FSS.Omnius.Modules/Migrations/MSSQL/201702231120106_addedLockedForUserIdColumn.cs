namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLockedForUserIdColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Blocks", "LockedForUserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "LockedForUserId");
        }
    }
}
