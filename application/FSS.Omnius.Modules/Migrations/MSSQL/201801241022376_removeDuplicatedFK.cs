namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeDuplicatedFK : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [TapestryDesigner_MetablocksConnections] DROP CONSTRAINT [FK_dbo.TapestryDesigner_MetablocksConnections_dbo.TapestryDesigner_Metablocks_TapestryDesignerMetablock_Id]");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [TapestryDesigner_MetablocksConnections] ADD CONSTRAINT [FK_dbo.TapestryDesigner_MetablocksConnections_dbo.TapestryDesigner_Metablocks_TapestryDesignerMetablock_Id] FOREIGN KEY (TapestryDesignerMetablockId) REFERENCES [TapestryDesigner_MetaBlocks]([Id])");
        }
    }
}
