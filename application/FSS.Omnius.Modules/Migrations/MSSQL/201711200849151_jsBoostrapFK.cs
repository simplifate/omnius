namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jsBoostrapFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mozaic_Js", "PageId", "dbo.Mozaic_Pages");
            AddForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.MozaicBootstrap_Page");
            AddForeignKey("dbo.Mozaic_Js", "MozaicBootstrapPageId", "dbo.Mozaic_Pages", "Id");
        }
    }
}
