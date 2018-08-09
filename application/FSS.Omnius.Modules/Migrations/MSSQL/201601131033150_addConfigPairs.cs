namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addConfigPairs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CORE_ConfigPairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 100),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Key, unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.CORE_ConfigPairs", new[] { "Key" });
            DropTable("dbo.CORE_ConfigPairs");
        }
    }
}
