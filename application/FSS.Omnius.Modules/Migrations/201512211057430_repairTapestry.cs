namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repairTapestry : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tapestry_AttributeRules", "FK_dbo.Tapestry_AttributeRoles_dbo.Tapestry_Blocks_BlockId");
            //DropForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_Blocks", new[] { "MozaicPageId" });
            DropPrimaryKey("dbo.Tapestry_Blocks");
            AlterColumn("dbo.Tapestry_Blocks", "MozaicPageId", c => c.Int());
            AlterColumn("dbo.Tapestry_Blocks", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Tapestry_WorkFlow", "InitBlockId", c => c.Int());
            AddPrimaryKey("dbo.Tapestry_Blocks", "Id");
            CreateIndex("dbo.Tapestry_Blocks", "MozaicPageId");
            AddForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks", "Id");
            AddForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks");
            DropIndex("dbo.Tapestry_Blocks", new[] { "MozaicPageId" });
            DropPrimaryKey("dbo.Tapestry_Blocks");
            AlterColumn("dbo.Tapestry_WorkFlow", "InitBlockId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tapestry_Blocks", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Tapestry_Blocks", "MozaicPageId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Tapestry_Blocks", "MozaicPageId");
            CreateIndex("dbo.Tapestry_Blocks", "MozaicPageId");
            AddForeignKey("dbo.Tapestry_ActionRule", "TargetBlockId", "dbo.Tapestry_Blocks", "MozaicPageId");
            AddForeignKey("dbo.Tapestry_ActionRule", "SourceBlockId", "dbo.Tapestry_Blocks", "MozaicPageId", cascadeDelete: true);
            AddForeignKey("dbo.Tapestry_WorkFlow", "Id", "dbo.Tapestry_Blocks", "MozaicPageId");
            AddForeignKey("dbo.Tapestry_AttributeRules", "BlockId", "dbo.Tapestry_Blocks", "MozaicPageId", cascadeDelete: true);
        }
    }
}
