namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pages", "MasterTemplate_Id", "dbo.Templates");
            DropIndex("dbo.Pages", new[] { "MasterTemplate_Id" });
            AlterColumn("dbo.Pages", "MasterTemplate_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Pages", "MasterTemplate_Id");
            AddForeignKey("dbo.Pages", "MasterTemplate_Id", "dbo.Templates", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "MasterTemplate_Id", "dbo.Templates");
            DropIndex("dbo.Pages", new[] { "MasterTemplate_Id" });
            AlterColumn("dbo.Pages", "MasterTemplate_Id", c => c.Int());
            CreateIndex("dbo.Pages", "MasterTemplate_Id");
            AddForeignKey("dbo.Pages", "MasterTemplate_Id", "dbo.Templates", "Id");
        }
    }
}
