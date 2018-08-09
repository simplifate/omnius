namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNameToDbRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entitron_DbRelation", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Entitron_DbRelation", "Name");
        }
    }
}
