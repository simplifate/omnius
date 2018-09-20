namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryDesignerForeachUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Foreach", "DataSource", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Foreach", "DataSource");
        }
    }
}
