namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryblockisInMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_WorkFlow", "IsInMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Tapestry_Blocks", "IsInMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_Blocks", "IsInMenu");
            DropColumn("dbo.Tapestry_WorkFlow", "IsInMenu");
        }
    }
}
