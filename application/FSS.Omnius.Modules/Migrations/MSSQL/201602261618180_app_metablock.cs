namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class app_metablock : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropIndex("dbo.Master_Applications", new[] { "TapestryDesignerRootMetablock_Id" });
            AddColumn("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentApp_Id");
            DropColumn("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id");
            AddForeignKey("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", "Master_Applications", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", "dbo.Master_Applications");
            AddColumn("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", c => c.Int());
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentApp_Id" });
            DropColumn("dbo.TapestryDesigner_Metablocks", "ParentApp_Id");
            CreateIndex("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id");
            AddForeignKey("dbo.Master_Applications", "TapestryDesignerRootMetablock_Id", "TapestryDesigner_Metablocks", "Id");
        }
    }
}
