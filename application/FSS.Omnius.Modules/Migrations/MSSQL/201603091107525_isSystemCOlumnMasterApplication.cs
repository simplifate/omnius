namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isSystemCOlumnMasterApplication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Master_Applications", "IsSystem", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Master_Applications", "IsSystem");
        }
    }
}
