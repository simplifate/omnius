namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mozaicmodalsettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicEditor_Pages", "IsModal", c => c.Boolean(nullable: false));
            AddColumn("dbo.MozaicEditor_Pages", "ModalWidth", c => c.Int());
            AddColumn("dbo.MozaicEditor_Pages", "ModalHeight", c => c.Int());
            AddColumn("dbo.MozaicEditor_Pages", "CompiledPageId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicEditor_Pages", "CompiledPageId");
            DropColumn("dbo.MozaicEditor_Pages", "ModalHeight");
            DropColumn("dbo.MozaicEditor_Pages", "ModalWidth");
            DropColumn("dbo.MozaicEditor_Pages", "IsModal");
        }
    }
}
