namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBlockBuildLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Blocks", "BuiltBlockId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "BuiltBlockId");
        }
    }
}
