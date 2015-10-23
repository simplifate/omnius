using System.Collections.Generic;
using FSPOC.Models;

namespace FSPOC
{
    public class DatabaseGenerator
    {
        private string connectionString;

        public void GenerateFrom(DbSchemeCommit scheme)
        {
            List<DbTable> schemeTables = new List<DbTable>(scheme.Tables);
            List<Entitron.DBTable> entitronTables = new List<Entitron.DBTable>();
            Entitron.DBApp.connectionString = connectionString;
            Entitron.DBApp entitronApp = new Entitron.DBApp();
            entitronApp.Name = "EntitronTest1";
            foreach (DbTable efTable in scheme.Tables)
            {
                Entitron.DBTable entitronTable = new Entitron.DBTable();
                entitronTable.tableName = efTable.Name;
                entitronTable.Application = entitronApp;

                foreach (DbColumn efColumn in efTable.Columns)
                {
                    Entitron.DBColumn entitronColumn = new Entitron.DBColumn();
                    entitronColumn.Name = efColumn.Name;
                    entitronColumn.type = efColumn.Type;
                    entitronColumn.maxLength = efColumn.ColumnLengthIsMax ? null : (int?)efColumn.ColumnLength;
                    entitronColumn.isUnique = efColumn.Unique;
                    entitronColumn.canBeNull = efColumn.AllowNull;

                    entitronTable.columns.Add(entitronColumn);
                    if (efColumn.PrimaryKey)
                        entitronTable.primaryKeys.Add(efColumn.Name);
                }
                entitronTables.Add(entitronTable);
                entitronTable.Create();
                foreach (DbIndex efIndex in efTable.Indices)
                {
                    entitronTable.indices.AddToDB(efIndex.Name, new List<string>(efIndex.ColumnNames.Split(',')));
                }
            }
            foreach (DbRelation efRelation in scheme.Relations)
            {
                Entitron.DBForeignKey foreignKey = new Entitron.DBForeignKey();
                foreignKey.sourceTable = entitronTables.Find(t => t.tableName == efRelation.SourceTable.Name);
                foreignKey.targetTable = entitronTables.Find(t => t.tableName == efRelation.TargetTable.Name);
                foreignKey.sourceColumn = efRelation.SourceColumn.Name;
                foreignKey.targetColumn = efRelation.TargetColumn.Name;
                foreignKey.sourceTable.foreignKeys.AddToDB(foreignKey);
            }
            entitronApp.SaveChanges();
        }

        public DatabaseGenerator()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}
