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
            List<string> primaryColumnsForTable = new List<string>();
            DBEntities entity = new DBEntities();

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                
                DBTable entitronTable = new DBTable();
                if (entity.DbTables.SingleOrDefault(x => x.Id == efTable.Id) == null) //pokud se nenachází id ze schématu v databázi, vytváří se nová tabulka
                {
                    entitronTable.tableName = efTable.Name;
                    entitronTable.Application = e.Application;
                    entitronTable.Create();
                    efTable.Id = entitronTable.tableId.Value; //každé nové tabulce je přiděleno id z databáze, která ho vygeneruje
                }
                else
                {
                    if (entitronTable.tableName != efTable.Name)
                        entitronTable.Rename(efTable.Name);
                }
                foreach (DbColumn efColumn in efTable.Columns)
                {
                    DBColumn entitronColumn = new DBColumn();
                    entitronColumn.type = efColumn.Type;
                    entitronColumn.maxLength = efColumn.ColumnLengthIsMax ? null : (int?)efColumn.ColumnLength;
                    entitronColumn.isUnique = efColumn.Unique;
                    entitronColumn.canBeNull = efColumn.AllowNull;

                    if (entity.DbColumn.SingleOrDefault(x => x.Id == efColumn.Id) == null)
                    {
                        entitronTable.columns.AddToDB(entitronColumn);
                        efColumn.Id = entitronColumn.ColumnId;
                    }
                    else
                    {
                        if (entitronColumn.Name != efColumn.Name)
                            entitronTable.columns.RenameInDB(entitronColumn.Name, efTable.Name);
                        entitronTable.columns.ModifyInDB(entitronColumn);
                    }

                    if (efColumn.PrimaryKey)
                    {
                        primaryColumnsForTable.Add(efColumn.Name);
                    }                        
                }
                entitronTables.Add(entitronTable);
                entitronTable.AddPrimaryKey(primaryColumnsForTable);

                foreach (DbIndex efIndex in efTable.Indices)
                {
                    //TODO porovnat názvy indexů v entitronu s názvy indexů ve schématu...na tomto základě mazat nebo přidávat indexy
                    entitronTable.indices.AddToDB(efIndex.Name, new List<string>(efIndex.ColumnNames.Split(',')));
                }

                //foreach (DbRelation efRelation in dbSchemeCommit.Relations)
                //{
                //TODO přidat do dbrelations name pro název cizího klíče, podle něj určovat zda se bude FK přidávat nebo mazat
                //    DBForeignKey entitronFK = new DBForeignKey();
                //    entitronFK.name;
                //    efRelation.
                //}
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