using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using FSS.Omnius.Modules.Entitron.Queryable.Cond;
using FSS.Omnius.Modules.Entitron.Queryable;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using Newtonsoft.Json.Linq;

namespace FSS.Omnius.Modules.Entitron.DB
{
    public class DBTable : Tabloid, IToJson
    {
        public DBTable(DBConnection db) : base(db)
        {
            _indexes = new EntityManager<DBIndex>(_db, this);
            _foreignKeys = new EntityManager<DBForeignKey>(_db, this);
            _itemsToAdd = new List<DBItem>();
        }
        
        private EntityManager<DBIndex> _indexes;
        public EntityManager<DBIndex> Indexes => _indexes;
        private EntityManager<DBForeignKey> _foreignKeys;
        public EntityManager<DBForeignKey> ForeignKeys => _foreignKeys;
        private List<DBForeignKey> _allForeignKeys;
        public List<DBForeignKey> AllForeignKeys
        {
            get
            {
                if (_allForeignKeys == null)
                {
                    _allForeignKeys = new List<DBForeignKey>();
                    using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_foreignKey(_db, Name, true, true)))
                    {
                        while(reader.Read())
                        {
                            _allForeignKeys.Add(new DBForeignKey(_db)
                            {
                                SourceTable = _db.Table(_db.CommandSet.FromRealTableName(_db.Application, (string)reader["sourceTable"])),
                                SourceColumn = (string)reader["sourceColumn"],
                                TargetTable = _db.Table(_db.CommandSet.FromRealTableName(_db.Application, (string)reader["targetTable"])),
                                TargetColumn = (string)reader["targetColumn"]
                            });
                        }
                    }
                }

