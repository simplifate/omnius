namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pageOldRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mozaic_Pages", "Application_Id", "dbo.Master_Applications");
            DropForeignKey("dbo.Mozaic_Pages", "Template_Id", "dbo.Mozaic_Template");
            DropIndex("dbo.Mozaic_Pages", new[] { "Application_Id" });
            DropIndex("dbo.Mozaic_Pages", new[] { "Template_Id" });
            DropColumn("dbo.Mozaic_Pages", "Application_Id");
            DropColumn("dbo.Mozaic_Pages", "Template_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Mozaic_Pages", "Template_Id", c => c.Int());
            AddColumn("dbo.Mozaic_Pages", "Application_Id", c => c.Int());
            CreateIndex("dbo.Mozaic_Pages", "Template_Id");
            CreateIndex("dbo.Mozaic_Pages", "Application_Id");
            AddForeignKey("dbo.Mozaic_Pages", "Template_Id", "dbo.Mozaic_Template", "Id");
            AddForeignKey("dbo.Mozaic_Pages", "Application_Id", "dbo.Master_Applications", "Id");
        }
    }
}
