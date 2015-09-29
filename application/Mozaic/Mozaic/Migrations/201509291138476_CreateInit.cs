namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateInit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateCategories", t => t.Parent_Id)
                .Index(t => t.Parent_Id);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Html = c.String(),
                        Css = c.String(),
                        Category_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TemplateCategories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "Category_Id", "dbo.TemplateCategories");
            DropForeignKey("dbo.TemplateCategories", "Parent_Id", "dbo.TemplateCategories");
            DropIndex("dbo.Templates", new[] { "Category_Id" });
            DropIndex("dbo.TemplateCategories", new[] { "Parent_Id" });
            DropTable("dbo.Templates");
            DropTable("dbo.TemplateCategories");
        }
    }
}
