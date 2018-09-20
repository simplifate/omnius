using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public enum OverviewConnectionEndpointTypes
    {
        LocalBlock = 0,
        LocalMetablock = 1,
        Remote = 2
    }
    public class AjaxTapestryDesignerApp : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AjaxTapestryDesignerMetablock RootMetablock { get; set; }
    }
    public class AjaxTapestryDesignerAppHeader : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxTapestryDesignerMetablock : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ParentMetablockId { get; set; }
        public int MenuOrder { get; set; }
        public int ParentAppId { get; set; }
        public bool IsNew { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsDeleted { get; set; }

        public List<AjaxTapestryDesignerMetablockConnection> Connections { get; set; }
        public List<AjaxTapestryDesignerMetablock> Metablocks { get; set; }
        public List<AjaxTapestryDesignerBlock> Blocks { get; set; }

        public AjaxTapestryDesignerMetablock()
        {
            Connections = new List<AjaxTapestryDesignerMetablockConnection>();
            Metablocks = new List<AjaxTapestryDesignerMetablock>();
            Blocks = new List<AjaxTapestryDesignerBlock>();
        }
    }
    public class AjaxTapestryDesignerMetablockConnection : IEntity
    {
        public int Id { get; set; }
        public int SourceType { get; set; }
        public int TargetType { get; set; }
        public int? SourceId { get; set; }
        public int? TargetId { get; set; }
    }
    public class AjaxTapestryDesignerBlock : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public string AssociatedTableId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsNew { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }
        public int? LockedForUserId { get; set; }
        public List<AjaxTapestryDesignerBlockCommit> BlockCommits { get; set; }

        public AjaxTapestryDesignerBlock()
        {
            BlockCommits = new List<AjaxTapestryDesignerBlockCommit>();
        }
    }
    public class AjaxTapestryDesignerBlockCommit : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int ParentMetablockId { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
        public int? LockedForUserId { get; set; }
        public string LockedForUserName { get; set; }
        

        public List<AjaxTapestryDesignerResourceRule> ResourceRules { get; set; }
        public List<AjaxTapestryDesignerWorkflowRule> WorkflowRules { get; set; }
        public List<int> PortTargets { get; set; }
        public List<int> AssociatedPageIds { get; set; }
        public List<int> AssociatedBootstrapPageIds { get; set; }
        public List<int> AssociatedTableIds { get; set; }
        public List<string> AssociatedTableName { get; set; }
        public string ModelTableName { get; set; }
        public List<string> RoleWhitelist { get; set; }
        public AjaxTapestryDesignerToolboxState ToolboxState { get; set; }

        public AjaxTapestryDesignerBlockCommit()
        {
            ResourceRules = new List<AjaxTapestryDesignerResourceRule>();
            WorkflowRules = new List<AjaxTapestryDesignerWorkflowRule>();
            PortTargets = new List<int>();
            AssociatedPageIds = new List<int>();
            AssociatedBootstrapPageIds = new List<int>();
            RoleWhitelist = new List<string>();
        }
    }


    public class AjaxBlockLockingStatus
    {
       public int lockStatusId { get; set; }
       public string lockedForUserName { get; set; }
    }

    public class AjaxTapestryDesignerBlockCommitHeader : IEntity
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
    }
    public class AjaxTapestryDesignerResourceRule : IEntity
    {
        public int Id { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<AjaxTapestryDesignerResourceItem> ResourceItems { get; set; }
        public List<AjaxTapestryDesignerResourceConnection> Connections { get; set; }

        public AjaxTapestryDesignerResourceRule()
        {
            ResourceItems = new List<AjaxTapestryDesignerResourceItem>();
            Connections = new List<AjaxTapestryDesignerResourceConnection>();
        }
    }
    public class AjaxTapestryDesignerWorkflowRule : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<AjaxTapestryDesignerSwimlane> Swimlanes { get; set; }
        public List<AjaxTapestryDesignerWorkflowConnection> Connections { get; set; }

        public AjaxTapestryDesignerWorkflowRule()
        {
            Swimlanes = new List<AjaxTapestryDesignerSwimlane>();
            Connections = new List<AjaxTapestryDesignerWorkflowConnection>();
        }
    }
    public class AjaxTapestryDesignerResourceItem : IEntity
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string TypeClass { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ActionId { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public int? StateId { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public bool? IsBootstrap { get; set; }
        public int? BootstrapPageId { get; set; }
        public string TableName { get; set; }
        public bool? IsShared { get; set; }
        public string ColumnName { get; set; }
        public List<string> ColumnFilter { get; set; }
        public List<AjaxTapestryDesignerConditionSet> ConditionSets { get; set; }

        public AjaxTapestryDesignerResourceItem()
        {
            ColumnFilter = new List<string>();
            ConditionSets = new List<AjaxTapestryDesignerConditionSet>();
        }
    }
    public class AjaxTapestryDesignerWorkflowItem : IEntity
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public int SwimlaneIndex { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ActionId { get; set; }
        public int? StateId { get; set; }
        public int? TargetId { get; set; }
        public string TargetName { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public bool? IsBootstrap { get; set; }
        public bool? isAjaxAction { get; set; }
        public bool? IsForeachStart { get; set; }
        public bool? IsForeachEnd { get; set; }
        public string Condition { get; set; }
        public string SymbolType { get; set; }
        public bool HasParallelLock { get; set; }
        public int? ParentSubflowId { get; set; }
        public int? ParentForeachId { get; set; }
        public virtual List<AjaxTapestryDesignerConditionSet> ConditionSets { get; set; }

        public AjaxTapestryDesignerWorkflowItem()
        {
            ConditionSets = new List<AjaxTapestryDesignerConditionSet>();
        }
    }
    public class AjaxTapestryDesignerSwimlane : IEntity
    {
        public int Id { get; set; }
        public int SwimlaneIndex { get; set; }
        public int Height { get; set; }
        public List<string> Roles { get; set; }
        public List<AjaxTapestryDesignerWorkflowItem> WorkflowItems { get; set; }
        public List<AjaxTapestryDesignerSubflow> Subflow { get; set; }
        public List<AjaxTapestryDesignerForeach> Foreach { get; set; }

        public AjaxTapestryDesignerSwimlane()
        {
            Roles = new List<string>();
            WorkflowItems = new List<AjaxTapestryDesignerWorkflowItem>();
            Subflow = new List<AjaxTapestryDesignerSubflow>();
            Foreach = new List<AjaxTapestryDesignerForeach>();
        }
    }
    public class AjaxTapestryDesignerSubflow : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<AjaxTapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        public AjaxTapestryDesignerSubflow()
        {
            WorkflowItems = new List<AjaxTapestryDesignerWorkflowItem>();
        }
    }
    public class AjaxTapestryDesignerForeach : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string DataSource { get; set; }
        public string ItemName { get; set; }
        public bool IsParallel { get; set; }

        public List<AjaxTapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        public AjaxTapestryDesignerForeach()
        {
            WorkflowItems = new List<AjaxTapestryDesignerWorkflowItem>();
        }
    }
    public abstract class AjaxTapestryDesignerConnection : IEntity
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int SourceSlot { get; set; }
        public int TargetId { get; set; }
        public int TargetSlot { get; set; }
    }
    public class AjaxTapestryDesignerResourceConnection : AjaxTapestryDesignerConnection
    {
    }
    public class AjaxTapestryDesignerWorkflowConnection : AjaxTapestryDesignerConnection
    {
    }
    public class AjaxTapestryDesignerIdPair : IEntity
    {
        public int TemporaryId { get; set; }
        public int RealId { get; set; }
    }
    public class AjaxTapestryDesignerIdMapping : IEntity
    {
        public List<AjaxTapestryDesignerIdPair> BlockIdPairs { get; set; }
        public List<AjaxTapestryDesignerIdPair> MetablockIdPairs { get; set; }

        public AjaxTapestryDesignerIdMapping()
        {
            BlockIdPairs = new List<AjaxTapestryDesignerIdPair>();
            MetablockIdPairs = new List<AjaxTapestryDesignerIdPair>();
        }
    }
    public class AjaxTapestryDesignerBlockListItem : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxTapestryDesignerBlockList : IEntity
    {
        public List<AjaxTapestryDesignerBlockListItem> ListItems { get; set; }

        public AjaxTapestryDesignerBlockList()
        {
            ListItems = new List<AjaxTapestryDesignerBlockListItem>();
        }
    }
    public class AjaxTapestryDesignerProperty : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class AjaxTapestryDesignerCondition : IEntity
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Relation { get; set; }
        public string Variable { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class AjaxTapestryDesignerConditionSet : IEntity
    {
        public int Id { get; set; }
        public int SetIndex { get; set; }
        public string SetRelation { get; set; }
        public List<AjaxTapestryDesignerCondition> Conditions { get; set; }

        public AjaxTapestryDesignerConditionSet()
        {
            Conditions = new List<AjaxTapestryDesignerCondition>();
        }
    }
    public class AjaxTapestryDesignerToolboxState : IEntity
    {
        public int Id { get; set; }
        public List<AjaxToolboxItem> Actions { get; set; }
        public List<AjaxToolboxItem> Attributes { get; set; }
        public List<AjaxToolboxItem> UiComponents { get; set; }
        public List<AjaxToolboxItem> Roles { get; set; }
        public List<AjaxToolboxItem> States { get; set; }
        public List<AjaxToolboxItem> Targets { get; set; }
        public List<AjaxToolboxItem> Templates { get; set; }
        public List<AjaxToolboxItem> Integrations { get; set; }

        public TapestryDesignerBlockCommit AssociatedBlockCommit { get; set; }

        public AjaxTapestryDesignerToolboxState()
        {
            Actions = new List<AjaxToolboxItem>();
            Attributes = new List<AjaxToolboxItem>();
            UiComponents = new List<AjaxToolboxItem>();
            Roles = new List<AjaxToolboxItem>();
            States = new List<AjaxToolboxItem>();
            Targets = new List<AjaxToolboxItem>();
            Templates = new List<AjaxToolboxItem>();
            Integrations = new List<AjaxToolboxItem>();
        }
    }
    public class AjaxToolboxItem : IEntity
    {
        public int Id { get; set; }
        public string TypeClass { get; set; }
        public string Label { get; set; }
        public int? ActionId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public bool? IsBootstrap { get; set; }
        public int? StateId { get; set; }
        public string TargetName { get; set; }
        public int? TargetId { get; set; }
    }
}
