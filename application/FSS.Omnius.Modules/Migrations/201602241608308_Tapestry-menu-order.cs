namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tapestrymenuorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Metablocks", "MenuOrder", c => c.Int(nullable: false));
            AddColumn("dbo.TapestryDesigner_Blocks", "MenuOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "MenuOrder");
            DropColumn("dbo.TapestryDesigner_Metablocks", "MenuOrder");
        }
    }
}
