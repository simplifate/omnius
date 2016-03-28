namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Conditionsandconditionsets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TapestryDesigner_Conditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        Relation = c.String(),
                        Variable = c.String(),
                        Operator = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TapestryDesigner_ConditionSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SetIndex = c.Int(nullable: false),
                        SetRelation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TapestryDesigner_ConditionSets");
            DropTable("dbo.TapestryDesigner_Conditions");
        }
    }
}
