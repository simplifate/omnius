namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleWhitelistAttributeToBlock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "RoleWhitelist", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_Blocks", "RoleWhitelist");
        }
    }
}
