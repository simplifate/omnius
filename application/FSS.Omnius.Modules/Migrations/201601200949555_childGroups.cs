namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class childGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona_Groups", "ParentId", c => c.Int());
            CreateIndex("dbo.Persona_Groups", "ParentId");
            AddForeignKey("dbo.Persona_Groups", "ParentId", "dbo.Persona_Groups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Persona_Groups", "ParentId", "dbo.Persona_Groups");
            DropIndex("dbo.Persona_Groups", new[] { "ParentId" });
            DropColumn("dbo.Persona_Groups", "ParentId");
        }
    }
}
