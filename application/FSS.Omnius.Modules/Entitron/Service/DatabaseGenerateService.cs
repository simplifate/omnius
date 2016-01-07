using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
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
            List<DbRelation> entitronRelations = new List<DbRelation>();

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
            foreach(DbRelation efRelation in dbSchemeCommit.Relations)
            {
                DbRelation entitronRelation = new DbRelation();
                entitronRelation.Id = efRelation.Id;
                entitronRelation.RightTable = efRelation.RightTable;
                entitronRelation.LeftTable = efRelation.LeftTable;
                entitronRelation.RightColumn = efRelation.RightColumn;
                entitronRelation.LeftColumn = efRelation.LeftColumn;
                entitronRelation.Type = efRelation.Type;

                entitronRelations.Add(entitronRelation);

                //DBForeignKey entitronFK = new DBForeignKey();
                //entitronFK.sourceTable.tableName = dbSchemeCommit.Tables.SingleOrDefault(x => x.Id == efRelation.RightTable).Name;
                //entitronFK.targetTable.tableName = dbSchemeCommit.Tables.SingleOrDefault(x => x.Id == efRelation.LeftTable).Name;
                //entitronFK.sourceColumn = dbSchemeCommit.Tables.SingleOrDefault(x => x.Id == efRelation.RightTable).Columns.SingleOrDefault(c => c.Id == efRelation.Id).Name;
                //entitronFK.sourceColumn = dbSchemeCommit.Tables.SingleOrDefault(x => x.Id == efRelation.LeftTable).Columns.SingleOrDefault(c => c.Id == efRelation.Id).Name;
            }

            e.Application.SaveChanges();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}