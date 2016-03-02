namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rememberUserApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "DesignAppId", c => c.Int());
            CreateIndex("dbo.Persona_Users", "DesignAppId");
            AddForeignKey("dbo.Persona_Users", "DesignAppId", "dbo.Master_Applications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Persona_Users", "DesignAppId", "dbo.Master_Applications");
            DropIndex("dbo.Persona_Users", new[] { "DesignAppId" });
            DropColumn("dbo.Persona_Users", "DesignAppId");
        }
    }
}
