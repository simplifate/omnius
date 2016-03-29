namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class identityWorkflow : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tapestry_WorkFlow", "Id", c => c.Int(nullable: false));
        }
    }
}
