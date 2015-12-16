namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueSeed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CORE_Modules", "Name", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Master_Applications", "Name", unique: true);
            CreateIndex("dbo.Tapestry_WorkFlow_Types", "Name", unique: true);
            CreateIndex("dbo.Tapestry_Actors", "Name", unique: true);
            CreateIndex("dbo.CORE_Modules", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.CORE_Modules", new[] { "Name" });
            DropIndex("dbo.Tapestry_Actors", new[] { "Name" });
            DropIndex("dbo.Tapestry_WorkFlow_Types", new[] { "Name" });
            DropIndex("dbo.Master_Applications", new[] { "Name" });
            AlterColumn("dbo.CORE_Modules", "Name", c => c.String(nullable: false));
        }
    }
}
