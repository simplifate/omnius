namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbSchemeCommitaddIsCompleteboolean : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Entitron_DbSchemeCommit", "IsComplete", c => c.Boolean(nullable: false));
            DropColumn("dbo.MozaicBootstrap_Components", "ElmId");
            DropColumn("dbo.MozaicBootstrap_Components", "Properties");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MozaicBootstrap_Components", "Properties", c => c.String());
            AddColumn("dbo.MozaicBootstrap_Components", "ElmId", c => c.String());
            DropColumn("dbo.Entitron_DbSchemeCommit", "IsComplete");
        }
    }
}