                return _allForeignKeys;
            }
        }

        private bool? _isInDB;
        public override bool IsInDB
        {
            get
            {
                if (_isInDB == null)
                {
                    if (string.IsNullOrWhiteSpace(Name))
                        return false;

                    IDbCommand command = _db.CommandSet.EXISTS_table(_db, Name);
                    using (DBReader reader = _db.ExecuteCommand(command))
                    {
                        reader.Read();
                        _isInDB = (int)reader["exists"] == 1;
                    }
                }

                return _isInDB.Value;
            }
        }

        internal List<DBItem> _itemsToAdd;

        public DBTable Create()
        {
            _db.TableCreate(this);

            return this;
        }
        public void Drop()
        {
            _db.TableDrop(this);
        }
        public DBTable Rename(string newName)
        {
            _db.Commands.Enqueue(_db.CommandSet.RENAME_table(_db, Name, newName));
            Name = newName;

            return this;
        }
        public DBTable Truncate()
        {
            _db.TableTruncate(Name);

            return this;
        }

        public int AddGetId(DBItem item)
        {
            if (!item.getAllProperties().Any())
            {
                Watchtower.OmniusWarning.Log($"Try to insert no values to table [{RealName}]", Watchtower.OmniusLogSource.Entitron, _db.Application);
                return -1;
            }

            item.Tabloid = this;
            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.INSERT(_db, Name, item)))
            {
                reader.Read();
                int id = Convert.ToInt32(reader[DBCommandSet.PrimaryKey]);
                item[DBCommandSet.PrimaryKey] = id;
                return id;
            }
        }
        public void Add(DBItem item)
        {
            if (!item.getAllProperties().Any())
            {
                Watchtower.OmniusWarning.Log($"Try to insert no values to table [{RealName}]", Watchtower.OmniusLogSource.Entitron, _db.Application);
                return;
            }

            _itemsToAdd.Add(item);
            _db.SaveTable(this);
        }
        public void AddRange(IEnumerable<DBItem> items)
        {
            int page = 0;
            int batch = 10;
            foreach (DBItem item in items)
                item.Tabloid = this;

            do
            {
                _db.Commands.Enqueue(_db.CommandSet.INSERT_range(_db, Name, items.Skip(page * batch).Take(batch)));
                page++;
            }
            while (page * batch < items.Count());
        }

        public DBTable AddCheck(string checkName, Manager<Condition> where)
        {
            _db.Commands.Enqueue(_db.CommandSet.ADD_check(_db, Name, checkName, where));

            return this;
        }

        public List<string> GetCheckConstraints()
        {
            List<string> checks = new List<string>();
            using (DBReader reader = _db.ExecuteCommand(_db.CommandSet.LIST_check(_db, Name)))
            {
                while (reader.Read())
                {
                    checks.Add(_db.CommandSet.FromCheckName(_db.Application, Name, (string)reader["name"], false));
                }
            }
            
            return checks;
        } 

        public List<string> GetOperators()
        {
            List<string> operators = new List<string>();
            operators.Add("");
            operators.Add("Equal");
            operators.Add("Not Equal");
            operators.Add("Less");
            operators.Add("Less or Equal");
            operators.Add("Greater");
            operators.Add("Greater or Equal");
            operators.Add("Like");
            operators.Add("Not Like");
            operators.Add("Between");
            operators.Add("Not Between");
            operators.Add("Null");
            operators.Add("Not Null");
            operators.Add("In");
            operators.Add("Not In");

            return operators;
        } 

        public DBTable Update(DBItem item, int id)
        {
            _db.Commands.Enqueue(_db.CommandSet.UPDATE(_db, Name, id, item));

            return this;
        }
        public DBTable Delete(DBItem item)
        {
            Manager<Condition> conditions = new Manager<Condition>(_db);
            conditions.i.tabloidName = Name;
            foreach(string columnName in item.getFullColumnNames())
            {
                conditions.Start();
                conditions.i.column = columnName;
                conditions.i.operation_params.Add(item[columnName]);
                conditions.Next();
            }

            _db.Commands.Enqueue(_db.CommandSet.DELETE(_db, Name, conditions));

            return this;
        }
        public DBTable Delete(int itemId)
        {
            Manager<Condition> conditions = new Manager<Condition>(_db);
            conditions.i.tabloidName = Name;
            conditions.i.column = DBCommandSet.PrimaryKey;
            conditions.i.operation_params.Add(itemId);
            conditions.Next();

            _db.Commands.Enqueue(_db.CommandSet.DELETE(_db, Name, conditions));

            return this;
        }
        public Delete Delete()
        {
            return new Delete(_db, Name);
        }
        
        public void RefreshIndexes()
        {
            _indexes = new EntityManager<DBIndex>(_db, this);
        }
        public void RefreshFK()
        {
            _foreignKeys = new EntityManager<DBForeignKey>(_db, this);
            _allForeignKeys = null;
        }

        /// <summary>
        /// Checks check & defaults if changed
        /// </summary>
        public void RefreshConstraints(DBColumn dbColumn, DbColumn designerColumn)
        {
            /// check
            #warning TODO: DbColumn.check

            /// default
            string defaultValue = designerColumn.RealDefaultValue(dbColumn.Type);
            if (string.IsNullOrEmpty(defaultValue) != (dbColumn.DefaultValue == null) || dbColumn.DefaultValue != defaultValue)
            {
                // drop old
                if (dbColumn.DefaultValue != null)
                    _db.Commands.Enqueue(_db.CommandSet.DROP_default(_db, Name, dbColumn.Name));

                // create new
                if (!string.IsNullOrWhiteSpace(defaultValue))
                    _db.Commands.Enqueue(_db.CommandSet.ADD_default(_db, Name, dbColumn.Name, defaultValue));
            }
        }
        
        //public object ConvertValue(DBColumn column, string value)
        //{
        //    object val;
        //    switch (column.Type)
        //    {
        //        case DbType.Int32:
        //            val = Convert.ToInt32(value);
        //            break;
        //        case DbType.Int64:
        //            val = Convert.ToInt64(value);
        //            break;
        //        case DbType.Int16:
        //            val = Convert.ToInt16(value);
        //            break;
        //        case DbType.Byte:
        //            val = Convert.ToByte(value);
        //            break;
        //        case DbType:
        //            val = Convert.ToDecimal(value);
        //            break;
        //        case "smallmoney":
        //            val = Convert.ToDecimal(value);
        //            break;
        //        case "money":
        //            val = Convert.ToDecimal(value);
        //            break;
        //        case "float":
        //            val = Convert.ToDouble(value);
        //            break;
        //        case "real":
        //            val = Convert.ToSingle(value);
        //            break;
        //        case "date":
        //            val = Convert.ToDateTime(value);
        //            break;
        //        case "time":
        //            val = TimeSpan.Parse(value);
        //            break;
        //        case "datetime":
        //            val = Convert.ToDateTime(value);
        //            break;
        //        case "datetime2":
        //            val = Convert.ToDateTime(value);
        //            break;
        //        case "datetimeoffset":
        //            val = DateTimeOffset.Parse(value);
        //            break;
        //        case "timestamp":
        //            val = Convert.ToByte(value);
        //            break;
        //        case "varbinary":
        //            val = Convert.ToByte(value);
        //            break;
        //        case "bit":
        //            val = Convert.ToBoolean(value);
        //            break;
        //        case "binary":
        //            val = Convert.ToByte(value);
        //            break;
        //        case "uniqueidentifier":
        //            val = Guid.Parse(value);
        //            break;
        //        default:
        //            val = value;
        //            break;
        //    }

        //    return val;
        //}

        public JToken ToJson()
        {
            return Select().ToList().ToJson();
        }

        public override string ToString() => $"DBTable [{Name}][{_db.Application.Name}]";
    }
}
