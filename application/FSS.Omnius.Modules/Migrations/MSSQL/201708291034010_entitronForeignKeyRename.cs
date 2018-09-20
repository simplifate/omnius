namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class entitronForeignKeyRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "LeftColumnId", newName: "SourceColumnId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "LeftTableId", newName: "SourceTableId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "RightColumnId", newName: "TargetColumnId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "RightTableId", newName: "TargetTableId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_LeftTableId", newName: "IX_SourceTableId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_LeftColumnId", newName: "IX_SourceColumnId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_RightTableId", newName: "IX_TargetTableId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_RightColumnId", newName: "IX_TargetColumnId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_TargetColumnId", newName: "IX_RightColumnId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_TargetTableId", newName: "IX_RightTableId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_SourceColumnId", newName: "IX_LeftColumnId");
            RenameIndex(table: "dbo.Entitron_DbRelation", name: "IX_SourceTableId", newName: "IX_LeftTableId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "TargetTableId", newName: "RightTableId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "TargetColumnId", newName: "RightColumnId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "SourceTableId", newName: "LeftTableId");
            RenameColumn(table: "dbo.Entitron_DbRelation", name: "SourceColumnId", newName: "LeftColumnId");
        }
    }
}
