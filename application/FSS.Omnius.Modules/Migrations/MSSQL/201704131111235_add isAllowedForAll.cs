namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addisAllowedForAll : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "IsAllowedForAll", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "IsAllowedForAll");
        }
    }
}
