namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryAdvanceConditions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule", "isDefault", c => c.Boolean(nullable: false, defaultValue: false));
            DropColumn("dbo.Tapestry_ActionRule", "Condition");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tapestry_ActionRule", "Condition", c => c.String());
            DropColumn("dbo.Tapestry_ActionRule", "isDefault");
            DropColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId");
        }
    }
}
