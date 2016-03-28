using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public enum ItemTypes
    {
        Attribute,
        View,
        Action,
        Operator,
        Role,
        State,
        Port
    }
    [Table("TapestryDesigner_Metablocks")]
    public class TapestryDesignerMetablock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; } // For visual representation in Overview
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }

        public virtual ICollection<TapestryDesignerMetablockConnection> Connections { get; set; }
        public virtual ICollection<TapestryDesignerMetablock> Metablocks { get; set; }
        public virtual ICollection<TapestryDesignerBlock> Blocks { get; set; }

        public TapestryDesignerMetablock ParentMetablock { get; set; }
        public Application ParentApp { get; set; }

        [JsonIgnore]
        public int? ParentMetablock_Id { get; set; }

        public TapestryDesignerMetablock()
        {
            Metablocks = new List<TapestryDesignerMetablock>();
            Blocks = new List<TapestryDesignerBlock>();
            Connections = new List<TapestryDesignerMetablockConnection>();
        }
    }
    [Table("TapestryDesigner_MetablocksConnections")]
    public class TapestryDesignerMetablockConnection
    {
        public int Id { get; set; }
        public int SourceType { get; set; }
        public int TargetType { get; set; }
        public int? SourceId { get; set; }
        public int? TargetId { get; set; }
    }
    [Table("TapestryDesigner_Blocks")]
    public class TapestryDesignerBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public int AssociatedTableId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }

        public virtual ICollection<TapestryDesignerBlockCommit> BlockCommits { get; set; }

        public virtual TapestryDesignerMetablock ParentMetablock { get; set; }
        [JsonIgnore]
        public int? ParentMetablock_Id { get; set; }

        public TapestryDesignerBlock()
        {
            BlockCommits = new List<TapestryDesignerBlockCommit>();
        }
    }
    [Table("TapestryDesigner_BlocksCommits")]
    public class TapestryDesignerBlockCommit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");

        //public BlockToolboxState ToolboxState { get; set; }
        public virtual ICollection<TapestryDesignerResourceRule> ResourceRules { get; set; }
        public virtual ICollection<TapestryDesignerWorkflowRule> WorkflowRules { get; set; }
        public string AssociatedPageIds { get; set; }
        public string AssociatedTableIds { get; set; }

        public virtual TapestryDesignerBlock ParentBlock { get; set; }
        [JsonIgnore]
        public int? ParentBlock_Id { get; set; }

        public TapestryDesignerBlockCommit()
        {
            ResourceRules = new List<TapestryDesignerResourceRule>();
            WorkflowRules = new List<TapestryDesignerWorkflowRule>();
        }
    }
    //[Table("TapestryDesigner_Rules")]
    //public class TapestryDesignerRule // TODO: remove after switching to new workflow model
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public int PositionX { get; set; }
    //    public int PositionY { get; set; }
    //    public int Width { get; set; }
    //    public int Height { get; set; }

    //    public virtual ICollection<TapestryDesignerItem> Items { get; set; }
    //    public virtual ICollection<TapestryDesignerOperator> Operators { get; set; }
    //    public virtual ICollection<TapestryDesignerConnection> Connections { get; set; }

    //    public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }

    //    public TapestryDesignerRule()
    //    {
    //        Items = new List<TapestryDesignerItem>();
    //        Operators = new List<TapestryDesignerOperator>();
    //        Connections = new List<TapestryDesignerConnection>();
    //    }
    //}
    [Table("TapestryDesigner_ResourceRules")]
    public class TapestryDesignerResourceRule
    {
        public int Id { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }
        public virtual ICollection<TapestryDesignerResourceConnection> Connections { get; set; }

        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }
        [JsonIgnore]
        public int? ParentBlockCommit_Id { get; set; }

        public TapestryDesignerResourceRule()
        {
            ResourceItems = new List<TapestryDesignerResourceItem>();
            Connections = new List<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowRules")]
    public partial class TapestryDesignerWorkflowRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual ICollection<TapestryDesignerSwimlane> Swimlanes { get; set; }
        public virtual ICollection<TapestryDesignerWorkflowConnection> Connections { get; set; }

        [JsonIgnore]
        public int? ParentBlockCommit_Id { get; set; }
        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }

        public TapestryDesignerWorkflowRule()
        {
            Swimlanes = new List<TapestryDesignerSwimlane>();
            Connections = new List<TapestryDesignerWorkflowConnection>();
        }
    }
    [Table("TapestryDesigner_Swimlanes")]
    public class TapestryDesignerSwimlane
    {
        public int Id { get; set; }
        public int SwimlaneIndex { get; set; }
        public int Height { get; set; }
        public string Roles { get; set; }

        public virtual ICollection<TapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        public virtual TapestryDesignerWorkflowRule ParentWorkflowRule { get; set; }
        [JsonIgnore]
        public int? ParentWorkflowRule_Id { get; set; }

        public TapestryDesignerSwimlane()
        {
            WorkflowItems = new List<TapestryDesignerWorkflowItem>();
        }
    }
    [Table("TapestryDesigner_ResourceItems")]
    public class TapestryDesignerResourceItem
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
        public string ColumnFilter { get; set; }
        public virtual ICollection<TapestryDesignerConditionSet> ConditionSets { get; set; }

        public virtual TapestryDesignerResourceRule ParentRule { get; set; }

        public virtual ICollection<TapestryDesignerResourceConnection> SourceToConnection { get; set; }
        public virtual ICollection<TapestryDesignerResourceConnection> TargetToConnection { get; set; }

        public TapestryDesignerResourceItem()
        {
            ConditionSets = new List<TapestryDesignerConditionSet>();
            SourceToConnection = new HashSet<TapestryDesignerResourceConnection>();
            TargetToConnection = new HashSet<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowItems")]
    public partial class TapestryDesignerWorkflowItem
    {
        public int Id { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual TapestryDesignerSwimlane ParentSwimlane { get; set; }

        public string Label { get; set; }
        public int? ActionId { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public int? StateId { get; set; }
        public int? TargetId { get; set; }
        public string ComponentId { get; set; }
        public bool? isAjaxAction { get; set; }
        public string Condition { get; set; }

        public virtual ICollection<TapestryDesignerWorkflowConnection> SourceToConnection { get; set; }
        public virtual ICollection<TapestryDesignerWorkflowConnection> TargetToConnection { get; set; }

        public TapestryDesignerWorkflowItem()
        {
            SourceToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
            TargetToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
        }
    }
    [Table("TapestryDesigner_BlockToolboxStates")]
    public class BlockToolboxState
    {
        public int Id { get; set; }
        public virtual ICollection<MenuItem> Attributes { get; set; }
        public virtual ICollection<MenuItem> Views { get; set; }
        public virtual ICollection<MenuItem> Actions { get; set; }
        public virtual ICollection<MenuItem> Operators { get; set; }
        public virtual ICollection<MenuItem> Roles { get; set; }
        public virtual ICollection<MenuItem> States { get; set; }
        public virtual ICollection<MenuItem> Ports { get; set; }

        public TapestryDesignerBlockCommit AssociatedBlockCommit { get; set; }

        public BlockToolboxState()
        {
            Attributes = new List<MenuItem>();
            Views = new List<MenuItem>();
            Actions = new List<MenuItem>();
            Operators = new List<MenuItem>();
            Roles = new List<MenuItem>();
            States = new List<MenuItem>();
            Ports = new List<MenuItem>();
        }
    }
    [Table("TapestryDesigner_MenuItems")]
    public class MenuItem
    {
        public int Id { get; set; }
        public int ItemReferenceId { get; set; }
        public int ItemType { get; set; }
        public string DialogType { get; set; }
        public bool IsDataSource { get; set; }
    }
    //[Table("TapestryDesigner_Items")]
    //public class TapestryDesignerItem
    //{
    //    public int Id { get; set; }
    //    public int ItemReferenceId { get; set; }
    //    public string Label { get; set; }
    //    public string TypeClass { get; set; }
    //    public string DialogType { get; set; }
    //    public bool IsDataSource { get; set; }
    //    public int PositionX { get; set; }
    //    public int PositionY { get; set; }

    //    public virtual ICollection<TapestryDesignerProperty> Properties { get; set; }

    //    public virtual TapestryDesignerRule ParentRule { get; set; }

    //    public TapestryDesignerItem()
    //    {
    //        Properties = new List<TapestryDesignerProperty>();
    //    }
    //}
    //[Table("TapestryDesigner_Operators")]
    //public class TapestryDesignerOperator
    //{
    //    public int Id { get; set; }
    //    public string Type { get; set; }
    //    public string DialogType { get; set; }
    //    public int PositionX { get; set; }
    //    public int PositionY { get; set; }

    //    public virtual TapestryDesignerRule ParentRule { get; set; }

    //    public TapestryDesignerOperator()
    //    {
    //    }
    //}
    public abstract class TapestryDesignerConnection
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int SourceSlot { get; set; }
        public int TargetId { get; set; }
        public int TargetSlot { get; set; }
    }
    [Table("TapestryDesigner_WorkflowConnections")]
    public partial class TapestryDesignerWorkflowConnection : TapestryDesignerConnection
    {
        public virtual TapestryDesignerWorkflowItem Source { get; set; }
        public virtual TapestryDesignerWorkflowItem Target { get; set; }

        [JsonIgnore]
        public int WorkflowRuleId { get; set; }
        public virtual TapestryDesignerWorkflowRule WorkflowRule { get; set; }
    }
    [Table("TapestryDesigner_ResourceConnections")]
    public partial class TapestryDesignerResourceConnection : TapestryDesignerConnection
    {        
        public virtual TapestryDesignerResourceItem Source { get; set; }
        public virtual TapestryDesignerResourceItem Target { get; set; }

        [JsonIgnore]
        public int ResourceRuleId { get; set; }
        public virtual TapestryDesignerResourceRule ResourceRule { get; set; }
    }
    [Table("TapestryDesigner_Conditions")]
    public class TapestryDesignerCondition
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Relation { get; set; }
        public string Variable { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    [Table("TapestryDesigner_ConditionSets")]
    public class TapestryDesignerConditionSet
    {
        public int Id { get; set; }
        public int SetIndex { get; set; }
        public string SetRelation { get; set; }
        public virtual ICollection<TapestryDesignerCondition> Conditions { get; set; }

        public TapestryDesignerConditionSet()
        {
            Conditions = new List<TapestryDesignerCondition>();
        }
    }
    //[Table("TapestryDesigner_Properties")]
    //public class TapestryDesignerProperty
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Value { get; set; }
    //}

    public class TapestryDesignerMenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubMenu { get; set; }
        public int MenuOrder { get; set; }
        public int Level { get; set; }
        public bool IsBlock { get; set; }
        public bool IsMetablock { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsInitial { get; set; }
        public string BlockName { get; set; }
    }
}
