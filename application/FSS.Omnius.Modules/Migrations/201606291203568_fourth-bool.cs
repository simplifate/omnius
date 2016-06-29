namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourthbool : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "MenuChangedSinceLastBuild", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "MenuChangedSinceLastBuild");
        }
    }
}
