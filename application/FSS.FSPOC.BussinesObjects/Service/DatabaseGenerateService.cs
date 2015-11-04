using System.Configuration;
//using System.Linq;
using FSS.FSPOC.BussinesObjects.Entities.DatabaseDesigner;
using System.Collections.Generic;

namespace FSS.FSPOC.BussinesObjects.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        /// <summary>
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit)
        {
            List<Entitron.DBTable> entitronTables = new List<Entitron.DBTable>();
            Entitron.DBApp.connectionString = ConfigurationManager.ConnectionStrings["EntitronTestingLocal"].ConnectionString;
            Entitron.DBApp entitronApp = new Entitron.DBApp();
            entitronApp.Name = "EntitronTest1";
            entitronApp.DisplayName = "EntitronTest1";
            foreach (DbTable efTable in dbSchemeCommit.Tables)
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
            /*foreach (DbRelation efRelation in dbSchemeCommit.Relations)
            {
                Entitron.DBForeignKey foreignKey = new Entitron.DBForeignKey();
                foreignKey.sourceTable = entitronTables.Find(t => t.tableName == efRelation.SourceTable.Name);
                foreignKey.targetTable = entitronTables.Find(t => t.tableName == efRelation.TargetTable.Name);
                foreignKey.sourceColumn = efRelation.SourceColumn.Name;
                foreignKey.targetColumn = efRelation.TargetColumn.Name;
                foreignKey.sourceTable.foreignKeys.AddToDB(foreignKey);
            }*/
            entitronApp.SaveChanges();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}