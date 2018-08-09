namespace FSS.Omnius.Modules.Migrations.MSSQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VirtualBlockLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tapestry_Blocks", "IsVirtualForBlockId", c => c.Int());
            CreateIndex("dbo.Tapestry_Blocks", "IsVirtualForBlockId");
            AddForeignKey("dbo.Tapestry_Blocks", "IsVirtualForBlockId", "dbo.Tapestry_Blocks", "Id");
            DropColumn("dbo.Tapestry_Blocks", "IsVirtual");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tapestry_Blocks", "IsVirtual", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Tapestry_Blocks", "IsVirtualForBlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_Blocks", new[] { "IsVirtualForBlockId" });
            DropColumn("dbo.Tapestry_Blocks", "IsVirtualForBlockId");
        }
    }
}
