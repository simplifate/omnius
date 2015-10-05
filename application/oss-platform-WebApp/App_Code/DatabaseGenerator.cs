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
                Entitron.DBTable writeTable = new Entitron.DBTable();
                writeTable.tableName = efTable.Name;
                foreach (DbColumn efColumn in efTable.Columns)
                {
                    Entitron.DBColumn writeColumn = new Entitron.DBColumn();
                    writeColumn.Name = efColumn.Name;
                    writeColumn.isPrimaryKey = efColumn.PrimaryKey;
                    writeColumn.isUnique = efColumn.Unique;
                    writeColumn.canBeNull = efColumn.AllowNull;
                    writeColumn.maxLength = efColumn.ColumnLengthIsMax ? null : (int?)efColumn.ColumnLength;
                    writeColumn.type = efColumn.Type;
                    writeTable.columns.AddToDB(writeColumn);
                }
                writeTable.Create();
            }
            Entitron.DBTable.SaveChanges();
        }

        public DatabaseGenerator()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}
