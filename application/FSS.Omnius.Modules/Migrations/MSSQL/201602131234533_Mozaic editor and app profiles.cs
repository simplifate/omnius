namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mozaiceditorandappprofiles : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_Apps", "Id", "dbo.TapestryDesigner_Metablocks");
            DropIndex("dbo.TapestryDesigner_Apps", new[] { "Id" });
            DropPrimaryKey("dbo.TapestryDesigner_Apps");
            CreateTable(
                "dbo.MozaicEditor_Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompiledPartialView = c.String(),
                        ParentApp_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ParentApp_Id, cascadeDelete: true)
                .Index(t => t.ParentApp_Id);
            
            CreateTable(
                "dbo.MozaicEditor_Components",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PositionX = c.Int(nullable: false),
                        PositionY = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Tag = c.String(),
                        Attributes = c.String(),
                        Classes = c.String(),
                        Styles = c.String(),
                        Content = c.String(),
                        Label = c.String(),
                        Placeholder = c.String(),
                        Properties = c.String(),
                        MozaicEditorPage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MozaicEditor_Pages", t => t.MozaicEditorPage_Id)
                .Index(t => t.MozaicEditorPage_Id);
            
            AddColumn("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", c => c.Int());
            AddColumn("dbo.Entitron_DbSchemeCommit", "Application_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_Apps", "RootMetablock_Id", c => c.Int());
            AlterColumn("dbo.TapestryDesigner_Apps", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TapestryDesigner_Apps", "Id");
            CreateIndex("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id");
            CreateIndex("dbo.Entitron_DbSchemeCommit", "Application_Id");
            CreateIndex("dbo.TapestryDesigner_Apps", "RootMetablock_Id");
            AddForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications", "Id");
            AddForeignKey("dbo.TapestryDesigner_Apps", "RootMetablock_Id", "dbo.TapestryDesigner_Metablocks", "Id");
            AddForeignKey("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", "dbo.TapestryDesigner_Metablocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.TapestryDesigner_Apps", "RootMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropForeignKey("dbo.MozaicEditor_Pages", "ParentApp_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorPage_Id", "dbo.MozaicEditor_Pages");
            DropForeignKey("dbo.Entitron_DbSchemeCommit", "Application_Id", "dbo.Master_Applications");
            DropIndex("dbo.TapestryDesigner_Apps", new[] { "RootMetablock_Id" });
            DropIndex("dbo.MozaicEditor_Components", new[] { "MozaicEditorPage_Id" });
            DropIndex("dbo.MozaicEditor_Pages", new[] { "ParentApp_Id" });
            DropIndex("dbo.Entitron_DbSchemeCommit", new[] { "Application_Id" });
            DropIndex("dbo.Master_Applications", new[] { "TapestryDesignerRootMetablock_Id" });
            DropPrimaryKey("dbo.TapestryDesigner_Apps");
            AlterColumn("dbo.TapestryDesigner_Apps", "Id", c => c.Int(nullable: false));
            DropColumn("dbo.TapestryDesigner_Apps", "RootMetablock_Id");
            DropColumn("dbo.Entitron_DbSchemeCommit", "Application_Id");
            DropColumn("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id");
            DropTable("dbo.MozaicEditor_Components");
            DropTable("dbo.MozaicEditor_Pages");
            AddPrimaryKey("dbo.TapestryDesigner_Apps", "Id");
            CreateIndex("dbo.TapestryDesigner_Apps", "Id");
            AddForeignKey("dbo.TapestryDesigner_Apps", "Id", "dbo.TapestryDesigner_Metablocks", "Id");
        }
    }
}
