namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TapestryblockisInMenu2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Metablocks", "IsInMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.TapestryDesigner_Blocks", "IsInMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Blocks", "IsInMenu");
            DropColumn("dbo.TapestryDesigner_Metablocks", "IsInMenu");
        }
    }
}
