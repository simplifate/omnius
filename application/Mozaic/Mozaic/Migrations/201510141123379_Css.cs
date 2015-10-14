namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Css : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mozaic_Css",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_CssPages",
                c => new
                    {
                        PageId = c.Int(nullable: false),
                        CssId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PageId, t.CssId })
                .ForeignKey("dbo.Mozaic_Pages", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.Mozaic_Css", t => t.CssId, cascadeDelete: true)
                .Index(t => t.PageId)
                .Index(t => t.CssId);
            
            AddColumn("dbo.Mozaic_Pages", "ApplicationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Mozaic_Pages", "ApplicationId");
            AddForeignKey("dbo.Mozaic_Pages", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
            DropColumn("dbo.Mozaic_Template", "Css");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mozaic_Template", "Css", c => c.String(nullable: false));
            DropForeignKey("dbo.Mozaic_CssPages", "CssId", "dbo.Mozaic_Css");
            DropForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Mozaic_Pages", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Mozaic_CssPages", new[] { "CssId" });
            DropIndex("dbo.Mozaic_CssPages", new[] { "PageId" });
            DropIndex("dbo.Mozaic_Pages", new[] { "ApplicationId" });
            DropColumn("dbo.Mozaic_Pages", "ApplicationId");
            DropTable("dbo.Mozaic_CssPages");
            DropTable("dbo.Mozaic_Css");
        }
    }
}
