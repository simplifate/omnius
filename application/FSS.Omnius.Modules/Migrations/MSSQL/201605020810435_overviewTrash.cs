namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class overviewTrash : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.TapestryDesigner_MetablocksConnections", name: "TapestryDesignerMetablock_Id", newName: "TapestryDesignerMetablockId");
            RenameIndex(table: "dbo.TapestryDesigner_MetablocksConnections", name: "IX_TapestryDesignerMetablock_Id", newName: "IX_TapestryDesignerMetablockId");
            AddColumn("dbo.TapestryDesigner_Blocks", "IsDeleted", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.TapestryDesigner_Metablocks", "IsDeleted", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TapestryDesigner_Metablocks", "IsDeleted");
            DropColumn("dbo.TapestryDesigner_Blocks", "IsDeleted");
            RenameIndex(table: "dbo.TapestryDesigner_MetablocksConnections", name: "IX_TapestryDesignerMetablockId", newName: "IX_TapestryDesignerMetablock_Id");
            RenameColumn(table: "dbo.TapestryDesigner_MetablocksConnections", name: "TapestryDesignerMetablockId", newName: "TapestryDesignerMetablock_Id");
        }
    }
}
