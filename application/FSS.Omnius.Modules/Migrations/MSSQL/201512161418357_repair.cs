namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repair : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Watchtower_LogItems", newName: "LogItems");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.LogItems", newName: "Watchtower_LogItems");
        }
    }
}
