namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDataType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CORE_DataTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CSharpName = c.String(nullable: false, maxLength: 50),
                        SqlName = c.String(nullable: false, maxLength: 50),
                        limited = c.Boolean(nullable: false),
                        shortcut = c.String(nullable: false, maxLength: 1),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.shortcut, unique: true);
            
            AddColumn("dbo.Tapestry_AttributeRules", "AttributeDataTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tapestry_AttributeRules", "AttributeDataTypeId");
            AddForeignKey("dbo.Tapestry_AttributeRules", "AttributeDataTypeId", "dbo.CORE_DataTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_AttributeRules", "AttributeDataTypeId", "dbo.CORE_DataTypes");
            DropIndex("dbo.CORE_DataTypes", new[] { "shortcut" });
            DropIndex("dbo.Tapestry_AttributeRules", new[] { "AttributeDataTypeId" });
            DropColumn("dbo.Tapestry_AttributeRules", "AttributeDataTypeId");
            DropTable("dbo.CORE_DataTypes");
        }
    }
}
