using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using System.Collections.Generic;
using System.Data;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBForeignKey : IEntityMultiple<DBForeignKey>
    {
        public DBForeignKey(DBConnection db)
        {
            _db = db;
        }

        private DBConnection _db;
        
        public DBTable SourceTable { get; set; }
        public DBTable TargetTable { get; set; }
        public string SourceColumn { get; set; }
        public string TargetColumn { get; set; }
        public string OnDelete { get; set; }
        public string OnUpdate { get; set; }

        public void AddToDB()
        {
            _db.Commands.Enqueue(_db.CommandSet.ADD_foreignKey(_db, SourceTable.Name, SourceColumn, TargetTable.Name, TargetColumn, OnUpdate, OnDelete));
        }

        public bool Compare(DBForeignKey item)
        {
            return
                SourceTable == item.SourceTable
                && SourceColumn == item.SourceColumn
                && TargetTable == item.TargetTable
                && TargetColumn == item.TargetColumn;
        }
        public bool Compare(DbRelation designerFK)
        {
            return
                _db.Application.Name == designerFK.DbSchemeCommit.Application.Name
                && SourceTable.Name == designerFK.LeftTable.Name
                && SourceColumn == designerFK.LeftColumn.Name
                && TargetTable.Name == designerFK.RightTable.Name
                && TargetColumn == designerFK.RightColumn.Name;
        }


        public HashSet<DBForeignKey> Load(Tabloid tabloid)
        {
            HashSet<DBForeignKey> result = new HashSet<DBForeignKey>();

            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_foreignKey(_db, tabloid.Name, true, false)))
            {
                while(reader.Read())
                {
                    result.Add(new DBForeignKey(_db)
                    {
                        SourceTable = (tabloid as DBTable),
                        SourceColumn = (string)reader["sourceColumn"],
                        TargetTable = _db.Table(_db.CommandSet.FromRealTableName(_db.Application, (string)reader["targetTable"])),
                        TargetColumn = (string)reader["targetColumn"],
                        OnDelete = (string)reader["onDelete"],
                        OnUpdate = (string)reader["onUpdate"]
                    });
                }
            }

            return result;
        }

        public void RemoveFromDB()
        {
            _db.Commands.Enqueue(_db.CommandSet.DROP_foreignKey(_db, SourceTable.Name, SourceColumn, TargetTable.Name, TargetColumn));
        }

        public override string ToString()
        {
            return _db.CommandSet.ForeignKeyName(_db.Application, SourceTable.Name, SourceColumn, TargetTable.Name, TargetColumn, false);
        }
    }
}
