namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlockDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "DisplayName", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Tapestry_Blocks", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tapestry_Blocks", new[] { "Name" });
            DropColumn("dbo.Tapestry_Blocks", "DisplayName");
        }
    }
}
