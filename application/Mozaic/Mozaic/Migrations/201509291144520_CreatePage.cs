namespace Mozaic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PartialRelations = c.String(),
                        DatasourceRelations = c.String(),
                        MasterTemplate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.MasterTemplate_Id)
                .Index(t => t.MasterTemplate_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pages", "MasterTemplate_Id", "dbo.Templates");
            DropIndex("dbo.Pages", new[] { "MasterTemplate_Id" });
            DropTable("dbo.Pages");
        }
    }
}
