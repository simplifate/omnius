namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class generateDatabase : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CORE_ConfigPairs", new[] { "Key" });
            DropColumn("dbo.Persona_Groups", "IsFromAD");
            DropTable("dbo.CORE_ConfigPairs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CORE_ConfigPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 100),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_Groups", "IsFromAD", c => c.Boolean(nullable: false));
            CreateIndex("dbo.CORE_ConfigPairs", "Key", unique: true);
        }
    }
}
