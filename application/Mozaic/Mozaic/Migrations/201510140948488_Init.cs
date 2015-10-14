namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mozaic_Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Relations = c.String(nullable: false),
                        MasterTemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_Template", t => t.MasterTemplateId, cascadeDelete: true)
                .Index(t => t.MasterTemplateId);
            
            CreateTable(
                "dbo.Mozaic_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Html = c.String(nullable: false),
                        Css = c.String(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_TemplateCategories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Mozaic_TemplateCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Mozaic_TemplateCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mozaic_Pages", "MasterTemplateId", "dbo.Mozaic_Template");
            DropForeignKey("dbo.Mozaic_Template", "CategoryId", "dbo.Mozaic_TemplateCategories");
            DropForeignKey("dbo.Mozaic_TemplateCategories", "ParentId", "dbo.Mozaic_TemplateCategories");
            DropIndex("dbo.Mozaic_TemplateCategories", new[] { "ParentId" });
            DropIndex("dbo.Mozaic_Template", new[] { "CategoryId" });
            DropIndex("dbo.Mozaic_Pages", new[] { "MasterTemplateId" });
            DropTable("dbo.Mozaic_TemplateCategories");
            DropTable("dbo.Mozaic_Template");
            DropTable("dbo.Mozaic_Pages");
            DropTable("dbo.Master_Applications");
        }
    }
}
