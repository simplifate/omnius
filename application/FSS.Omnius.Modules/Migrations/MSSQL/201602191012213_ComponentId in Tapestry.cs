namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComponentIdinTapestry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_ResourceItems", "ComponentId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_ResourceItems", "ComponentId");
        }
    }
}
