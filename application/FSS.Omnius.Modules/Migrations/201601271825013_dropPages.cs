namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropPages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mozaic_Pages", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Pages", "MasterTemplateId", "dbo.Mozaic_Template");
            DropForeignKey("dbo.Tapestry_Blocks", "MozaicPageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages");
            DropIndex("dbo.Mozaic_Pages", new[] { "MasterTemplateId" });
            DropIndex("dbo.Mozaic_Pages", new[] { "ApplicationId" });
            DropTable("dbo.Mozaic_Pages");
            CreateTable("dbo.Mozaic_Pages",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ViewName = c.String(nullable: true, maxLength: 50),
                    ViewPath = c.String(nullable: true, maxLength: 500),
                    ViewContent = c.String(nullable: true),
                    Application_Id = c.Int(),
                    Template_Id = c.Int()
                })
                .PrimaryKey(i => i.Id);
            AddForeignKey("dbo.Tapestry_Blocks", "MozaicPageId", "dbo.Mozaic_Pages", "Id");
            AddForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages", "Id");
        }
        
        public override void Down()
        {
            DropTable("dbo.Mozaic_Pages");
            CreateTable("dbo.Mozaic_Pages",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ViewName = c.String(nullable: true, maxLength: 50),
                    ViewPath = c.String(nullable: true, maxLength: 500),
                    ViewContent = c.String(nullable: true),
                })
                .PrimaryKey(i => i.Id);
            CreateIndex("dbo.Mozaic_Pages", "ApplicationId");
            CreateIndex("dbo.Mozaic_Pages", "MasterTemplateId");
            AddForeignKey("dbo.Mozaic_Pages", "MasterTemplateId", "dbo.Mozaic_Template", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Mozaic_Pages", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
        }
    }
}
