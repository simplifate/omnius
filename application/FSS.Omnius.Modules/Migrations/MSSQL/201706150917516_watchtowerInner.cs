namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class watchtowerInner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Watchtower_LogItems", "ParentLogItemId", c => c.Int());
            CreateIndex("dbo.Watchtower_LogItems", "ParentLogItemId");
            AddForeignKey("dbo.Watchtower_LogItems", "ParentLogItemId", "dbo.Watchtower_LogItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Watchtower_LogItems", "ParentLogItemId", "dbo.Watchtower_LogItems");
            DropIndex("dbo.Watchtower_LogItems", new[] { "ParentLogItemId" });
            DropColumn("dbo.Watchtower_LogItems", "ParentLogItemId");
        }
    }
}
