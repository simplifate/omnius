using System.Collections.Generic;
using FSPOC.Models;

namespace FSPOC
{
    public class DatabaseGenerator
    {
        private string connectionString;

        public void GenerateFrom(DbSchemeCommit scheme)
        {
            Entitron.DBTable.connectionString = connectionString;
            Entitron.DBTable.ApplicationName = "ApplicationTest";
            foreach (DbTable efTable in scheme.Tables)
            {
                Entitron.DBTable entitronTable = new Entitron.DBTable();
                entitronTable.tableName = efTable.Name;
                foreach (DbColumn efColumn in efTable.Columns)
                {
                    Entitron.DBColumn entitronColumn = new Entitron.DBColumn();
                    entitronColumn.Name = efColumn.Name;
                    entitronColumn.isUnique = efColumn.Unique;
                    entitronColumn.canBeNull = efColumn.AllowNull;
                    entitronColumn.maxLength = efColumn.ColumnLengthIsMax ? null : (int?)efColumn.ColumnLength;
                    entitronColumn.type = efColumn.Type;
                    entitronTable.columns.AddToDB(entitronColumn);
                }
                foreach (DbIndex efIndex in efTable.Indices)
                {
                    entitronTable.indices.AddToDB(efIndex.Name, new List<string>(efIndex.ColumnNames.Split(',')));
                }
                entitronTable.Create();
            }
            Entitron.DBTable.SaveChanges();
        }

        public DatabaseGenerator()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}
