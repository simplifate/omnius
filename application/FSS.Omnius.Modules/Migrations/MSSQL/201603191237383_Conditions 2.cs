namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditions2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            CreateIndex("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id");
            AddForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id", "dbo.TapestryDesigner_ConditionSets");
            DropIndex("dbo.TapestryDesigner_Conditions", new[] { "TapestryDesignerConditionSet_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            DropColumn("dbo.TapestryDesigner_Conditions", "TapestryDesignerConditionSet_Id");
        }
    }
}
