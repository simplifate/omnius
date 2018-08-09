namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MozaicBootstrapModelsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MozaicBootstrap_Components", "ElmId", c => c.String());
            AddColumn("dbo.MozaicBootstrap_Components", "Properties", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MozaicBootstrap_Components", "Properties");
            DropColumn("dbo.MozaicBootstrap_Components", "ElmId");
        }
    }
}
