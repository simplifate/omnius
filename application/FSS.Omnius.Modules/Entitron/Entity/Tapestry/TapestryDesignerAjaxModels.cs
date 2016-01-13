﻿using System;
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
        public bool IsNew { get; set; }

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
        public bool IsNew { get; set; }

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

        public int ParentMetablockId { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeString => Timestamp.ToString("d. M. yyyy H:mm:ss");

        public BlockToolboxState ToolboxState { get; set; }
        public List<AjaxTapestryDesignerRule> Rules { get; set; }
        public List<int> PortTargets { get; set; }

        public AjaxTapestryDesignerBlockCommit()
        {
            Rules = new List<AjaxTapestryDesignerRule>();
            PortTargets = new List<int>();
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

        public List<AjaxTapestryDesignerProperty> Properties { get; set; }

        public AjaxTapestryDesignerItem()
        {
            Properties = new List<AjaxTapestryDesignerProperty>();
        }
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
}
