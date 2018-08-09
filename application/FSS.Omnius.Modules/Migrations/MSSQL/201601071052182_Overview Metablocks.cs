namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverviewMetablocks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.TapestryDesigner_Apps");
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentMetaBlock_Id" });
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentApp_Id" });
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetaBlock_Id" });
            AddColumn("dbo.TapestryDesigner_Blocks", "AssociatedTableId", c => c.Int(nullable: false));
            AlterColumn("dbo.TapestryDesigner_Apps", "Id", c => c.Int(nullable: false));
            CreateIndex("dbo.TapestryDesigner_Apps", "Id");
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentMetablock_Id");
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetablock_Id");
            AddForeignKey("dbo.TapestryDesigner_Apps", "Id", "dbo.TapestryDesigner_Metablocks", "Id");
            DropColumn("dbo.TapestryDesigner_Metablocks", "ParentApp_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_Metablocks", "ParentApp_Id", c => c.Int());
            DropForeignKey("dbo.TapestryDesigner_Apps", "Id", "dbo.TapestryDesigner_Metablocks");
            DropIndex("dbo.TapestryDesigner_Blocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Metablocks", new[] { "ParentMetablock_Id" });
            DropIndex("dbo.TapestryDesigner_Apps", new[] { "Id" });
            DropPrimaryKey("dbo.TapestryDesigner_Apps");
            AlterColumn("dbo.TapestryDesigner_Apps", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.TapestryDesigner_Blocks", "AssociatedTableId");
            AddPrimaryKey("dbo.TapestryDesigner_Apps", "Id");
            CreateIndex("dbo.TapestryDesigner_Blocks", "ParentMetaBlock_Id");
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentApp_Id");
            CreateIndex("dbo.TapestryDesigner_Metablocks", "ParentMetaBlock_Id");
            AddForeignKey("dbo.TapestryDesigner_MetaBlocks", "ParentApp_Id", "dbo.TapestryDesigner_Apps", "Id");
        }
    }
}
