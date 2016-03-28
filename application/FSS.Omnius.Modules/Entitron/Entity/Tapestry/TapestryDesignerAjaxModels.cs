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
    public class AjaxTapestryDesignerApp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AjaxTapestryDesignerMetablock RootMetablock { get; set; }
    }
    public class AjaxTapestryDesignerAppHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxTapestryDesignerMetablock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ParentMetablockId { get; set; }
        public int MenuOrder { get; set; }
        public bool IsNew { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }

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
    public class AjaxTapestryDesignerMetablockConnection
    {
        public int Id { get; set; }
        public int SourceType { get; set; }
        public int TargetType { get; set; }
        public int? SourceId { get; set; }
        public int? TargetId { get; set; }
    }
    public class AjaxTapestryDesignerBlock
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

        public List<AjaxTapestryDesignerBlockCommit> BlockCommits { get; set; }

        public AjaxTapestryDesignerBlock()
        {
            BlockCommits = new List<AjaxTapestryDesignerBlockCommit>();
        }
    }
    public class AjaxTapestryDesignerBlockCommit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int ParentMetablockId { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");

        public BlockToolboxState ToolboxState { get; set; }
        public List<AjaxTapestryDesignerResourceRule> ResourceRules { get; set; }
        public List<AjaxTapestryDesignerWorkflowRule> WorkflowRules { get; set; }
        public List<int> PortTargets { get; set; }
        public List<int> AssociatedPageIds { get; set; }
        public List<int> AssociatedTableIds { get; set; }
        public List<string> AssociatedTableName { get; set; }

        public AjaxTapestryDesignerBlockCommit()
        {
            ResourceRules = new List<AjaxTapestryDesignerResourceRule>();
            WorkflowRules = new List<AjaxTapestryDesignerWorkflowRule>();
            PortTargets = new List<int>();
            AssociatedPageIds = new List<int>();
        }
    }
    public class AjaxTapestryDesignerBlockCommitHeader
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
    }
    public class AjaxTapestryDesignerResourceRule
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
    public class AjaxTapestryDesignerWorkflowRule
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
    public class AjaxTapestryDesignerResourceItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string TypeClass { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ActionId { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public List<string> ColumnFilter { get; set; }
        public List<AjaxTapestryDesignerConditionSet> ConditionSets { get; set; }

        public AjaxTapestryDesignerResourceItem()
        {
            ColumnFilter = new List<string>();
            ConditionSets = new List<AjaxTapestryDesignerConditionSet>();
        }
    }
    public class AjaxTapestryDesignerWorkflowItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public int SwimlaneIndex { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ActionId { get; set; }
        public int? StateId { get; set; }
        public int? TargetId { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public string ComponentName { get; set; }
        public bool? isAjaxAction { get; set; }
        public string Condition { get; set; }
    }
    public class AjaxTapestryDesignerSwimlane
    {
        public int Id { get; set; }
        public int SwimlaneIndex { get; set; }
        public int Height { get; set; }
        public List<string> Roles { get; set; }
        public List<AjaxTapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        public AjaxTapestryDesignerSwimlane()
        {
            Roles = new List<string>();
            WorkflowItems = new List<AjaxTapestryDesignerWorkflowItem>();
        }
    }
    public abstract class AjaxTapestryDesignerConnection
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
    public class AjaxTapestryDesignerIdPair
    {
        public int TemporaryId { get; set; }
        public int RealId { get; set; }
    }
    public class AjaxTapestryDesignerIdMapping
    {
        public List<AjaxTapestryDesignerIdPair> BlockIdPairs { get; set; }
        public List<AjaxTapestryDesignerIdPair> MetablockIdPairs { get; set; }

        public AjaxTapestryDesignerIdMapping()
        {
            BlockIdPairs = new List<AjaxTapestryDesignerIdPair>();
            MetablockIdPairs = new List<AjaxTapestryDesignerIdPair>();
        }
    }
    public class AjaxTapestryDesignerBlockListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxTapestryDesignerBlockList
    {
        public List<AjaxTapestryDesignerBlockListItem> ListItems { get; set; }

        public AjaxTapestryDesignerBlockList()
        {
            ListItems = new List<AjaxTapestryDesignerBlockListItem>();
        }
    }
    public class AjaxTapestryDesignerProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class AjaxTapestryDesignerCondition
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Relation { get; set; }
        public string Variable { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class AjaxTapestryDesignerConditionSet
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
}
