namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MozaicBootstrapEditorModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MozaicBootstrap_Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Content = c.String(),
                        CompiledPartialView = c.String(),
                        CompiledPageId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ParentApp_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ParentApp_Id, cascadeDelete: true)
                .Index(t => t.ParentApp_Id);
            
            CreateTable(
                "dbo.MozaicBootstrap_Components",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tag = c.String(),
                        UIC = c.String(),
                        Attributes = c.String(),
                        Content = c.String(),
                        ParentComponentId = c.Int(),
                        MozaicBootstrapPageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MozaicBootstrap_Components", t => t.ParentComponentId)
                .ForeignKey("dbo.MozaicBootstrap_Page", t => t.MozaicBootstrapPageId, cascadeDelete: true)
                .Index(t => t.ParentComponentId)
                .Index(t => t.MozaicBootstrapPageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MozaicBootstrap_Page", "ParentApp_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.MozaicBootstrap_Components", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            DropForeignKey("dbo.MozaicBootstrap_Components", "ParentComponentId", "dbo.MozaicBootstrap_Components");
            DropIndex("dbo.MozaicBootstrap_Components", new[] { "MozaicBootstrapPageId" });
            DropIndex("dbo.MozaicBootstrap_Components", new[] { "ParentComponentId" });
            DropIndex("dbo.MozaicBootstrap_Page", new[] { "ParentApp_Id" });
            DropTable("dbo.MozaicBootstrap_Components");
            DropTable("dbo.MozaicBootstrap_Page");
        }
    }
}
