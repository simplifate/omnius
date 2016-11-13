namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsChangedParameterAddedToBlock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Blocks", "IsChanged", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "IsChanged");
        }
    }
}
