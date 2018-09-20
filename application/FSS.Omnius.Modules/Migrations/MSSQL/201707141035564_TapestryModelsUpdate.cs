namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryModelsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction", c => c.String());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart", c => c.Boolean());
            AddColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachEnd");
            DropColumn("dbo.Tapestry_ActionRule_Action", "IsForeachStart");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualParentId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualItemId");
            DropColumn("dbo.Tapestry_ActionRule_Action", "VirtualAction");
        }
    }
}
