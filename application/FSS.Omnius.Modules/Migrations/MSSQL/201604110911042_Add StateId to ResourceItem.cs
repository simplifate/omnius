namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStateIdtoResourceItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "StateId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ResourceItems", "StateId");
        }
    }
}
