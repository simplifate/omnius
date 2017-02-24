namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsSharedTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "IsShared", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ResourceItems", "IsShared");
        }
    }
}
