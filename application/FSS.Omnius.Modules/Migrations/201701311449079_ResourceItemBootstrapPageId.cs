namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceItemBootstrapPageId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId");
            AddForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "dbo.MozaicBootstrap_Page", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropIndex("dbo.TapestryDesigner_ResourceItems", new[] { "BootstrapPageId" });
            DropColumn("dbo.TapestryDesigner_ResourceItems", "BootstrapPageId");
        }
    }
}
