using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBColumn : IEntityMultiple<DBColumn>
    {
        public DBColumn(DBConnection db)
        {
            _db = db;
        }

        private DBConnection _db;

        public Tabloid Tabloid { get; set; }
        public string Name { get; set; }
        public DbType Type { get; set; }
        public int MaxLength { get; set; }
        public int Scale { get; set; }
        public bool IsPrimary => (Name == DBCommandSet.PrimaryKey);
        public bool IsNullable { get; set; }
        public bool IsUnique { get; set; }
        public string DefaultValue { get; set; }
        public string AdditionalOptions { get; set; }

        private ColumnMetadata _metadata;
        public ColumnMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = _db.Application.ColumnMetadata.SingleOrDefault(cm => cm.TableName == Tabloid.Name && cm.ColumnName == Name);

                return _metadata;
            }
        }

        public void AddToDB()
        {
            // if table doesn't exist it creates column with creating table
            if (_db.Exists(Tabloid.Name, ETabloid.ApplicationTables | ETabloid.SystemTables))
                _db.Commands.Enqueue(_db.CommandSet.ADD_column(_db, Tabloid.Name, this));
        }

        public bool Compare(DBColumn item)
        {
            return
                Name == item.Name
                && Type == item.Type
                && MaxLength == item.MaxLength
                && Scale == item.Scale
                && IsNullable == item.IsNullable
                && IsUnique == item.IsUnique
                && DefaultValue == item.DefaultValue
                && AdditionalOptions == item.AdditionalOptions;
        }

        public HashSet<DBColumn> Load(Tabloid Tabloid)
        {
            // in cache
            string tabloidRealName = _db.CommandSet.ToRealTableName(_db.Application, Tabloid.Name);
            if (Cache.ContainsKey(tabloidRealName))
                return Cache[tabloidRealName];

            HashSet<DBColumn> result = new HashSet<DBColumn>();
            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_column(_db, Tabloid.Name)))
            {
                while(reader.Read())
                {
                    string defaultValue = null;
                    if (reader["default"] != DBNull.Value)
                    {
                        defaultValue = Convert.ToString(reader["default"]).Trim(new char[] { '(', ')' }); // removes ('value')
                        if ((!defaultValue.StartsWith("'") || !defaultValue.EndsWith("'")) && defaultValue.ToUpper() != "NULL")
                            defaultValue = $"'{defaultValue}'";
                    }

                    result.Add(new DBColumn(_db)
                    {
                        Tabloid = Tabloid,
                        Name = (string)reader["name"],
                        Type = DataType.FromDBName((string)reader["typeName"], _db.Type),
                        MaxLength = reader["max_length"] != DBNull.Value ? Convert.ToInt32(reader["max_length"]) : -1,
                        IsNullable = Convert.ToBoolean(reader["is_nullable"]),
                        IsUnique = Convert.ToBoolean(reader["is_unique"] != DBNull.Value ? reader["is_unique"] : false) || (string)reader["name"] == DBCommandSet.PrimaryKey,
                        DefaultValue = defaultValue
                        //,Scale = Convert.ToInt32(reader["scale"])
                    });
                }
            }

            Cache.Add(tabloidRealName, result);

            return result;
        }

        public void RemoveFromDB()
        {
            _db.Commands.Enqueue(_db.CommandSet.DROP_column(_db, Tabloid.Name, Name));
        }

        public void Rename(string newName)
        {
            string oldName = Name;
            Name = newName;
            _db.Commands.Enqueue(_db.CommandSet.RENAME_column(_db, Tabloid.Name, oldName, this));
        }
        public void Alter(DBColumn newAlter)
        {
            _db.Commands.Enqueue(_db.CommandSet.UPDATE_column(_db, Tabloid.Name, newAlter));

            Name = newAlter.Name;
            Type = newAlter.Type;
            MaxLength = newAlter.MaxLength;
            Scale = newAlter.Scale;
            IsNullable = newAlter.IsNullable;
            IsUnique = newAlter.IsUnique;
            DefaultValue = newAlter.DefaultValue;
            AdditionalOptions = newAlter.AdditionalOptions;
        }
        public void ModifyInDB(DBColumn column, bool reCreateIndex = true, bool reCreateChecks = true, bool reCreateDefault = true, bool reCreateForeignKeys = true)
        {
            // init
            IEnumerable<DBForeignKey> fks;
            IEnumerable<DBIndex> indexes;
            IEnumerable<DBItem> checks;
            string defaultValue;

            // get constraints & FK
            getColumnConstraints(column.Name, out fks, out indexes, out checks, out defaultValue, skipDefault: !reCreateDefault);

            // drop FK & constraints - check, default & index
            dropColumnConstraints(column.Name, fks, indexes, checks, defaultValue);

            /// modify column
            _db.Commands.Enqueue(_db.CommandSet.UPDATE_column(_db, Tabloid.Name, column));
            // change object
            Type = column.Type;
            MaxLength = column.MaxLength;
            Scale = column.Scale;
            IsNullable = column.IsNullable;
            IsUnique = column.IsUnique;
            DefaultValue = column.DefaultValue;
            AdditionalOptions = column.AdditionalOptions;

            /// re-create constraints & FK
            if (reCreateIndex)
                foreach (DBIndex index in indexes)
                {
                    (Tabloid as DBTable).Indexes.Add(index);
                }
            else
                (Tabloid as DBTable).RefreshIndexes();
            if (reCreateChecks)
                foreach (DBItem check in checks)
                {
                    _db.Commands.Enqueue(_db.CommandSet.ADD_check(_db, Tabloid.Name, (string)check["name"], (string)check["definition"]));
                }
            if (reCreateDefault && defaultValue != null)
                _db.Commands.Enqueue(_db.CommandSet.ADD_default(_db, Tabloid.Name, column.Name, defaultValue));
            if (reCreateForeignKeys)
                foreach (DBForeignKey fk in fks)
                {
                    (Tabloid as DBTable).ForeignKeys.Add(fk);
                }
            else
                (Tabloid as DBTable).RefreshFK();
        }
        public void ModifyInDB(DbColumn column, bool reCreateIndex = true, bool reCreateChecks = true, bool reCreateDefault = true, bool reCreateForeignKeys = true)
        {
            DbType type = DataType.FromDesignerName(column.Type);
            ModifyInDB(new DBColumn(_db)
            {
                Tabloid = Tabloid,
                Name = column.Name,
                Type = type,
                MaxLength = column.ColumnLengthIsMax ? DataType.MaxLength(type) : column.ColumnLength,
                IsNullable = column.AllowNull,
                DefaultValue = column.RealDefaultValue(type),
                IsUnique = column.Unique
            }, reCreateIndex, reCreateChecks, reCreateDefault, reCreateForeignKeys);
        }

        public void DropDefault()
        {
            _db.Commands.Enqueue(_db.CommandSet.DROP_default(_db, Tabloid.Name, Name));
        }
        
        public bool Compare(DbColumn column)
        {
            return
                IsNullable == column.AllowNull
                && !(DataType.MaxLength(Type) > 0 && ((column.ColumnLengthIsMax && MaxLength < DataType.MaxLength(Type)) || (!column.ColumnLengthIsMax && MaxLength != column.ColumnLength)))
                && Type == DataType.FromDesignerName(column.Type);
        }
        
        /// <summary>
        /// FK, index, default, check
        /// </summary>
        private void getColumnConstraints(string columnName, out IEnumerable<DBForeignKey> fks, out IEnumerable<DBIndex> indexes, out IEnumerable<DBItem> checks, out string defaultValue, bool skipFK = false, bool skipIndex = false, bool skipDefault = false, bool skipCheck = false)
        {
            fks = skipFK
                ? new List<DBForeignKey>()
                : (Tabloid as DBTable).AllForeignKeys.Where(fk => (fk.SourceTable.Name == Tabloid.Name && fk.SourceColumn == columnName) || (fk.TargetTable.Name == Tabloid.Name && fk.TargetColumn == columnName));

            indexes = skipIndex
                ? new List<DBIndex>()
                : (Tabloid as DBTable).Indexes.Where(i => i.Columns.Contains(columnName));

            checks = skipCheck
                ? new ListJson<DBItem>()
                : _db.ExecuteRead(_db.CommandSet.LIST_check(_db, Tabloid.Name), Tabloid);

            defaultValue = skipDefault
                ? null
                : DefaultValue;
        }
        private void dropColumnConstraints(string columnName, IEnumerable<DBForeignKey> fks, IEnumerable<DBIndex> indexes, IEnumerable<DBItem> checks, string defaultValue)
        {
            foreach (DBForeignKey fk in fks)
            {
                (Tabloid as DBTable).ForeignKeys.Remove(fk);
            }

            foreach (DBIndex index in indexes)
            {
                (Tabloid as DBTable).Indexes.Remove(index);
            }

            foreach (DBItem check in checks)
            {
                _db.Commands.Enqueue(_db.CommandSet.DROP_check(_db, Tabloid.Name, (string)check["name"]));
            }

            if (defaultValue != null)
                _db.Commands.Enqueue(_db.CommandSet.DROP_default(_db, Tabloid.Name, columnName));
        }

        public override string ToString() => Name;

        public static Dictionary<string, HashSet<DBColumn>> Cache = new Dictionary<string, HashSet<DBColumn>>();
    }
}
