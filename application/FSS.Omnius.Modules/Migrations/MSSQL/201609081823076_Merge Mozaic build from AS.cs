namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeMozaicbuildfromAS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "EditorPageId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tapestry_Blocks", "EditorPageId");
        }
    }
}
