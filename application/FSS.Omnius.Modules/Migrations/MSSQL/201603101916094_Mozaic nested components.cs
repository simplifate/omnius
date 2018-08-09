namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mozaicnestedcomponents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Components", "MozaicEditorComponent_Id", c => c.Int());
            CreateIndex("dbo.MozaicEditor_Components", "MozaicEditorComponent_Id");
            AddForeignKey("dbo.MozaicEditor_Components", "MozaicEditorComponent_Id", "dbo.MozaicEditor_Components", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MozaicEditor_Components", "MozaicEditorComponent_Id", "dbo.MozaicEditor_Components");
            DropIndex("dbo.MozaicEditor_Components", new[] { "MozaicEditorComponent_Id" });
            DropColumn("dbo.MozaicEditor_Components", "MozaicEditorComponent_Id");
        }
    }
}
