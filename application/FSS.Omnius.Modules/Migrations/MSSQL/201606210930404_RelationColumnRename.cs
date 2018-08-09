namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelationColumnRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Entitron_DbRelation", "LeftTable", "LeftTableId");
            RenameColumn("dbo.Entitron_DbRelation", "LeftColumn", "LeftColumnId");
            RenameColumn("dbo.Entitron_DbRelation", "RightTable", "RightTableId");
            RenameColumn("dbo.Entitron_DbRelation", "RightColumn", "RightColumnId");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Entitron_DbRelation", "LeftTableId", "LeftTable");
            RenameColumn("dbo.Entitron_DbRelation", "LeftColumnId", "LeftColumn");
            RenameColumn("dbo.Entitron_DbRelation", "RightTableId", "RightTable");
            RenameColumn("dbo.Entitron_DbRelation", "RightColumnId", "RightColumn");
        }
    }
}
