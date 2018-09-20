namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubflowUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TapestryDesigner_Subflow", "CommentBottom", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Subflow", "CommentBottom");
        }
    }
}
