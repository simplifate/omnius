namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPreBlockRun : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "PreBlockActionRuleId", c => c.Int());
            CreateIndex("dbo.Tapestry_Blocks", "PreBlockActionRuleId");
            AddForeignKey("dbo.Tapestry_Blocks", "PreBlockActionRuleId", "dbo.Tapestry_ActionRule", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_Blocks", "PreBlockActionRuleId", "dbo.Tapestry_ActionRule");
            DropIndex("dbo.Tapestry_Blocks", new[] { "PreBlockActionRuleId" });
            DropColumn("dbo.Tapestry_Blocks", "PreBlockActionRuleId");
        }
    }
}
