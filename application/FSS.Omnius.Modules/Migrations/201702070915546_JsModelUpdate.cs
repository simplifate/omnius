namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JsModelUpdate : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Mozaic_Js", name: "PageId", newName: "MozaicBootstrapPageId");
            RenameIndex(table: "dbo.Mozaic_Js", name: "IX_PageId", newName: "IX_MozaicBootstrapPageId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Mozaic_Js", name: "IX_MozaicBootstrapPageId", newName: "IX_PageId");
            RenameColumn(table: "dbo.Mozaic_Js", name: "MozaicBootstrapPageId", newName: "PageId");
        }
    }
}
