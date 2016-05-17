namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conditionSetCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id", cascadeDelete: false);
        }
    }
}
