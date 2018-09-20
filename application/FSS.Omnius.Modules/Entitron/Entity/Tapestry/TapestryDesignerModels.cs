using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    using FSS.Omnius.Modules.Entitron.Entity.Entitron;
    using Master;
    using Mozaic;
    using Mozaic.Bootstrap;
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
    public class TapestryDesignerMetablock : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int PositionX { get; set; } // For visual representation in Overview
        public int PositionY { get; set; }
        public int MenuOrder { get; set; }
        public bool IsInitial { get; set; }
        public bool IsInMenu { get; set; }
        public bool IsDeleted { get; set; }
        
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerMetablockConnection> Connections { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerBlock> Blocks { get; set; }
        
        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerMetablock))]
        public int? ParentMetablock_Id { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual TapestryDesignerMetablock ParentMetablock { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerMetablock> Metablocks { get; set; }

        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int ParentAppId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application ParentApp { get; set; }
        
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
        public int Id { get; set; }

        public int SourceType { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerBlock), typeof(TapestryDesignerMetablock), KeyForMultiple_property = "SourceType")]
        public int? SourceId { get; set; }
        public int TargetType { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerBlock), typeof(TapestryDesignerMetablock), KeyForMultiple_property = "TargetType")]
        public int? TargetId { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerMetablock))]
        public int TapestryDesignerMetablockId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerMetablock TapestryDesignerMetablock { get; set; }
    }
    [Table("TapestryDesigner_Blocks")]
    public class TapestryDesignerBlock : IEntity
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
        public bool IsDeleted { get; set; }
        public bool IsChanged { get; set; }
        public int? BuiltBlockId { get; set; } // feel free to remove
        public int? LockedForUserId { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerBlockCommit> BlockCommits { get; set; }
        public virtual TapestryDesignerToolboxState ToolboxState { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerWorkflowItem> TargetFor { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerMetablock))]
        public int ParentMetablock_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerMetablock ParentMetablock { get; set; }

        public TapestryDesignerBlock()
        {
            BlockCommits = new List<TapestryDesignerBlockCommit>();
        }
    }
    [Table("TapestryDesigner_BlocksCommits")]
    public class TapestryDesignerBlockCommit : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public string ModelTableName { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
        [Obsolete]
        public string AssociatedTableIds { get; set; }
        public string RoleWhitelist { get; set; }

        //public BlockToolboxState ToolboxState { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerResourceRule> ResourceRules { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerWorkflowRule> WorkflowRules { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(MozaicEditorPage), MultipleIdInString = true)]
        public string AssociatedPageIds { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(MozaicBootstrapPage), MultipleIdInString = true)]
        public string AssociatedBootstrapPageIds { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerBlock), exportCount = 3, exportOrderColumn = "Timestamp")]
        public int ParentBlock_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerBlock ParentBlock { get; set; }

        public TapestryDesignerBlockCommit()
        {
            ResourceRules = new List<TapestryDesignerResourceRule>();
            WorkflowRules = new List<TapestryDesignerWorkflowRule>();
        }
    }
    [Table("TapestryDesigner_ResourceRules")]
    public class TapestryDesignerResourceRule : IEntity
    {
        public int Id { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerResourceItem> ResourceItems { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerResourceConnection> Connections { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerBlockCommit))]
        public int ParentBlockCommit_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }

        public TapestryDesignerResourceRule()
        {
            ResourceItems = new List<TapestryDesignerResourceItem>();
            Connections = new List<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowRules")]
    public partial class TapestryDesignerWorkflowRule : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerSwimlane> Swimlanes { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerWorkflowConnection> Connections { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerBlockCommit))]
        public int ParentBlockCommit_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
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
        public int Id { get; set; }

        public int SwimlaneIndex { get; set; }
        public int Height { get; set; }
        public string Roles { get; set; }
        
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerWorkflowItem> WorkflowItems { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerSubflow> Subflow { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerForeach> Foreach { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerWorkflowRule))]
        public int ParentWorkflowRule_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerWorkflowRule ParentWorkflowRule { get; set; }

        public TapestryDesignerSwimlane()
        {
            WorkflowItems = new List<TapestryDesignerWorkflowItem>();
            Subflow = new List<TapestryDesignerSubflow>();
            Foreach = new List<TapestryDesignerForeach>();
        }
    }

    [Table("TapestryDesigner_Subflow")]
    public class TapestryDesignerSubflow : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [ImportExport(ELinkType.LinkChild)]
        public ICollection<TapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerSwimlane))]
        public int ParentSwimlaneId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerSwimlane ParentSwimlane { get; set; }

        public TapestryDesignerSubflow()
        {
            WorkflowItems = new List<TapestryDesignerWorkflowItem>();
        }
    }

    [Table("TapestryDesigner_Foreach")]
    public class TapestryDesignerForeach : IEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        [StringLength(100)]
        public string DataSource { get; set; }
        [StringLength(50)]
        public string ItemName { get; set; }
        public bool IsParallel { get; set; }

        [ImportExport(ELinkType.LinkChild)]
        public ICollection<TapestryDesignerWorkflowItem> WorkflowItems { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerSwimlane))]
        public int ParentSwimlaneId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerSwimlane ParentSwimlane { get; set; }

        public TapestryDesignerForeach()
        {
            WorkflowItems = new List<TapestryDesignerWorkflowItem>();
        }
    }


    [Table("TapestryDesigner_ResourceItems")]
    public class TapestryDesignerResourceItem : IEntity
    {
        public int Id { get; set; }

        public string Label { get; set; }
        public string TypeClass { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int? ActionId { get; set; }
        public int? StateId { get; set; }
        public string ComponentName { get; set; }
        public bool? IsBootstrap { get; set; }
        public string TableName { get; set; }
        public bool? IsShared { get; set; }
        public string ColumnName { get; set; }
        public string ColumnFilter { get; set; }

        [ImportExport(ELinkType.LinkOptional, typeof(MozaicEditorPage))]
        public int? PageId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual MozaicEditorPage Page { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(MozaicBootstrapPage))]
        public int? BootstrapPageId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual MozaicBootstrapPage BootstrapPage { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerConditionGroup> ConditionGroups { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerResourceConnection> SourceToConnection { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerResourceConnection> TargetToConnection { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerResourceRule))]
        public int ParentRuleId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerResourceRule ParentRule { get; set; }
        
        public TapestryDesignerResourceItem()
        {
            ConditionGroups = new List<TapestryDesignerConditionGroup>();
            SourceToConnection = new HashSet<TapestryDesignerResourceConnection>();
            TargetToConnection = new HashSet<TapestryDesignerResourceConnection>();
        }
    }
    [Table("TapestryDesigner_WorkflowItems")]
    public partial class TapestryDesignerWorkflowItem : IEntity
    {
        public int Id { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool CommentBottom { get; set; }
        public int? ActionId { get; set; }
        public string InputVariables { get; set; }
        public string OutputVariables { get; set; }
        public int? StateId { get; set; }
        public string TargetName { get; set; }
        [Obsolete]
        public int? PageId { get; set; }
        public string ComponentName { get; set; }
        public bool? IsBootstrap { get; set; }
        public bool? isAjaxAction { get; set; }
        public bool? IsForeachStart { get; set; }
        public bool? IsForeachEnd { get; set; }
        public string Condition { get; set; }
        public string SymbolType { get; set; }
        public bool HasParallelLock { get; set; }

        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerSubflow))]
        public int? ParentSubflowId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual TapestryDesignerSubflow ParentSubflow { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerForeach))]
        public int? ParentForeachId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public virtual TapestryDesignerForeach ParentForeach { get; set; }
        [ImportExport(ELinkType.LinkOptional, typeof(TapestryDesignerBlock))]
        public int? TargetId { get; set; }
        [ImportExport(ELinkType.LinkOptional)]
        public TapestryDesignerBlock Target { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerConditionGroup> ConditionGroups { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerWorkflowConnection> SourceToConnection { get; set; }
        [ImportExport(ELinkType.LinkChild)]
        public virtual ICollection<TapestryDesignerWorkflowConnection> TargetToConnection { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerSwimlane))]
        public int ParentSwimlaneId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerSwimlane ParentSwimlane { get; set; }

        public TapestryDesignerWorkflowItem()
        {
            SourceToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
            TargetToConnection = new HashSet<TapestryDesignerWorkflowConnection>();
            ConditionGroups = new HashSet<TapestryDesignerConditionGroup>();
        }
    }
    [Table("TapestryDesigner_ToolboxStates")]
    public class TapestryDesignerToolboxState : IEntity
    {
        public int Id { get; set; }
        public virtual ICollection<ToolboxItem> Actions { get; set; }
        public virtual ICollection<ToolboxItem> Attributes { get; set; }
        public virtual ICollection<ToolboxItem> UiComponents { get; set; }
        public virtual ICollection<ToolboxItem> Roles { get; set; }
        public virtual ICollection<ToolboxItem> States { get; set; }
        public virtual ICollection<ToolboxItem> Targets { get; set; }
        public virtual ICollection<ToolboxItem> Templates { get; set; }
        public virtual ICollection<ToolboxItem> Integrations { get; set; }
        
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
    public abstract class TapestryDesignerConnection : IEntity
    {
        public int Id { get; set; }
        public virtual int SourceId { get; set; }
        public int SourceSlot { get; set; }
        public virtual int TargetId { get; set; }
        public int TargetSlot { get; set; }
    }
    [Table("TapestryDesigner_WorkflowConnections")]
    public partial class TapestryDesignerWorkflowConnection : TapestryDesignerConnection
    {
        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerWorkflowItem))]
        public override int SourceId { get ; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerWorkflowItem Source { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerWorkflowItem))]
        public override int TargetId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerWorkflowItem Target { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerWorkflowRule))]
        public int WorkflowRuleId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerWorkflowRule WorkflowRule { get; set; }
    }
    [Table("TapestryDesigner_ResourceConnections")]
    public partial class TapestryDesignerResourceConnection : TapestryDesignerConnection
    {
        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerResourceItem))]
        public override int SourceId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerResourceItem Source { get; set; }
        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerResourceItem))]
        public override int TargetId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerResourceItem Target { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerResourceRule))]
        public int ResourceRuleId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerResourceRule ResourceRule { get; set; }
    }
    [Table("TapestryDesigner_Conditions")]
    public partial class TapestryDesignerCondition : IEntity
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Relation { get; set; }
        public string Variable { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        
        [ForeignKey("TapestryDesignerConditionSet")]
        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerConditionSet), skipItem = true)]
        public int TapestryDesignerConditionSet_Id { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerConditionSet TapestryDesignerConditionSet { get; set; }
    }
    [Table("TapestryDesigner_ConditionSets")]
    public partial class TapestryDesignerConditionSet : IEntity
    {
        public int Id { get; set; }

        public int SetIndex { get; set; }
        public string SetRelation { get; set; }

        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerCondition> Conditions { get; set; }

        [ImportExport(ELinkType.Parent, typeof(TapestryDesignerConditionGroup), skipItem = true)]
        public int ConditionGroupId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual TapestryDesignerConditionGroup ConditionGroup { get; set; }

        public TapestryDesignerConditionSet()
        {
            Conditions = new List<TapestryDesignerCondition>();
        }
    }
    [Table("TapestryDesigner_ConditionGroups")]
    public partial class TapestryDesignerConditionGroup : IEntity
    {
        public int Id { get; set; }

        public virtual ICollection<ActionRule> ActionRules { get; set; }
        [Index]
        public virtual int? ResourceMappingPairId { get; set; }
        [ImportExport(ELinkType.Child)]
        public virtual ICollection<TapestryDesignerConditionSet> ConditionSets { get; set; }

        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerResourceItem), skipItem = true, skipPair = new string[] { "TapestryDesignerWorkflowItemId" })]
        public int? TapestryDesignerResourceItemId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerResourceItem TapestryDesignerResourceItem { get; set; }

        [ImportExport(ELinkType.LinkRequired, typeof(TapestryDesignerWorkflowItem), skipItem = true, skipPair = new string[] { "TapestryDesignerResourceItemId" })]
        public int? TapestryDesignerWorkflowItemId { get; set; }
        [ImportExport(ELinkType.LinkRequired)]
        public virtual TapestryDesignerWorkflowItem TapestryDesignerWorkflowItem { get; set; }

        [ImportExport(ELinkType.Parent, typeof(Application))]
        public int? ApplicationId { get; set; }
        [ImportExport(ELinkType.Parent)]
        public virtual Application Application { get; set; }


        public TapestryDesignerConditionGroup()
        {
            ConditionSets = new List<TapestryDesignerConditionSet>();
        }
    }
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
