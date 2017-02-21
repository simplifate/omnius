namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedColumnToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Users", "LastAction", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona_Users", "LastAction");
        }
    }
}
