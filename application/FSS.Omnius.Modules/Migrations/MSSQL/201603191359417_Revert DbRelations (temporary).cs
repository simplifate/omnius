namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevertDbRelationstemporary : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Entitron_DbRelation", "LeftColumnId", "dbo.Entitron_DbColumn");
            DropForeignKey("dbo.Entitron_DbRelation", "LeftTableId", "dbo.Entitron_DbTable");
            DropForeignKey("dbo.Entitron_DbRelation", "RightColumnId", "dbo.Entitron_DbColumn");
            DropForeignKey("dbo.Entitron_DbRelation", "RightTableId", "dbo.Entitron_DbTable");
            DropIndex("dbo.Entitron_DbRelation", new[] { "LeftTableId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "LeftColumnId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "RightTableId" });
            DropIndex("dbo.Entitron_DbRelation", new[] { "RightColumnId" });
            DropColumn("dbo.Entitron_DbRelation", "LeftTable");
            DropColumn("dbo.Entitron_DbRelation", "LeftColumn");
            DropColumn("dbo.Entitron_DbRelation", "RightTable");
            DropColumn("dbo.Entitron_DbRelation", "RightColumn");
            AddColumn("dbo.Entitron_DbRelation", "LeftTable", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "LeftColumn", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "RightTable", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "RightColumn", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AddColumn("dbo.Entitron_DbRelation", "RightColumnId", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "RightTableId", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "LeftColumnId", c => c.Int(nullable: false));
            AddColumn("dbo.Entitron_DbRelation", "LeftTableId", c => c.Int(nullable: false));
            DropColumn("dbo.Entitron_DbRelation", "RightColumn");
            DropColumn("dbo.Entitron_DbRelation", "RightTable");
            DropColumn("dbo.Entitron_DbRelation", "LeftColumn");
            DropColumn("dbo.Entitron_DbRelation", "LeftTable");
            CreateIndex("dbo.Entitron_DbRelation", "RightColumnId");
            CreateIndex("dbo.Entitron_DbRelation", "RightTableId");
            CreateIndex("dbo.Entitron_DbRelation", "LeftColumnId");
            CreateIndex("dbo.Entitron_DbRelation", "LeftTableId");
            AddForeignKey("dbo.Entitron_DbRelation", "RightTableId", "dbo.Entitron_DbTable", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Entitron_DbRelation", "RightColumnId", "dbo.Entitron_DbColumn", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Entitron_DbRelation", "LeftTableId", "dbo.Entitron_DbTable", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Entitron_DbRelation", "LeftColumnId", "dbo.Entitron_DbColumn", "Id", cascadeDelete: true);
        }
    }
}
