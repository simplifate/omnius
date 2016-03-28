namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blockTemp : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tapestry_Blocks", new[] { "Name" });
            AddColumn("dbo.Tapestry_Blocks", "IsTemp", c => c.Boolean(nullable: false, defaultValue: false));
            CreateIndex("dbo.Tapestry_Blocks", new[] { "Name", "IsTemp" }, unique: true, name: "blockUniqueness");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tapestry_Blocks", "blockUniqueness");
            DropColumn("dbo.Tapestry_Blocks", "IsTemp");
            CreateIndex("dbo.Tapestry_Blocks", "Name", unique: true);
        }
    }
}
