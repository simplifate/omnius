using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBIndex : IEntityMultiple<DBIndex>
    {
        public DBIndex(DBConnection db)
        {
            _db = db;
            Columns = new List<string>();
        }

        private DBConnection _db { get; set; }
        public DBTable Table { get; set; }
        public string Name { get; set; }
        public bool isUnique { get; set; }
        public List<string> Columns { get; set; }

        public void AddToDB()
        {
            // if table doesn't exist it creates column with creating table
            if (_db.Exists(Table.Name, ETabloid.ApplicationTables | ETabloid.SystemTables))
                _db.Commands.Enqueue(_db.CommandSet.ADD_index(_db, Table.Name, Columns, isUnique));
        }

        public bool Compare(DBIndex item)
        {
            return Table.Name == item.Table.Name
                && Columns.All(c => item.Columns.Contains(c))
                && item.Columns.All(c => Columns.Contains(c));
        }

        public HashSet<DBIndex> Load(Tabloid tabloid)
        {
            HashSet<DBIndex> result = new HashSet<DBIndex>();

            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_index(_db, tabloid.Name)))
            {
                while (reader.Read())
                {
                    result.Add(new DBIndex(_db)
                    {
                        Table = (DBTable)tabloid,
                        Name = (string)reader["name"],
                        isUnique = Convert.ToBoolean(reader["is_unique"])
                    });
                }
            }

            foreach(DBIndex index in result)
            {
                using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_COLUMNS_index(_db, tabloid.Name, index.Name)))
                {
                    while(reader.Read())
                    {
                        index.Columns.Add(tabloid.Columns.SingleOrDefault(c => c.Name == (string)reader["ColumnName"]).Name);
                    }
                }
            }

            return result;
        }

        public void RemoveFromDB()
        {
            _db.Commands.Enqueue(_db.CommandSet.DROP_index(_db, Table.Name, Name));
        }

        public override string ToString() => $"DBIndex [{Name}][{_db.Application.Name}]";
    }
}
