namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCSSTemplateTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mozaic_CssTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CSS = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Master_Applications", "CssTemplate_Id", c => c.Int());
            CreateIndex("dbo.Master_Applications", "CssTemplate_Id");
            AddForeignKey("dbo.Master_Applications", "CssTemplate_Id", "dbo.Mozaic_CssTemplates", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Master_Applications", "CssTemplate_Id", "dbo.Mozaic_CssTemplates");
            DropIndex("dbo.Master_Applications", new[] { "CssTemplate_Id" });
            DropColumn("dbo.Master_Applications", "CssTemplate_Id");
            DropTable("dbo.Mozaic_CssTemplates");
        }
    }
}
