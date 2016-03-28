namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConnectionSplitStep2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.TapestryDesigner_Connections");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TapestryDesigner_Connections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceId = c.Int(nullable: false),
                        SourceSlot = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                        TargetSlot = c.Int(nullable: false),
                        WorkflowRuleId = c.Int(),
                        ResourceRuleId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
