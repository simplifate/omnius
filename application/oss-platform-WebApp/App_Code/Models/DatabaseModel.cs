using System;
using System.Collections.Generic;

namespace FSPOC.Models
{
    public enum RelationTypes
    {
        NoRelation = 0,
        OneToOne,
        OnetToMany,
        ManyToOne,
        ManyToMany
    }
    public class DbColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Unique { get; set; }
        public bool AllowNull { get; set; }
        public string Type { get; set; }
        public int ColumnLength { get; set; }
        public bool ColumnLengthIsMax { get; set; }
        public string DefaultValue { get; set; }

        public virtual DbTable DbTable { get; set; }
    }
    public class DbIndex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Unique { get; set; }
        public string ColumnNames { get; set; }

        public virtual DbTable DbTable { get; set; }
    }
    public class DbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual List<DbColumn> Columns { get; set; }
        public virtual List<DbIndex> Indices { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }

        public DbTable()
        {
            Columns = new List<DbColumn>();
            Indices = new List<DbIndex>();
        }
    }
    public class DbRelation
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public virtual DbTable SourceTable { get; set; }
        public virtual DbColumn SourceColumn { get; set; }
        public virtual DbTable TargetTable { get; set; }
        public virtual DbColumn TargetColumn { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
    public class DbView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual DbSchemeCommit DbSchemeCommit { get; set; }
    }
    public class DbSchemeCommit
    {
        public int Id { get; set; }
        public string CommitMessage { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual List<DbTable> Tables { get; set; }
        public virtual List<DbRelation> Relations { get; set; }
        public virtual List<DbView> Views { get; set; }

        public DbSchemeCommit()
        {
            Tables = new List<DbTable>();
            Relations = new List<DbRelation>();
            Views = new List<DbView>();
        }
    }
}
