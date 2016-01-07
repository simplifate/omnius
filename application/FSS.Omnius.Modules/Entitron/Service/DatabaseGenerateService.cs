using System.Collections.Generic;
using System.Configuration;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        /// <summary>
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(Application application, DbSchemeCommit dbSchemeCommit)
        {
            List<DBTable> entitronTables = new List<DBTable>();

            CORE.CORE core = new CORE.CORE();
            Entitron e = core.Entitron;
            e.Application = application;

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                DBTable entitronTable = new DBTable();
                entitronTable.tableName = efTable.Name;
                entitronTable.Application = e.Application;

                foreach (DbColumn efColumn in efTable.Columns)
                {
                    DBColumn entitronColumn = new DBColumn();
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
            
            e.Application.SaveChanges();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}