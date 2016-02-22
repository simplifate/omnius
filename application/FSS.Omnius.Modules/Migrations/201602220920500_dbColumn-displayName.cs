namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbColumndisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entitron_DbColumn", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entitron_DbColumn", "DisplayName");
        }
    }
}
