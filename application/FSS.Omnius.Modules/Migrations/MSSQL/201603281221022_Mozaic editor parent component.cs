namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mozaiceditorparentcomponent : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "MozaicEditorComponent_Id", newName: "ParentComponent_Id");
            RenameIndex(table: "dbo.MozaicEditor_Components", name: "IX_MozaicEditorComponent_Id", newName: "IX_ParentComponent_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MozaicEditor_Components", name: "IX_ParentComponent_Id", newName: "IX_MozaicEditorComponent_Id");
            RenameColumn(table: "dbo.MozaicEditor_Components", name: "ParentComponent_Id", newName: "MozaicEditorComponent_Id");
        }
    }
}
