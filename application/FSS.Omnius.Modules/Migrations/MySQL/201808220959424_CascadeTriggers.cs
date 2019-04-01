namespace FSS.Omnius.Modules.Migrations.MySQL
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeTriggers : DbMigration
    {
        public override void Up()
        {
            Sql("CREATE TRIGGER `TRG_C:Entitron_DbColumn` BEFORE DELETE ON `Entitron_DbColumn` FOR EACH ROW BEGIN DELETE FROM `Entitron_DbRelation` WHERE `SourceColumnId` = OLD.Id OR `TargetColumnId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Entitron_DbSchemeCommit` BEFORE DELETE ON `Entitron_DbSchemeCommit` FOR EACH ROW BEGIN DELETE FROM `Entitron_DbRelation` WHERE `DbSchemeCommitId` = OLD.Id; DELETE FROM `Entitron_DbTable` WHERE `DbSchemeCommitId` = OLD.Id; DELETE FROM `Entitron_DbView` WHERE `DbSchemeCommitId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Entitron_DbTable` BEFORE DELETE ON `Entitron_DbTable` FOR EACH ROW BEGIN DELETE FROM `Entitron_DbRelation` WHERE `SourceTableId` = OLD.Id OR `TargetTableId` = OLD.Id; DELETE FROM `Entitron_DbColumn` WHERE `DbTableId` = OLD.Id; DELETE FROM `Entitron_DbIndex` WHERE `DbTableId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Hermes_Email_Template` BEFORE DELETE ON `Hermes_Email_Template` FOR EACH ROW BEGIN DELETE FROM `Hermes_Email_Template_Content` WHERE `Hermes_Email_Template_Id` = OLD.Id; DELETE FROM `Hermes_Email_Placeholder` WHERE `Hermes_Email_Template_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Hermes_Incoming_Email` BEFORE DELETE ON `Hermes_Incoming_Email` FOR EACH ROW BEGIN DELETE FROM `Hermes_Incoming_Email_Rule` WHERE `IncomingEmailId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Master_Applications` BEFORE DELETE ON `Master_Applications` FOR EACH ROW BEGIN DELETE FROM `Persona_AppRoles` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Entitron_ColumnMetadata` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Entitron_DbSchemeCommit` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Hermes_Email_Template` WHERE `AppId` = OLD.Id; DELETE FROM `Hermes_Incoming_Email_Rule` WHERE `ApplicationId` = OLD.Id; DELETE FROM `MozaicBootstrap_Page` WHERE `ParentApp_Id` = OLD.Id; DELETE FROM `MozaicEditor_Pages` WHERE `ParentApp_Id` = OLD.Id; DELETE FROM `TapestryDesigner_ConditionGroups` WHERE `ApplicationId` = OLD.Id; DELETE FROM `TapestryDesigner_Metablocks` WHERE `ParentAppId` = OLD.Id; DELETE FROM `Master_UsersApplications` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Tapestry_WorkFlow` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Hermes_Email_Queue` WHERE `ApplicationId` = OLD.Id; DELETE FROM `Cortex_Task` WHERE `AppId` = OLD.Id; DELETE FROM `Persona_ADgroups` WHERE `ApplicationId` = OLD.Id; UPDATE `Persona_Users` SET `DesignAppId` = NULL WHERE `DesignAppId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Mozaic_Pages` BEFORE DELETE ON `Mozaic_Pages` FOR EACH ROW BEGIN UPDATE `Tapestry_Blocks` SET `MozaicPageId` = NULL WHERE `MozaicPageId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:MozaicBootstrap_Page` BEFORE DELETE ON `MozaicBootstrap_Page` FOR EACH ROW BEGIN UPDATE `MozaicBootstrap_Components` SET `ParentComponentId` = NULL WHERE `MozaicBootstrapPageId` = OLD.Id; DELETE FROM `MozaicBootstrap_Components` WHERE `MozaicBootstrapPageId` = OLD.Id; DELETE FROM `Mozaic_Js` WHERE `MozaicBootstrapPageId` = OLD.Id; UPDATE `TapestryDesigner_ResourceItems` SET `BootstrapPageId` = NULL WHERE `BootstrapPageId` = OLD.Id; UPDATE `Tapestry_Blocks` SET `BootstrapPageId` = NULL WHERE `BootstrapPageId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:MozaicEditor_Pages` BEFORE DELETE ON `MozaicEditor_Pages` FOR EACH ROW BEGIN  DELETE FROM `MozaicEditor_Components` WHERE `MozaicEditorPageId` = OLD.Id; UPDATE `TapestryDesigner_ResourceItems` SET `PageId` = NULL WHERE `PageId` = OLD.Id; UPDATE `Tapestry_Blocks` SET `EditorPageId` = NULL WHERE `EditorPageId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Nexus_FileMetadataRecords` BEFORE DELETE ON `Nexus_FileMetadataRecords` FOR EACH ROW BEGIN DELETE FROM `Nexus_CachedFiles` WHERE `Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Nexus_WebDavServers` BEFORE DELETE ON `Nexus_WebDavServers` FOR EACH ROW BEGIN DELETE FROM `Nexus_FileMetadataRecords` WHERE `WebDavServer_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Persona_ADgroups` BEFORE DELETE ON `Persona_ADgroups` FOR EACH ROW BEGIN DELETE FROM `Persona_ADgroup_User` WHERE `ADgroupId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Persona_AppRoles` BEFORE DELETE ON `Persona_AppRoles` FOR EACH ROW BEGIN DELETE FROM `Persona_ActionRuleRights` WHERE `AppRoleId` = OLD.Id; DELETE FROM `Persona_User_Role` WHERE `Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Persona_Users` BEFORE DELETE ON `Persona_Users` FOR EACH ROW BEGIN DELETE FROM `Persona_ADgroup_User` WHERE `UserId` = OLD.Id; DELETE FROM `Persona_UserClaim` WHERE `UserId` = OLD.Id; DELETE FROM `Persona_UserLogin` WHERE `UserId` = OLD.Id; DELETE FROM `Persona_ModuleAccessPermissions` WHERE `UserId` = OLD.Id; DELETE FROM `Persona_User_Role` WHERE `UserId` = OLD.Id; DELETE FROM `Master_UsersApplications` WHERE `UserId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Tapestry_ActionRule` BEFORE DELETE ON `Tapestry_ActionRule` FOR EACH ROW BEGIN DELETE FROM `Tapestry_ActionRule_Action` WHERE `ActionRuleId` = OLD.Id; DELETE FROM `Persona_ActionRuleRights` WHERE `ActionRuleId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Tapestry_Actors` BEFORE DELETE ON `Tapestry_Actors` FOR EACH ROW BEGIN DELETE FROM `Tapestry_ActionRule` WHERE `ActorId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Tapestry_Blocks` BEFORE DELETE ON `Tapestry_Blocks` FOR EACH ROW BEGIN DELETE FROM `Tapestry_ActionRule` WHERE `SourceBlockId` = OLD.Id OR `TargetBlockId` = OLD.Id; DELETE FROM `Tapestry_ResourceMappingPairs` WHERE `BlockId` = OLD.Id; UPDATE `Tapestry_WorkFlow` SET `InitBlockId` = NULL WHERE `InitBlockId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Tapestry_WorkFlow` BEFORE DELETE ON `Tapestry_WorkFlow` FOR EACH ROW BEGIN DELETE FROM `Tapestry_Blocks` WHERE `WorkFlowId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Tapestry_WorkFlow_Types` BEFORE DELETE ON `Tapestry_WorkFlow_Types` FOR EACH ROW BEGIN DELETE FROM `Tapestry_WorkFlow` WHERE `TypeId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_Blocks` BEFORE DELETE ON `TapestryDesigner_Blocks` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_MetablocksConnections` WHERE (`SourceType` = 0 AND `SourceId` = OLD.Id) OR (`TargetType` = 0 AND `TargetId` = OLD.Id); DELETE FROM `TapestryDesigner_BlocksCommits` WHERE `ParentBlock_Id` = OLD.Id; UPDATE `TapestryDesigner_WorkflowItems` SET `TargetId` = NULL WHERE `TargetId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_BlocksCommits` BEFORE DELETE ON `TapestryDesigner_BlocksCommits` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_ToolboxStates` WHERE `AssociatedBlockCommit_Id` = OLD.Id; DELETE FROM `TapestryDesigner_WorkflowRules` WHERE `ParentBlockCommit_Id` = OLD.Id; DELETE FROM `TapestryDesigner_ResourceRules` WHERE `ParentBlockCommit_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_ConditionSets` BEFORE DELETE ON `TapestryDesigner_ConditionSets` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_Conditions` WHERE `TapestryDesignerConditionSet_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_Foreach` BEFORE DELETE ON `TapestryDesigner_Foreach` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_WorkflowItems` WHERE `ParentForeachId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_Metablocks` BEFORE DELETE ON `TapestryDesigner_Metablocks` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_MetablocksConnections` WHERE `TapestryDesignerMetablockId` = OLD.Id OR (`SourceType` = 1 AND `SourceId` = OLD.Id) OR (`TargetType` = 1 AND `TargetId` = OLD.Id); DELETE FROM `TapestryDesigner_Blocks` WHERE `ParentMetablock_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_ResourceItems` BEFORE DELETE ON `TapestryDesigner_ResourceItems` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_ResourceConnections` WHERE `SourceId` = OLD.Id OR `TargetId` = OLD.Id; DELETE FROM `Tapestry_ResourceMappingPairs` WHERE `Source_Id` = OLD.Id OR `Target_Id` = OLD.Id; DELETE FROM `TapestryDesigner_ConditionGroups` WHERE `TapestryDesignerResourceItemId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_ResourceRules` BEFORE DELETE ON `TapestryDesigner_ResourceRules` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_ResourceItems` WHERE `ParentRuleId` = OLD.Id; DELETE FROM `TapestryDesigner_ResourceConnections` WHERE `ResourceRuleId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_Subflow` BEFORE DELETE ON `TapestryDesigner_Subflow` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_WorkflowItems` WHERE `ParentSubflowId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_Swimlanes` BEFORE DELETE ON `TapestryDesigner_Swimlanes` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_WorkflowItems` WHERE `ParentSwimlaneId` = OLD.Id; DELETE FROM `TapestryDesigner_Foreach` WHERE `ParentSwimlaneId` = OLD.Id; DELETE FROM `TapestryDesigner_Subflow` WHERE `ParentSwimlaneId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_ToolboxStates` BEFORE DELETE ON `TapestryDesigner_ToolboxStates` FOR EACH ROW BEGIN UPDATE `TapestryDesigner_Blocks` SET `ToolboxState_Id` = NULL WHERE `ToolboxState_Id` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id1` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id2` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id3` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id4` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id5` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id6` = OLD.Id; DELETE FROM `TapestryDesigner_ToolboxItems` WHERE `TapestryDesignerToolboxState_Id7` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_WorkflowItems` BEFORE DELETE ON `TapestryDesigner_WorkflowItems` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_WorkflowConnections` WHERE `SourceId` = OLD.Id OR `TargetId` = OLD.Id; DELETE FROM `TapestryDesigner_ConditionGroups` WHERE `TapestryDesignerWorkflowItemId` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:TapestryDesigner_WorkflowRules` BEFORE DELETE ON `TapestryDesigner_WorkflowRules` FOR EACH ROW BEGIN DELETE FROM `TapestryDesigner_WorkflowConnections` WHERE `WorkflowRuleId` = OLD.Id; DELETE FROM `TapestryDesigner_Swimlanes` WHERE `ParentWorkflowRule_Id` = OLD.Id; END");
            Sql("CREATE TRIGGER `TRG_C:Watchtower_LogItems` BEFORE DELETE ON `Watchtower_LogItems` FOR EACH ROW BEGIN DELETE FROM `Watchtower_LogItems` WHERE `ParentLogItemId` = OLD.Id; END");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER `TRG_C:Entitron_DbColumn`");
            Sql("DROP TRIGGER `TRG_C:Entitron_DbSchemeCommit`");
            Sql("DROP TRIGGER `TRG_C:Entitron_DbTable`");
            Sql("DROP TRIGGER `TRG_C:Hermes_Email_Template`");
            Sql("DROP TRIGGER `TRG_C:Hermes_Incoming_Email`");
            Sql("DROP TRIGGER `TRG_C:Master_Applications`");
            Sql("DROP TRIGGER `TRG_C:Mozaic_Pages`");
            Sql("DROP TRIGGER `TRG_C:MozaicBootstrap_Page`");
            Sql("DROP TRIGGER `TRG_C:MozaicEditor_Pages`");
            Sql("DROP TRIGGER `TRG_C:Nexus_FileMetadataRecords`");
            Sql("DROP TRIGGER `TRG_C:Nexus_WebDavServers`");
            Sql("DROP TRIGGER `TRG_C:Persona_ADgroups`");
            Sql("DROP TRIGGER `TRG_C:Persona_AppRoles`");
            Sql("DROP TRIGGER `TRG_C:Persona_Users`");
            Sql("DROP TRIGGER `TRG_C:Tapestry_ActionRule`");
            Sql("DROP TRIGGER `TRG_C:Tapestry_Actors`");
            Sql("DROP TRIGGER `TRG_C:Tapestry_Blocks`");
            Sql("DROP TRIGGER `TRG_C:Tapestry_WorkFlow`");
            Sql("DROP TRIGGER `TRG_C:Tapestry_WorkFlow_Types`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_Blocks`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_BlocksCommits`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_ConditionSets`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_Foreach`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_Metablocks`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_ResourceItems`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_ResourceRules`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_Subflow`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_Swimlanes`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_ToolboxStates`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_WorkflowItems`");
            Sql("DROP TRIGGER `TRG_C:TapestryDesigner_WorkflowRules`");
            Sql("DROP TRIGGER `TRG_C:Watchtower_LogItems`");
        }
    }
}
