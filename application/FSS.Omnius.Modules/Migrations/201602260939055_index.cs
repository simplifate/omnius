namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class index : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entitron_DbIndex", "ColumnNames", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entitron_DbIndex", "ColumnNames");
        }
    }
}
