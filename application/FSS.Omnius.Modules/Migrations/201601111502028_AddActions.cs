namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tapestry_Actions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MasterId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Actions", t => t.MasterId)
                .Index(t => t.MasterId);
            
            CreateIndex("dbo.Tapestry_ActionRule_Action", "ActionId");
            AddForeignKey("dbo.Tapestry_ActionRule_Action", "ActionId", "dbo.Tapestry_Actions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_Actions", "MasterId", "dbo.Tapestry_Actions");
            DropForeignKey("dbo.Tapestry_ActionRule_Action", "ActionId", "dbo.Tapestry_Actions");
            DropIndex("dbo.Tapestry_Actions", new[] { "MasterId" });
            DropIndex("dbo.Tapestry_ActionRule_Action", new[] { "ActionId" });
            DropTable("dbo.Tapestry_Actions");
        }
    }
}
