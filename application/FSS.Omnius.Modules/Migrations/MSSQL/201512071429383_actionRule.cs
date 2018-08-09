namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class actionRule : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Tapestry_AttributeRoles", newName: "Tapestry_AttributeRules");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Tapestry_AttributeRules", newName: "Tapestry_AttributeRoles");
        }
    }
}
