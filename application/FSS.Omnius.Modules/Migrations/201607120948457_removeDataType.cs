namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDataType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_AttributeRules", "AttributeDataTypeId", "dbo.CORE_DataTypes");
            DropIndex("dbo.Tapestry_AttributeRules", new[] { "AttributeDataTypeId" });
            DropIndex("dbo.CORE_DataTypes", new[] { "shortcut" });
            DropTable("dbo.CORE_DataTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CORE_DataTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CSharpName = c.String(nullable: false, maxLength: 50),
                        SqlName = c.String(nullable: false, maxLength: 50),
                        DBColumnTypeName = c.String(maxLength: 50),
                        limited = c.Boolean(nullable: false),
                        shortcut = c.String(nullable: false, maxLength: 1),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.CORE_DataTypes", "shortcut", unique: true);
            CreateIndex("dbo.Tapestry_AttributeRules", "AttributeDataTypeId");
            AddForeignKey("dbo.Tapestry_AttributeRules", "AttributeDataTypeId", "dbo.CORE_DataTypes", "Id", cascadeDelete: true);
        }
    }
}
