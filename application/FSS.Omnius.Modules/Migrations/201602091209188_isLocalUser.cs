namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isLocalUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Persona_Users", "isLocalUser");
            AddColumn("dbo.Persona_Users", "isLocalUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "isLocalUser");
            AddColumn("dbo.Persona_Users", "isLocalUser", c => c.Boolean(nullable: false));
        }
    }
}
