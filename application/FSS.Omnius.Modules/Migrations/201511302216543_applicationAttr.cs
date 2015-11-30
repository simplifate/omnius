namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applicationAttr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "ShowInAppManager", c => c.Boolean(nullable: false));
            AddColumn("dbo.Master_Applications", "DisplayName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "DisplayName");
            DropColumn("dbo.Master_Applications", "ShowInAppManager");
        }
    }
}
