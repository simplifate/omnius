namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredAttributesTemplatePage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "Category_Id", "dbo.TemplateCategories");
            DropIndex("dbo.Templates", new[] { "Category_Id" });
            AlterColumn("dbo.Pages", "PartialRelations", c => c.String(nullable: false, defaultValue: ""));
            AlterColumn("dbo.Pages", "DatasourceRelations", c => c.String(nullable: false, defaultValue: ""));
            AlterColumn("dbo.Templates", "Html", c => c.String(nullable: false, defaultValue: ""));
            AlterColumn("dbo.Templates", "Css", c => c.String(nullable: false, defaultValue: ""));
            AlterColumn("dbo.Templates", "Category_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Templates", "Category_Id");
            AddForeignKey("dbo.Templates", "Category_Id", "dbo.TemplateCategories", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "Category_Id", "dbo.TemplateCategories");
            DropIndex("dbo.Templates", new[] { "Category_Id" });
            AlterColumn("dbo.Templates", "Category_Id", c => c.Int());
            AlterColumn("dbo.Templates", "Css", c => c.String());
            AlterColumn("dbo.Templates", "Html", c => c.String());
            AlterColumn("dbo.Pages", "DatasourceRelations", c => c.String());
            AlterColumn("dbo.Pages", "PartialRelations", c => c.String());
            CreateIndex("dbo.Templates", "Category_Id");
            AddForeignKey("dbo.Templates", "Category_Id", "dbo.TemplateCategories", "Id");
        }
    }
}
