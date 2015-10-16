namespace Tapestry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tapestry_ActionCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_ActionCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Tapestry_Actions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        MethodName = c.String(nullable: false, maxLength: 100),
                        RequiredAttributes = c.String(maxLength: 200),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_ActionCategories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Tapestry_ActionRole_Action",
                c => new
                    {
                        ActionRoleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        ResultVariables = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => new { t.ActionRoleId, t.ActionId })
                .ForeignKey("dbo.Tapestry_Actions", t => t.ActionId, cascadeDelete: true)
                .ForeignKey("dbo.Tapestry_ActionRoles", t => t.ActionRoleId, cascadeDelete: true)
                .Index(t => t.ActionRoleId)
                .Index(t => t.ActionId);
            
            CreateTable(
                "dbo.Tapestry_ActionRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SourceBlockId = c.Int(nullable: false),
                        TargetBlockId = c.Int(nullable: false),
                        ActorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Actors", t => t.ActorId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.SourceBlockId, cascadeDelete: true)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.TargetBlockId)
                .Index(t => t.SourceBlockId)
                .Index(t => t.TargetBlockId)
                .Index(t => t.ActorId);
            
            CreateTable(
                "dbo.Tapestry_Actors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tapestry_Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ModelName = c.String(maxLength: 50),
                        IsVirtual = c.Boolean(nullable: false),
                        WorkFlowId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_WorkFlows", t => t.WorkFlowId, cascadeDelete: true)
                .Index(t => t.WorkFlowId);
            
            CreateTable(
                "dbo.Tapestry_AttributeRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InputName = c.String(nullable: false, maxLength: 50),
                        AttributeName = c.String(nullable: false, maxLength: 50),
                        BlockId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.BlockId, cascadeDelete: true)
                .Index(t => t.BlockId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlows",
                c => new
                    {
                        InitBlockId = c.Int(nullable: false),
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        ParentId = c.Int(),
                        ApplicationId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InitBlockId)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId, cascadeDelete: true)
                .ForeignKey("dbo.Tapestry_WorkFlows", t => t.ParentId)
                .ForeignKey("dbo.Tapestry_WorkFlow_Types", t => t.TypeId)
                .ForeignKey("dbo.Tapestry_Blocks", t => t.InitBlockId)
                .Index(t => t.InitBlockId)
                .Index(t => t.ParentId)
                .Index(t => t.ApplicationId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.Tapestry_WorkFlow_Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tapestry_ActionCategories", "ParentId", "dbo.Tapestry_ActionCategories");
            DropForeignKey("dbo.Tapestry_Actions", "CategoryId", "dbo.Tapestry_ActionCategories");
            DropForeignKey("dbo.Tapestry_ActionRole_Action", "ActionRoleId", "dbo.Tapestry_ActionRoles");
            DropForeignKey("dbo.Tapestry_ActionRoles", "TargetBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRoles", "SourceBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_Blocks", "WorkFlowId", "dbo.Tapestry_WorkFlows");
            DropForeignKey("dbo.Tapestry_WorkFlows", "InitBlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_WorkFlows", "TypeId", "dbo.Tapestry_WorkFlow_Types");
            DropForeignKey("dbo.Tapestry_WorkFlows", "ParentId", "dbo.Tapestry_WorkFlows");
            DropForeignKey("dbo.Tapestry_WorkFlows", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Tapestry_AttributeRoles", "BlockId", "dbo.Tapestry_Blocks");
            DropForeignKey("dbo.Tapestry_ActionRoles", "ActorId", "dbo.Tapestry_Actors");
            DropForeignKey("dbo.Tapestry_ActionRole_Action", "ActionId", "dbo.Tapestry_Actions");
            DropIndex("dbo.Tapestry_WorkFlows", new[] { "TypeId" });
            DropIndex("dbo.Tapestry_WorkFlows", new[] { "ApplicationId" });
            DropIndex("dbo.Tapestry_WorkFlows", new[] { "ParentId" });
            DropIndex("dbo.Tapestry_WorkFlows", new[] { "InitBlockId" });
            DropIndex("dbo.Tapestry_AttributeRoles", new[] { "BlockId" });
            DropIndex("dbo.Tapestry_Blocks", new[] { "WorkFlowId" });
            DropIndex("dbo.Tapestry_ActionRoles", new[] { "ActorId" });
            DropIndex("dbo.Tapestry_ActionRoles", new[] { "TargetBlockId" });
            DropIndex("dbo.Tapestry_ActionRoles", new[] { "SourceBlockId" });
            DropIndex("dbo.Tapestry_ActionRole_Action", new[] { "ActionId" });
            DropIndex("dbo.Tapestry_ActionRole_Action", new[] { "ActionRoleId" });
            DropIndex("dbo.Tapestry_Actions", new[] { "CategoryId" });
            DropIndex("dbo.Tapestry_ActionCategories", new[] { "ParentId" });
            DropTable("dbo.Tapestry_WorkFlow_Types");
            DropTable("dbo.Tapestry_WorkFlows");
            DropTable("dbo.Tapestry_AttributeRoles");
            DropTable("dbo.Tapestry_Blocks");
            DropTable("dbo.Tapestry_Actors");
            DropTable("dbo.Tapestry_ActionRoles");
            DropTable("dbo.Tapestry_ActionRole_Action");
            DropTable("dbo.Tapestry_Actions");
            DropTable("dbo.Tapestry_ActionCategories");
        }
    }
}
