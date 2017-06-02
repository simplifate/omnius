namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addConditionGroup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ResourceMappingPair_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerResourceItem_Id" });
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "TapestryDesignerWorkflowItem_Id" });
            CreateTable(
                "dbo.TapestryDesigner_ConditionGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceMappingPairId = c.Int(),
                        TapestryDesignerResourceItemId = c.Int(),
                        TapestryDesignerWorkflowItemId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.TapestryDesignerResourceItemId)
                .ForeignKey("dbo.TapestryDesigner_WorkflowItems", t => t.TapestryDesignerWorkflowItemId)
                .Index(t => t.ResourceMappingPairId)
                .Index(t => t.TapestryDesignerResourceItemId)
                .Index(t => t.TapestryDesignerWorkflowItemId);
            
            AddColumn("dbo.Tapestry_ActionRule", "ConditionGroupId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", c => c.Int(nullable: false));

            Sql(@"DECLARE @id int;

                SELECT TapestryDesignerWorkflowItem_Id = null, TapestryDesignerResourceItem_Id, count(*) c INTO __temp_rcs FROM dbo.TapestryDesigner_ConditionSets
                WHERE TapestryDesignerResourceItem_Id IS NOT NULL
                GROUP By TapestryDesignerResourceItem_Id;
                WHILE((SELECT count(*) FROM __temp_rcs) > 0)
                BEGIN
                    SELECT TOP 1 @id = TapestryDesignerResourceItem_Id FROM __temp_rcs;
                    INSERT INTO TapestryDesigner_ConditionGroups(TapestryDesignerResourceItemId, ResourceMappingPairId) VALUES(@id, (SELECT ResourceMappingPair_Id FROM dbo.TapestryDesigner_ConditionSets WHERE TapestryDesignerResourceItem_Id = @id AND ResourceMappingPair_Id IS NOT NULL));
                    DELETE FROM __temp_rcs WHERE TapestryDesignerResourceItem_Id = @id;
                END
                DROP TABLE __temp_rcs;

                SELECT TapestryDesignerWorkflowItem_Id, TapestryDesignerResourceItem_Id = NULL, count(*) c INTO __temp_wcs FROM dbo.TapestryDesigner_ConditionSets
                WHERE TapestryDesignerWorkflowItem_Id IS NOT NULL
                GROUP By TapestryDesignerWorkflowItem_Id;
                WHILE((SELECT count(*) FROM __temp_wcs) > 0)
                BEGIN
                    SELECT TOP 1 @id = TapestryDesignerWorkflowItem_Id FROM __temp_wcs;
                    INSERT INTO TapestryDesigner_ConditionGroups(TapestryDesignerWorkflowItemId, ResourceMappingPairId) VALUES(@id, (SELECT ResourceMappingPair_Id FROM dbo.TapestryDesigner_ConditionSets WHERE TapestryDesignerWorkflowItem_Id = @id AND ResourceMappingPair_Id IS NOT NULL));
                    DELETE FROM __temp_wcs WHERE TapestryDesignerWorkflowItem_Id = @id;
                END
                DROP TABLE __temp_wcs;

                UPDATE cs SET cs.ConditionGroupId = g.id FROM TapestryDesigner_ConditionSets cs
                JOIN TapestryDesigner_ConditionGroups g ON cs.TapestryDesignerResourceItem_Id = g.TapestryDesignerResourceItemId OR cs.TapestryDesignerWorkflowItem_Id = g.TapestryDesignerWorkflowItemId;

                UPDATE ar SET ar.ConditionGroupId = g.Id FROM Tapestry_ActionRule ar
                JOIN TapestryDesigner_ConditionGroups g ON ar.ItemWithConditionId = g.TapestryDesignerWorkflowItemId;");

            CreateIndex("dbo.Tapestry_ActionRule", "ConditionGroupId");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId");
            AddForeignKey("dbo.Tapestry_ActionRule", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups", "Id", cascadeDelete: true);
            DropColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId", c => c.Int());
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropForeignKey("dbo.Tapestry_ActionRule", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ConditionGroupId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "TapestryDesignerWorkflowItemId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "TapestryDesignerResourceItemId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "ResourceMappingPairId" });
            DropIndex("dbo.Tapestry_ActionRule", new[] { "ConditionGroupId" });

            Sql(@"UPDATE cs SET cs.ResourceMappingPair_Id = cg.ResourceMappingPairId, cs.TapestryDesignerResourceItem_Id = cg.TapestryDesignerResourceItemId, cs.TapestryDesignerWorkflowItem_Id = cg.TapestryDesignerWorkflowItemId 
                FROM dbo.TapestryDesigner_ConditionSets cs
                JOIN dbo.TapestryDesigner_ConditionGroups cg ON cg.Id = cs.ConditionGroupId

                UPDATE ar SET ar.ItemWithConditionId = cg.TapestryDesignerWorkflowItemId FROM dbo.Tapestry_ActionRule ar
                JOIN dbo.TapestryDesigner_ConditionGroups cg ON cg.Id = ar.ConditionGroupId");

            DropColumn("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId");
            DropColumn("dbo.Tapestry_ActionRule", "ConditionGroupId");
            DropTable("dbo.TapestryDesigner_ConditionGroups");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", "dbo.TapestryDesigner_WorkflowItems", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", "dbo.TapestryDesigner_ResourceItems", "Id");
        }
    }
}
