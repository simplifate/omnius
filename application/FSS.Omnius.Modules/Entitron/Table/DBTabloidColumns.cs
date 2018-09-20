using System;
using System.Collections.Generic;


namespace FSS.Omnius.Modules.Entitron
{
    using FSS.Omnius.Modules.Entitron.Entity.CORE;
    using Sql;

    public class DBTabloidColumns : List<DBColumn>
    {
        protected DBTabloid _table;
        public DBTabloid Table
        {
            get
            {
                return _table;
            }
        }
        
        public DBTabloidColumns(DBTabloid tabloid)
        {
            _table = tabloid;

            // if table exists - get columns
            if (tabloid.isInDB)
            {
                SqlQuery_Column_List query = new SqlQuery_Column_List() { application = Table.Application, tabloid = Table };

                foreach (DBItem i in query.ExecuteWithRead())
                {
                    DataType type = DataType.ByMSSQLName((string)i["typeName"]);
                    string defaults = Convert.ToString(i["default"]);
                    string name = (string)i["name"];
                    DBColumn column = new DBColumn()
                    {
                        ColumnId = Convert.ToInt32(i["column_id"]),
                        Name = name,
                        type = type,
                        maxLength = Convert.ToInt32(type.Limitation == 2 ? i["precision"] : (Int16)i["max_length"] / 2),
                        scale = Convert.ToInt32(i["scale"]),
                        canBeNull = (bool)i["is_nullable"],
                        isUnique = (bool)i["is_unique"],
                        Default = DBDefault.Create(Table, name, defaults)
                    };

                    Add(column);
                }
            }
        }
    }
}
