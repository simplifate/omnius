namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeOldTapestryDesignerApp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_Apps", "RootMetablock_Id", "dbo.TapestryDesigner_Metablocks");
            DropIndex("dbo.TapestryDesigner_Apps", new[] { "RootMetablock_Id" });
            DropTable("dbo.TapestryDesigner_Apps");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TapestryDesigner_Apps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RootMetablock_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.TapestryDesigner_Apps", "RootMetablock_Id");
            AddForeignKey("dbo.TapestryDesigner_Apps", "RootMetablock_Id", "dbo.TapestryDesigner_Metablocks", "Id");
        }
    }
}
