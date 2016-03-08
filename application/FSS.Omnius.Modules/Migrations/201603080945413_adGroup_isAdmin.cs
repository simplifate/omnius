namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adGroup_isAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_ADgroups", "isAdmin", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_ADgroups", "isAdmin");
        }
    }
}
