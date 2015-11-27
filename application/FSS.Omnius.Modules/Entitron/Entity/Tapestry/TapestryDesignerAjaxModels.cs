using System;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Entitron.Entity.Tapestry
{
    public class AjaxTapestryDesignerApp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AjaxTapestryDesignerMetaBlock> MetaBlocks { get; set; }

        public AjaxTapestryDesignerApp()
        {
            MetaBlocks = new List<AjaxTapestryDesignerMetaBlock>();
        }
    }
    public class AjaxTapestryDesignerAppHeader
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AjaxTapestryDesignerMetaBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public List<AjaxTapestryDesignerMetaBlock> MetaBlocks { get; set; }
        public List<AjaxTapestryDesignerBlock> Blocks { get; set; }

        public AjaxTapestryDesignerMetaBlock()
        {
            MetaBlocks = new List<AjaxTapestryDesignerMetaBlock>();
            Blocks = new List<AjaxTapestryDesignerBlock>();
        }
    }
    public class AjaxTapestryDesignerBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AssociatedTableName { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

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
        public string AssociatedTableName { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");

        public BlockToolboxState ToolboxState { get; set; }
        public List<AjaxTapestryDesignerRule> Rules { get; set; }

        public AjaxTapestryDesignerBlockCommit()
        {
            Rules = new List<AjaxTapestryDesignerRule>();
        }
    }
    public class AjaxTapestryDesignerBlockCommitHeader
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");
    }
    public class AjaxTapestryDesignerRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<AjaxTapestryDesignerItem> Items { get; set; }
        public List<AjaxTapestryDesignerOperator> Operators { get; set; }
        public List<AjaxTapestryDesignerConnection> Connections { get; set; }

        public AjaxTapestryDesignerRule()
        {
            Items = new List<AjaxTapestryDesignerItem>();
            Operators = new List<AjaxTapestryDesignerOperator>();
            Connections = new List<AjaxTapestryDesignerConnection>();
        }
    }
    public class AjaxTapestryDesignerItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string TypeClass { get; set; }
        public bool IsDataSource { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
    public class AjaxTapestryDesignerOperator
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string DialogType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
    public class AjaxTapestryDesignerConnection
    {
        public int Id { get; set; }
        public int Source { get; set; }
        public int SourceSlot { get; set; }
        public int Target { get; set; }
    }
}
