namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestryrolewhitelist : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_BlocksCommits", "RoleWhitelist", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_BlocksCommits", "RoleWhitelist");
        }
    }
}
