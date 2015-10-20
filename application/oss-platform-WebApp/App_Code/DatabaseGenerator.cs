using System.Collections.Generic;
using FSPOC.Models;

namespace FSPOC
{
    public class DatabaseGenerator
    {
        private string connectionString;

        public void GenerateFrom(DbSchemeCommit scheme)
        {
            Entitron.DBApp.connectionString = connectionString;
            Entitron.DBApp entitronApp = new Entitron.DBApp();
            entitronApp.Name = "ApplicationTest";
            foreach (DbTable efTable in scheme.Tables)
            {
                Entitron.DBTable entitronTable = Entitron.DBTable.Create(efTable.Name);
                foreach (DbColumn efColumn in efTable.Columns)
                {
                    entitronTable.columns.AddToDB(efColumn.Name, efColumn.Type, false, false,
                        efColumn.ColumnLengthIsMax ? null : (int?)efColumn.ColumnLength, isUnique: efColumn.Unique, canBeNull: efColumn.AllowNull);
                }
                foreach (DbIndex efIndex in efTable.Indices)
                {
                    entitronTable.indices.AddToDB(efIndex.Name, new List<string>(efIndex.ColumnNames.Split(',')));
                }
            }
            entitronApp.SaveChanges();
        }

        public DatabaseGenerator()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}
