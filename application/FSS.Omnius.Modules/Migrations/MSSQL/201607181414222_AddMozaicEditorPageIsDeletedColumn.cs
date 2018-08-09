namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMozaicEditorPageIsDeletedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Pages", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicEditor_Pages", "IsDeleted");
        }
    }
}
