namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypeExtend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CORE_DataTypes", "DBColumnTypeName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CORE_DataTypes", "DBColumnTypeName");
        }
    }
}
