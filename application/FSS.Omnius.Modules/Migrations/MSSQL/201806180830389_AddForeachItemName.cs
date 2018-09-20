namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeachItemName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Foreach", "ItemName", c => c.String(maxLength: 50));
            AlterColumn("dbo.TapestryDesigner_Foreach", "Name", c => c.String(maxLength: 50));
            AlterColumn("dbo.TapestryDesigner_Foreach", "DataSource", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TapestryDesigner_Foreach", "DataSource", c => c.String());
            AlterColumn("dbo.TapestryDesigner_Foreach", "Name", c => c.String());
            DropColumn("dbo.TapestryDesigner_Foreach", "ItemName");
        }
    }
}
