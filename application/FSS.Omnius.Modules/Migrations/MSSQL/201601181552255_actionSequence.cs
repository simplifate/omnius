namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class actionSequence : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_ActionRule_Action", "ActionId", "dbo.Tapestry_Actions");
            DropForeignKey("dbo.Tapestry_Actions", "MasterId", "dbo.Tapestry_Actions");
            DropIndex("dbo.Tapestry_ActionRule_Action", new[] { "ActionId" });
            DropIndex("dbo.Tapestry_Actions", new[] { "MasterId" });
            DropTable("dbo.Tapestry_Actions");

            CreateTable(
                "dbo.Tapestry_ActionSequences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChildId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Id, t.ChildId });
        }
        
        public override void Down()
        {
            DropTable("dbo.Tapestry_ActionSequences");

            CreateTable(
                "dbo.Tapestry_Actions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MasterId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            CreateIndex("dbo.Tapestry_Actions", "MasterId");
            CreateIndex("dbo.Tapestry_ActionRule_Action", "ActionId");
            AddForeignKey("dbo.Tapestry_Actions", "MasterId", "dbo.Tapestry_Actions", "Id");
            AddForeignKey("dbo.Tapestry_ActionRule_Action", "ActionId", "dbo.Tapestry_Actions", "Id", cascadeDelete: true);
        }
    }
}
