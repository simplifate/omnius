namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class athenaupdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Athena_Graph", "Html", c => c.String());
            AddColumn("dbo.Athena_Graph", "Library", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Athena_Graph", "Library");
            DropColumn("dbo.Athena_Graph", "Html");
        }
    }
}
