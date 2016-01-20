namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class actionRigthsToActionRuleRights : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Persona_ActionRights", "GroupId", "dbo.Persona_Groups");
            DropIndex("dbo.Persona_ActionRights", new[] { "GroupId" });
            DropTable("dbo.Persona_ActionRights");
            CreateTable(
                "dbo.Persona_ActionRuleRights",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        ActionRuleId = c.Int(nullable: false),
                        Readable = c.Boolean(nullable: false),
                        Executable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.ActionRuleId })
                .ForeignKey("dbo.Persona_Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Tapestry_ActionRule", t => t.ActionRuleId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.ActionRuleId);
            
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Persona_ActionRights",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Readable = c.Boolean(nullable: false),
                        Executable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.ActionId });
            
            DropForeignKey("dbo.Persona_ActionRuleRights", "ActionRuleId", "dbo.Tapestry_ActionRule");
            DropForeignKey("dbo.Persona_ActionRuleRights", "GroupId", "dbo.Persona_Groups");
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "ActionRuleId" });
            DropIndex("dbo.Persona_ActionRuleRights", new[] { "GroupId" });
            DropTable("dbo.Persona_ActionRuleRights");
            CreateIndex("dbo.Persona_ActionRights", "GroupId");
            AddForeignKey("dbo.Persona_ActionRights", "GroupId", "dbo.Persona_Groups", "Id", cascadeDelete: true);
        }
    }
}
