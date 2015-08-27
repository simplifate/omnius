using System;
using System.Collections.Generic;

namespace FSPOC.Models
{
    public enum RelationTypes
    {
        NoRelation = 0,
        OneToOne,
        OnetToN,
        NToOne,
        MToN
    }
    public class DbColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool PrimaryKey { get; set; }
        public string Type { get; set; }

        public virtual DbTable DbTable { get; set; }
    }
    public class DbTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public virtual ICollection<DbColumn> Columns { get; set; }

        public virtual DbScheme DbScheme { get; set; }

        public DbTable()
        {
            Columns = new List<DbColumn>();
        }
    }
    public class DbRelation
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int LeftTable { get; set; }
        public int LeftColumn { get; set; }
        public int RightTable { get; set; }
        public int RightColumn { get; set; }

        public virtual DbScheme DbScheme { get; set; }
    }
    public class DbScheme
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ICollection<DbTable> Tables { get; set; }
        public virtual ICollection<DbRelation> Relations { get; set; }

        public DbScheme()
        {
            Tables = new List<DbTable>();
            Relations = new List<DbRelation>();
        }
    }
}