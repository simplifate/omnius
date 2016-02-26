namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gendatabase : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Entitron_DbIndex", "ColumnNames");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Entitron_DbIndex", "ColumnNames", c => c.String());
        }
    }
}
