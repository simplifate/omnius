using System;
using System.Collections.Generic;

namespace FSPOC.Models.Tapestry
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
    public class Commit
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual ICollection<MetaBlock> MetaBlocks { get; set; }

        public Commit()
        {
            MetaBlocks = new List<MetaBlock>();
        }
    }
    public class MetaBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<MetaBlock> MetaBlocks { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
        public int PositionX { get; set; } // For visual representation in Overview
        public int PositionY { get; set; }

        public MetaBlock ParentMetaBlock { get; set; }
        public Commit ParentCommit { get; set; }

        public MetaBlock()
        {
            MetaBlocks = new List<MetaBlock>();
            Blocks = new List<Block>();
        }
    }
    public class Block
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public virtual ICollection<Rule> Rules { get; set; }
        public BlockToolboxState ToolboxState { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual MetaBlock ParentMetaBlock { get; set; }

        public Block()
        {
            Rules = new List<Rule>();
        }
    }
    public class Rule
    {
        public int Id { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public virtual Block ParentBlock { get; set; }

        public Rule()
        {
            Items = new List<Item>();
        }
    }
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

        public Block AssociatedBlock { get; set; }

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
    public class MenuItem
    {
        public int Id { get; set; }
        public int ItemReferenceId { get; set; }
        public int ItemType { get; set; }
        public string DialogType { get; set; }
        public bool IsDataSource { get; set; }
    }
    public class Item
    {
        public int Id { get; set; }
        public int ItemReferenceId { get; set; }
        public int ItemType { get; set; }
        public string DialogType { get; set; }
        public bool IsDataSource { get; set; }
        public virtual ICollection<Item> Inputs { get; set; }
        public Item Output { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public Logic.ConditionSet ConditionSet { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual Rule ParentRule { get; set; }

        public Item()
        {
            Inputs = new List<Item>();
            Properties = new List<Property>();
        }
    }
    public class Property
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Item ParentItem { get; set; }
    }
}
