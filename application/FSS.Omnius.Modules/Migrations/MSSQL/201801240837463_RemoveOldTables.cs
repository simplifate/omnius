namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Master_Applications", "CssTemplate_Id", "dbo.Mozaic_CssTemplates");
            DropForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Mozaic_CssPages", "CssId", "dbo.Mozaic_Css");
            DropForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Tapestry_PreBlockActions", "BlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Mozaic_TemplateCategories", "ParentId", "dbo.Mozaic_TemplateCategories");
            DropForeignKey("dbo.Mozaic_Template", "CategoryId", "dbo.Mozaic_TemplateCategories");
            DropIndex("dbo.Master_Applications", new[] { "CssTemplate_Id" });
            DropIndex("dbo.Tapestry_AttributeRules", new[] { "BlockId" });
            DropIndex("dbo.Tapestry_PreBlockActions", new[] { "BlockId" });
            DropIndex("dbo.Mozaic_TemplateCategories", new[] { "ParentId" });
            DropIndex("dbo.Mozaic_Template", new[] { "CategoryId" });
            DropIndex("dbo.Mozaic_CssPages", new[] { "CssId" });
            DropIndex("dbo.Mozaic_CssPages", new[] { "PageId" });
            DropColumn("dbo.Master_Applications", "CssTemplate_Id");
            DropTable("dbo.Mozaic_CssTemplates");
            DropTable("dbo.Tapestry_AttributeRules");
            DropTable("dbo.Mozaic_Css");
            DropTable("dbo.Tapestry_PreBlockActions");
            DropTable("dbo.Tapestry_ActionSequences");
            DropTable("dbo.Mozaic_TemplateCategories");
            DropTable("dbo.Mozaic_Template");
            DropTable("dbo.Mozaic_CssPages");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Mozaic_CssPages",
                c => new
                    {
                        CssId = c.Int(nullable: false),
                        PageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CssId, t.PageId });
            
            CreateTable(
                "dbo.Mozaic_Template",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Html = c.String(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_TemplateCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tapestry_ActionSequences",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        ChildId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.ChildId });
            
            CreateTable(
                "dbo.Tapestry_PreBlockActions",
                c => new
                    {
                        BlockId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        InputVariablesMapping = c.String(maxLength: 200),
                        OutputVariablesMapping = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => new { t.BlockId, t.ActionId });
            
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
                "dbo.Tapestry_AttributeRules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InputName = c.String(nullable: false, maxLength: 50),
                        AttributeName = c.String(nullable: false, maxLength: 50),
                        AttributeDataTypeId = c.Int(nullable: false),
                        BlockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mozaic_CssTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Master_Applications", "CssTemplate_Id", c => c.Int());
            CreateIndex("dbo.Mozaic_CssPages", "PageId");
            CreateIndex("dbo.Mozaic_CssPages", "CssId");
            CreateIndex("dbo.Mozaic_Template", "CategoryId");
            CreateIndex("dbo.Mozaic_TemplateCategories", "ParentId");
            CreateIndex("dbo.Tapestry_PreBlockActions", "BlockId");
            CreateIndex("dbo.Tapestry_AttributeRules", "BlockId");
            CreateIndex("dbo.Master_Applications", "CssTemplate_Id");
            AddForeignKey("dbo.Mozaic_Template", "CategoryId", "dbo.Mozaic_TemplateCategories", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Mozaic_TemplateCategories", "ParentId", "dbo.Mozaic_TemplateCategories", "Id");
            AddForeignKey("dbo.Tapestry_PreBlockActions", "BlockId", "dbo.Tapestry_Blocks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Mozaic_CssPages", "PageId", "dbo.Mozaic_Pages", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Mozaic_CssPages", "CssId", "dbo.Mozaic_Css", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Master_Applications", "CssTemplate_Id", "dbo.Mozaic_CssTemplates", "Id");
        }
    }
}
