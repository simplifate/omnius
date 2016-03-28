namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPriorityToPersonaAppRoles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_AppRoles", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_AppRoles", "Priority");
        }
    }
}
