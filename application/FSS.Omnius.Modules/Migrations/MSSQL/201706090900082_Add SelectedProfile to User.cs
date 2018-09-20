namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSelectedProfiletoUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "SelectedProfile", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "SelectedProfile");
        }
    }
}
