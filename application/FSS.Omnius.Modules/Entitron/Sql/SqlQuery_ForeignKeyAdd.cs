using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSS.Omnius.Modules.Entitron.Sql
{
    class SqlQuery_ForeignKeyAdd : SqlQuery_withAppTable
    {
        public DBTable table2 { get; set; }
        public string foreignKey { get; set; }
        public string primaryKey { get; set; }
        public string foreignName { get; set; }
        public string onDelete { get; set; }
        public string onUpdate { get; set; }
        
        protected override void BaseExecution(MarshalByRefObject transaction)
        {
            string update = (onUpdate == "cascade")
                ? " ON UPDATE CASCADE"
                : (onUpdate == "null")
                    ? " ON UPDATE SET NULL"
                    : (onUpdate == "default") ? " ON UPDATE SET DEFAULT" : ""; ;
            string delete = (onDelete == "cascade")
                ? " ON DELETE CASCADE"
                : (onDelete == "null")
                    ? " ON DELETE SET NULL"
                    : (onDelete == "default") ?" ON DELETE SET DEFAULT" : "";
            
            sqlString =
                $"ALTER TABLE [Entitron_{application.Name}_{table.tableName}] ADD CONSTRAINT FK_{foreignName} FOREIGN KEY ({foreignKey}) REFERENCES [Entitron_{application.Name}_{table2.tableName}] ({primaryKey}) " +
                $" {delete} {update} ;";
            
            base.BaseExecution(transaction);
        }

        public override string ToString()
        {
            return string.Format("Add Foreign key to {0} in {1}[{2}]", table2.tableName, table.tableName, application.Name);
        }
    }
}
