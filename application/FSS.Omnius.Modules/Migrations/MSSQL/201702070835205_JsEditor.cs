namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class JsEditor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Mozaic_Js",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        PageId = c.Int(),
                        Name = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Mozaic_Pages", t => t.PageId)
                .Index(t => t.ApplicationId)
                .Index(t => t.PageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mozaic_Js", "PageId", "dbo.Mozaic_Pages");
            DropForeignKey("dbo.Mozaic_Js", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Mozaic_Js", new[] { "PageId" });
            DropIndex("dbo.Mozaic_Js", new[] { "ApplicationId" });
            DropTable("dbo.Mozaic_Js");
        }
    }
}
