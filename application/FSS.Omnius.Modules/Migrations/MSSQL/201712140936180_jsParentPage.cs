namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jsParentPage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mozaic_Js", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropIndex("dbo.Mozaic_Js", new[] { "ApplicationId" });
            DropIndex("dbo.Mozaic_Js", new[] { "MozaicBootstrapPageId" });
            AlterColumn("dbo.Mozaic_Js", "MozaicBootstrapPageId", c => c.Int(nullable: false));
            CreateIndex("dbo.Mozaic_Js", "MozaicBootstrapPageId");
            AddForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page", "Id", cascadeDelete: true);
            DropColumn("dbo.Mozaic_Js", "ApplicationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mozaic_Js", "ApplicationId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropIndex("dbo.Mozaic_Js", new[] { "MozaicBootstrapPageId" });
            AlterColumn("dbo.Mozaic_Js", "MozaicBootstrapPageId", c => c.Int());
            CreateIndex("dbo.Mozaic_Js", "MozaicBootstrapPageId");
            CreateIndex("dbo.Mozaic_Js", "ApplicationId");
            AddForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page", "Id");
            AddForeignKey("dbo.Mozaic_Js", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
