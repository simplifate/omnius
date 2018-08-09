namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPreBlockRun : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tapestry_PreBlockActions",
                c => new
                    {
                        BlockId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        InputVariablesMapping = c.String(maxLength: 200),
                        OutputVariablesMapping = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => new { t.BlockId, t.ActionId })
                .ForeignKey("dbo.Tapestry_Blocks", t => t.BlockId, cascadeDelete: true)
                .Index(t => t.BlockId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_PreBlockActions", "BlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_PreBlockActions", new[] { "BlockId" });
            DropTable("dbo.Tapestry_PreBlockActions");
        }
    }
}
