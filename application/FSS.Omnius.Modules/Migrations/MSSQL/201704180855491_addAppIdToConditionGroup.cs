namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAppIdToConditionGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ConditionGroups", "ApplicationId", c => c.Int());
            CreateIndex("dbo.TapestryDesigner_ConditionGroups", "ApplicationId");
            AddForeignKey("dbo.TapestryDesigner_ConditionGroups", "ApplicationId", "dbo.Master_Applications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "ApplicationId" });
            DropColumn("dbo.TapestryDesigner_ConditionGroups", "ApplicationId");
        }
    }
}
