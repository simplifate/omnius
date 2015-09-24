using FSPOC.Models;

namespace FSPOC
{
    public class DatabaseGenerator
    {
        public void GenerateFrom(DbSchemeCommit scheme)
        {
            // TODO: load real connection string
            Entitron.DBTable.connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Fabio\upstream\OSS-platform\application\oss-platform-WebApp\App_Data\EntitronTesting.mdf;Integrated Security=True";
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
                    writeTable.columns.Add(writeColumn);
                }
                writeTable.Create();
            }
            Entitron.DBTable.SaveChanges();
        }

        public DatabaseGenerator() { }
    }
}