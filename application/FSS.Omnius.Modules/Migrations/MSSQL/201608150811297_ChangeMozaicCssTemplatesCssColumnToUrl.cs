namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeMozaicCssTemplatesCssColumnToUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mozaic_CssTemplates", "Url", c => c.String());
            DropColumn("dbo.Mozaic_CssTemplates", "CSS");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mozaic_CssTemplates", "CSS", c => c.String());
            DropColumn("dbo.Mozaic_CssTemplates", "Url");
        }
    }
}
