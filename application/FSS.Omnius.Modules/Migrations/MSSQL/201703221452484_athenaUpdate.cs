namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class athenaUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Athena_Graph", "DemoData", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Athena_Graph", "DemoData");
        }
    }
}
