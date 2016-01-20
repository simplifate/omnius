namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupsFromAD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Groups", "IsFromAD", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Groups", "IsFromAD");
        }
    }
}
