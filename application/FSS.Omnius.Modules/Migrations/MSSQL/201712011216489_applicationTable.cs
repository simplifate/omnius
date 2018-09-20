namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applicationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "IsAllowedGuests", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "IsAllowedGuests");
        }
    }
}
