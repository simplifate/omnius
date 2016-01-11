namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActionWithoutIdentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tapestry_Actions", "Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tapestry_Actions", "Id", c => c.Int(nullable: false, identity: true));
        }
    }
}
