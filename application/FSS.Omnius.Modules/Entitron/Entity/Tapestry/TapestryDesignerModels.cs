using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using Master;
    using Mozaic;

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
    [Table("TapestryDesigner_MetaBlocks")]
    public class TapestryDesignerMetablock : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; } // For visual representation in Overview
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<TapestryDesignerMetablockConnection> Connections { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerMetablock> Metablocks { get; set; }
        public virtual ICollection<TapestryDesignerBlock> Blocks { get; set; }

        public int? ParentMetablock_Id { get; set; }
        [ImportExportIgnore]
        public TapestryDesignerMetablock ParentMetablock { get; set; }
        [ImportExportIgnore]
        public int ParentAppId { get; set; }
        [ImportExportIgnore]
        public Application ParentApp { get; set; }
        
        public TapestryDesignerMetablock()
        {
            Metablocks = new List<TapestryDesignerMetablock>();
            Blocks = new List<TapestryDesignerBlock>();
            Connections = new List<TapestryDesignerMetablockConnection>();
        }
    }
    [Table("TapestryDesigner_MetablocksConnections")]
    public class TapestryDesignerMetablockConnection : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int SourceType { get; set; }
        public int TargetType { get; set; }
        public int? SourceId { get; set; }
        public int? TargetId { get; set; }

        [ImportExportIgnore]
        public virtual int TapestryDesignerMetablockId { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerMetablock TapestryDesignerMetablock { get; set; }
    }
    [Table("TapestryDesigner_Blocks")]
    public class TapestryDesignerBlock : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public int AssociatedTableId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<TapestryDesignerBlockCommit> BlockCommits { get; set; }
        public virtual TapestryDesignerToolboxState ToolboxState { get; set; }
        
        [ImportExportIgnore]
        public virtual TapestryDesignerMetablock ParentMetablock { get; set; }
        [ImportExportIgnore]
        public int ParentMetablock_Id { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerWorkflowItem> TargetFor { get; set; }

        public TapestryDesignerBlock()
        {
            BlockCommits = new List<TapestryDesignerBlockCommit>();
        }
    }
    [Table("TapestryDesigner_BlocksCommits")]
    public class TapestryDesignerBlockCommit : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public string ModelTableName { get; set; }
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
        public string RoleWhitelist { get; set; }

        [ImportExportIgnore]
        public virtual TapestryDesignerBlock ParentBlock { get; set; }
        [ImportExportIgnore]
        public int ParentBlock_Id { get; set; }

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
    public class TapestryDesignerResourceRule : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }
        public virtual ICollection<TapestryDesignerResourceConnection> Connections { get; set; }

        [ImportExportIgnore]
        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }
        [ImportExportIgnore]
        public int ParentBlockCommit_Id { get; set; }

        public TapestryDesignerResourceRule()
        {
            ResourceItems = new List<TapestryDesignerResourceItem>();
            Connections = new List<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowRules")]
    public partial class TapestryDesignerWorkflowRule : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual ICollection<TapestryDesignerSwimlane> Swimlanes { get; set; }
        public virtual ICollection<TapestryDesignerWorkflowConnection> Connections { get; set; }

        [ImportExportIgnore]
        public int ParentBlockCommit_Id { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }

        public TapestryDesignerWorkflowRule()
        {
            Swimlanes = new List<TapestryDesignerSwimlane>();
            Connections = new List<TapestryDesignerWorkflowConnection>();
        }
    }
    [Table("TapestryDesigner_Swimlanes")]
    public class TapestryDesignerSwimlane : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int SwimlaneIndex { get; set; }
        public int Height { get; set; }
        public string Roles { get; set; }
        
        public virtual ICollection<TapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        [ImportExportIgnore]
        public virtual TapestryDesignerWorkflowRule ParentWorkflowRule { get; set; }
        [ImportExportIgnore]
        public int ParentWorkflowRule_Id { get; set; }

        public TapestryDesignerSwimlane()
        {
            WorkflowItems = new List<TapestryDesignerWorkflowItem>();
        }
    }
    [Table("TapestryDesigner_ResourceItems")]
    public class TapestryDesignerResourceItem : IEntity
    {
        [ImportIgnore]
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
        [ImportExportIgnore]
        public virtual MozaicEditorPage Page { get; set; }
        public string ComponentName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnFilter { get; set; }
        public virtual ICollection<TapestryDesignerConditionSet> ConditionSets { get; set; }

        [ImportExportIgnore]
        public int ParentRuleId { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerResourceRule ParentRule { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerResourceConnection> SourceToConnection { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerResourceConnection> TargetToConnection { get; set; }

        public TapestryDesignerResourceItem()
        {
            ConditionSets = new List<TapestryDesignerConditionSet>();
            SourceToConnection = new HashSet<TapestryDesignerResourceConnection>();
            TargetToConnection = new HashSet<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowItems")]
    public partial class TapestryDesignerWorkflowItem : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        [ImportExportIgnore]
        public int ParentSwimlaneId { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerSwimlane ParentSwimlane { get; set; }

        public string Label { get; set; }
        public int? ActionId { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public int? StateId { get; set; }
        public string TargetName { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public bool? isAjaxAction { get; set; }
        public string Condition { get; set; }
        public int? TargetId { get; set; }
        public string SymbolType { get; set; }
        public virtual ICollection<TapestryDesignerConditionSet> ConditionSets { get; set; }

        [ImportExportIgnore]
        public TapestryDesignerBlock Target { get; set; }

        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerWorkflowConnection> SourceToConnection { get; set; }
        [ImportExportIgnore]
        public virtual ICollection<TapestryDesignerWorkflowConnection> TargetToConnection { get; set; }

        public TapestryDesignerWorkflowItem()
        {
            SourceToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
            TargetToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
            ConditionSets = new HashSet<TapestryDesignerConditionSet>();
        }
    }
    [Table("TapestryDesigner_ToolboxStates")]
    public class TapestryDesignerToolboxState : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public virtual ICollection<ToolboxItem> Actions { get; set; }
        public virtual ICollection<ToolboxItem> Attributes { get; set; }
        public virtual ICollection<ToolboxItem> UiComponents { get; set; }
        public virtual ICollection<ToolboxItem> Roles { get; set; }
        public virtual ICollection<ToolboxItem> States { get; set; }
        public virtual ICollection<ToolboxItem> Targets { get; set; }
        public virtual ICollection<ToolboxItem> Templates { get; set; }
        public virtual ICollection<ToolboxItem> Integrations { get; set; }

        [ImportExportIgnore]
        public TapestryDesignerBlockCommit AssociatedBlockCommit { get; set; }

        public TapestryDesignerToolboxState()
        {
            Actions = new List<ToolboxItem>();
            Attributes = new List<ToolboxItem>();
            UiComponents = new List<ToolboxItem>();
            Roles = new List<ToolboxItem>();
            States = new List<ToolboxItem>();
            Targets = new List<ToolboxItem>();
            Templates = new List<ToolboxItem>();
            Integrations = new List<ToolboxItem>();
        }
    }
    [Table("TapestryDesigner_ToolboxItems")]
    public class ToolboxItem : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public string TypeClass { get; set; }
        public string Label { get; set; }
        public int? ActionId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public int? StateId { get; set; }
        public string TargetName { get; set; }
        public int? TargetId { get; set; }
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
    public abstract class TapestryDesignerConnection : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int SourceId { get; set; }
        public int SourceSlot { get; set; }
        public int TargetId { get; set; }
        public int TargetSlot { get; set; }
    }
    [Table("TapestryDesigner_WorkflowConnections")]
    public partial class TapestryDesignerWorkflowConnection : TapestryDesignerConnection
    {
        [ImportExportIgnore]
        public virtual TapestryDesignerWorkflowItem Source { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerWorkflowItem Target { get; set; }

        [ImportExportIgnore]
        public int WorkflowRuleId { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerWorkflowRule WorkflowRule { get; set; }
    }
    [Table("TapestryDesigner_ResourceConnections")]
    public partial class TapestryDesignerResourceConnection : TapestryDesignerConnection
    {
        [ImportExportIgnore]
        public virtual TapestryDesignerResourceItem Source { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerResourceItem Target { get; set; }

        [ImportExportIgnore]
        public int ResourceRuleId { get; set; }
        [ImportExportIgnore]
        public virtual TapestryDesignerResourceRule ResourceRule { get; set; }
    }
    [Table("TapestryDesigner_Conditions")]
    public class TapestryDesignerCondition : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int Index { get; set; }
        public string Relation { get; set; }
        public string Variable { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        [Required]
        public virtual TapestryDesignerConditionSet TapestryDesignerConditionSet { get; set; }
    }
    [Table("TapestryDesigner_ConditionSets")]
    public class TapestryDesignerConditionSet : IEntity
    {
        [ImportIgnore]
        public int Id { get; set; }
        public int SetIndex { get; set; }
        public string SetRelation { get; set; }

        public virtual TapestryDesignerResourceItem TapestryDesignerResourceItem { get; set; }
        public virtual TapestryDesignerWorkflowItem TapestryDesignerWorkflowItem { get; set; }
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

    public class TapestryDesignerMenuItem : IEntity
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

        public string rights { get; set; }
    }
}
