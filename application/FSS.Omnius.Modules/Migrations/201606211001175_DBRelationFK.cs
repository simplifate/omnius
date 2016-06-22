namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBRelationFK : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Entitron_DbRelation", "LeftTableId");
            CreateIndex("dbo.Entitron_DbRelation", "LeftColumnId");
            CreateIndex("dbo.Entitron_DbRelation", "RightTableId");
            CreateIndex("dbo.Entitron_DbRelation", "RightColumnId");
            AddForeignKey("dbo.Entitron_DbRelation", "LeftColumnId", "dbo.Entitron_DbColumn", "Id");
            AddForeignKey("dbo.Entitron_DbRelation", "LeftTableId", "dbo.Entitron_DbTable", "Id");
            AddForeignKey("dbo.Entitron_DbRelation", "RightColumnId", "dbo.Entitron_DbColumn", "Id");
            AddForeignKey("dbo.Entitron_DbRelation", "RightTableId", "dbo.Entitron_DbTable", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Entitron_DbRelation", "RightTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbRelation", "RightColumnId", "dbo.Entitron_DbColumn");
            DropForeignKey("dbo.Entitron_DbRelation", "LeftTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbRelation", "LeftColumnId", "dbo.Entitron_DbColumn");
            DropIndex("dbo.Entitron_DbRelation", new[] { "RightColumnId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "RightTableId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "LeftColumnId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "LeftTableId" });
        }
    }
}
