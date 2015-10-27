using System.Configuration;
//using System.Linq;
//using Entitron;
using FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        /// <summary>
        /// Nejde referencovat Entitron
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit)
        {
            //DBTable.connectionString = GetConnectionString();
            //DBTable.ApplicationName = "ApplicationTest";
            //foreach (var efTable in dbSchemeCommit.Tables)
            //{
            //    var writeTable = new DBTable {tableName = efTable.Name};
            //    foreach (var writeColumn in efTable.Columns.Select(efColumn => new DBColumn
            //    {
            //        canBeNull    = efColumn.AllowNull,
            //        isPrimaryKey = efColumn.PrimaryKey,
            //        isUnique     = efColumn.Unique,
            //        maxLength    = efColumn.ColumnLengthIsMax ? null : (int?) efColumn.ColumnLength,
            //        Name         = efColumn.Name,
            //        type         = efColumn.Type
            //    }))
            //    {
            //        writeTable.columns.AddToDB(writeColumn);
            //    }
            //    writeTable.Create();
            //}
            //DBTable.SaveChanges();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}