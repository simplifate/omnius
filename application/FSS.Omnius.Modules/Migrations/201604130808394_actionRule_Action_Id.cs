namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class actionRule_Action_Id : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Tapestry_ActionRule_Action");
            AddColumn("dbo.Tapestry_ActionRule_Action", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Tapestry_ActionRule_Action", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Tapestry_ActionRule_Action");
            DropColumn("dbo.Tapestry_ActionRule_Action", "Id");
            AddPrimaryKey("dbo.Tapestry_ActionRule_Action", new[] { "ActionRuleId", "ActionId" });
        }
    }
}
