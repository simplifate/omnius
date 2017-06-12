namespace FSS.Omnius.Modules.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDB : DbMigration
    {
        public override void Up()
        {
            /// addConditionGroup
            // delete condition with no parent
            Sql("DELETE FROM[dbo].[TapestryDesigner_ConditionSets] WHERE[TapestryDesignerResourceItem_Id] IS NULL AND[TapestryDesignerWorkflowItem_Id] IS NULL");

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
                    ApplicationId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Master_Applications", t => t.ApplicationId)
                .ForeignKey("dbo.TapestryDesigner_ResourceItems", t => t.TapestryDesignerResourceItemId)
                .ForeignKey("dbo.TapestryDesigner_WorkflowItems", t => t.TapestryDesignerWorkflowItemId)
                .Index(t => t.ResourceMappingPairId)
                .Index(t => t.TapestryDesignerResourceItemId)
                .Index(t => t.TapestryDesignerWorkflowItemId)
                .Index(t => t.ApplicationId);
            AddColumn("dbo.Tapestry_ActionRule", "ConditionGroupId", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", c => c.Int(nullable: true));
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
                AlterColumn("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tapestry_ActionRule", "ConditionGroupId");
            CreateIndex("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId");
            AddForeignKey("dbo.Tapestry_ActionRule", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups", "Id");
            AddForeignKey("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups", "Id", cascadeDelete: true);
            DropColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id");
            DropColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id");

            /// EntitronMeta_applicationName
            DropForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            AddColumn("dbo.Entitron___META", "ApplicationName", c => c.String(maxLength: 50));
            Sql("UPDATE e SET e.ApplicationName = a.Name FROM dbo.Entitron___META e JOIN dbo.Master_Applications a ON e.ApplicationId = a.Id");
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationName", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            DropColumn("dbo.Entitron___META", "ApplicationId");

            /// DontUseIdentityRoles
            DropForeignKey("dbo.Persona_User_Role", "RoleId", "dbo.Persona_AppRoles");
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_User_Role", new[] { "RoleId" });
            DropPrimaryKey("dbo.Persona_User_Role");
            CreateTable(
                "dbo.Persona_Identity_UserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Iden_Role_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Persona_Identity_Roles", t => t.Iden_Role_Id)
                .Index(t => t.UserId)
                .Index(t => t.Iden_Role_Id);
            
            
            CreateTable(
                "dbo.Persona_Identity_Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Persona_User_Role", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Persona_User_Role", "RoleName", c => c.String(nullable: true, maxLength: 50));
            AddColumn("dbo.Persona_User_Role", "ApplicationId", c => c.Int(nullable: true));
                Sql("UPDATE ur SET ur.RoleName = ar.Name, ur.ApplicationId = ar.ApplicationId FROM dbo.Persona_User_Role ur JOIN dbo.Persona_AppRoles ar ON ur.RoleId = ar.Id");
                AlterColumn("dbo.Persona_User_Role", "RoleName", c => c.String(maxLength: 50, nullable: false));
                AlterColumn("dbo.Persona_User_Role", "ApplicationId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Persona_User_Role", "Id");
            CreateIndex("dbo.Persona_User_Role", "UserId");
            CreateIndex("dbo.Persona_User_Role", "RoleName");
            CreateIndex("dbo.Persona_User_Role", "ApplicationId");
            AddForeignKey("dbo.Persona_User_Role", "ApplicationId", "dbo.Master_Applications", "Id", cascadeDelete: true);
            DropColumn("dbo.Persona_User_Role", "RoleId");
        }
        
        public override void Down()
        {
            /// DontUseIdentityRoles
            DropForeignKey("dbo.Persona_Identity_UserRoles", "Iden_Role_Id", "dbo.Persona_Identity_Roles");
            DropForeignKey("dbo.Persona_User_Role", "ApplicationId", "dbo.Master_Applications");
            DropIndex("dbo.Persona_User_Role", new[] { "ApplicationId" });
            DropIndex("dbo.Persona_User_Role", new[] { "RoleName" });
            DropIndex("dbo.Persona_User_Role", new[] { "UserId" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "Iden_Role_Id" });
            DropIndex("dbo.Persona_Identity_UserRoles", new[] { "UserId" });
            DropPrimaryKey("dbo.Persona_User_Role");
            AddColumn("dbo.Persona_User_Role", "RoleId", c => c.Int(nullable: true));
                Sql("UPDATE ur SET ur.RoleId = ar.Id FROM dbo.Persona_User_Role ur JOIN dbo.Persona_AppRoles ar ON ur.RoleName = ar.Name AND ur.ApplicationId = ar.ApplicationId");
                AlterColumn("dbo.Persona_User_Role", "RoleId", c => c.Int(nullable: false));
            DropColumn("dbo.Persona_User_Role", "ApplicationId");
            DropColumn("dbo.Persona_User_Role", "RoleName");
            DropColumn("dbo.Persona_User_Role", "Id");
            DropTable("dbo.Persona_Identity_Roles");
            DropTable("dbo.Persona_Identity_UserRoles");
            AddPrimaryKey("dbo.Persona_User_Role", new[] { "UserId", "RoleId" });
            CreateIndex("dbo.Persona_User_Role", "RoleId");
            CreateIndex("dbo.Persona_User_Role", "UserId");
            AddForeignKey("dbo.Persona_User_Role", "RoleId", "dbo.Persona_AppRoles", "Id", cascadeDelete: true);

            /// EntitronMeta_applicationName
            AddColumn("dbo.Entitron___META", "ApplicationId", c => c.Int());
            Sql("UPDATE e SET e.ApplicationId = a.Id FROM dbo.Entitron___META e JOIN dbo.Master_Applications a ON e.ApplicationName = a.Name");
            DropIndex("dbo.Entitron___META", "UNIQUE_Entitron___META_Name");
            DropColumn("dbo.Entitron___META", "ApplicationName");
            CreateIndex("dbo.Entitron___META", new[] { "ApplicationId", "Name" }, unique: true, name: "UNIQUE_Entitron___META_Name");
            AddForeignKey("dbo.Entitron___META", "ApplicationId", "dbo.Master_Applications", "Id");

            /// addConditionGroup
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerWorkflowItem_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "TapestryDesignerResourceItem_Id", c => c.Int());
            AddColumn("dbo.TapestryDesigner_ConditionSets", "ResourceMappingPair_Id", c => c.Int());
            AddColumn("dbo.Tapestry_ActionRule", "ItemWithConditionId", c => c.Int());
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerWorkflowItemId", "dbo.TapestryDesigner_WorkflowItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "TapestryDesignerResourceItemId", "dbo.TapestryDesigner_ResourceItems");
            DropForeignKey("dbo.TapestryDesigner_ConditionSets", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropForeignKey("dbo.TapestryDesigner_ConditionGroups", "ApplicationId", "dbo.Master_Applications");
            DropForeignKey("dbo.Tapestry_ActionRule", "ConditionGroupId", "dbo.TapestryDesigner_ConditionGroups");
            DropIndex("dbo.TapestryDesigner_ConditionSets", new[] { "ConditionGroupId" });
            DropIndex("dbo.TapestryDesigner_ConditionGroups", new[] { "ApplicationId" });
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
