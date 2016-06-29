namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedsincelastsavebools : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "TapestryChangedSinceLastBuild", c => c.Boolean(nullable: false));
            AddColumn("dbo.Master_Applications", "MozaicChangedSinceLastBuild", c => c.Boolean(nullable: false));
            AddColumn("dbo.Master_Applications", "EntitronChangedSinceLastBuild", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "EntitronChangedSinceLastBuild");
            DropColumn("dbo.Master_Applications", "MozaicChangedSinceLastBuild");
            DropColumn("dbo.Master_Applications", "TapestryChangedSinceLastBuild");
        }
    }
}
