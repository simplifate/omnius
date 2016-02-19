namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExecuteByToActionRule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_ActionRule", "ExecutedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_ActionRule", "ExecutedBy");
        }
    }
}
