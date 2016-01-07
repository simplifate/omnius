using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Table("TapestryDesigner_Apps")]
    public class TapestryDesignerApp
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual TapestryDesignerMetablock RootMetablock { get; set; }
    }
    [Table("TapestryDesigner_Metablocks")]
    public class TapestryDesignerMetablock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<TapestryDesignerMetablock> Metablocks { get; set; }
        public virtual ICollection<TapestryDesignerBlock> Blocks { get; set; }
        public int PositionX { get; set; } // For visual representation in Overview
        public int PositionY { get; set; }

        public TapestryDesignerMetablock ParentMetablock { get; set; }
        public TapestryDesignerApp ParentApp { get; set; }

        public TapestryDesignerMetablock()
        {
            Metablocks = new List<TapestryDesignerMetablock>();
            Blocks = new List<TapestryDesignerBlock>();
        }
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

        public virtual ICollection<TapestryDesignerBlockCommit> BlockCommits { get; set; }

        public virtual TapestryDesignerMetablock ParentMetablock { get; set; }

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
        public virtual ICollection<TapestryDesignerRule> Rules { get; set; }

        public virtual TapestryDesignerBlock ParentBlock { get; set; }

        public TapestryDesignerBlockCommit()
        {
            Rules = new List<TapestryDesignerRule>();
        }
    }
    [Table("TapestryDesigner_Rules")]
    public class TapestryDesignerRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<TapestryDesignerItem> Items { get; set; }
        public virtual ICollection<TapestryDesignerOperator> Operators { get; set; }
        public virtual ICollection<TapestryDesignerConnection> Connections { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual TapestryDesignerBlockCommit ParentBlockCommit { get; set; }

        public TapestryDesignerRule()
        {
            Items = new List<TapestryDesignerItem>();
            Operators = new List<TapestryDesignerOperator>();
            Connections = new List<TapestryDesignerConnection>();
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
    [Table("TapestryDesigner_Items")]
    public class TapestryDesignerItem
    {
        public int Id { get; set; }
        public int ItemReferenceId { get; set; }
        public string Label { get; set; }
        public string TypeClass { get; set; }
        public string DialogType { get; set; }
        public bool IsDataSource { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual TapestryDesignerRule ParentRule { get; set; }

        public TapestryDesignerItem()
        {
        }
    }
    [Table("TapestryDesigner_Operators")]
    public class TapestryDesignerOperator
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual TapestryDesignerRule ParentRule { get; set; }

        public TapestryDesignerOperator()
        {
        }
    }
    [Table("TapestryDesigner_Connections")]
    public class TapestryDesignerConnection
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int SourceSlot { get; set; }
        public int Target { get; set; }
    }
}
