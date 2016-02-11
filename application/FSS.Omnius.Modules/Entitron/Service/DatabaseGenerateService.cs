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
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core)
        {
            Entitron e = core.Entitron;

            List<string> deletedTables =
                            e.Application.GetTables()
                                .Select(x => x.tableName)
                                .Except(dbSchemeCommit.Tables.Select(x => x.Name))
                                .ToList();                          //list tabulek které nejsou ve schématu, ale jsou ještě v entitronu

            foreach (string deleteTable in deletedTables)           //mazání všech tabulek z entitronu, které nejsou ve schématu
            {
                e.Application.GetTable(deleteTable).Drop();
            }

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                List<string> primaryColumnsForTable = new List<string>(); //list do kterého se ukládají názvy sloupců které jsou primárním klíčem pro tabulku
                DBTable entitronTable =
                    e.Application.
                    GetTables().
                    SingleOrDefault(x => x.tableId == efTable.Id);

                if (entitronTable == null)                          //pokud se nenachází id ze schématu v databázi, vytváří se nová tabulka
                {
                    entitronTable.tableName = efTable.Name;
                    entitronTable.Application = e.Application;
                    entitronTable.Create();
                    efTable.Id = entitronTable.tableId.Value;       //každé nové tabulce je přiděleno id z databáze, která ho vygeneruje
                }
                else                                                //pokud tabulka existuje v entitronu
                {
                    if (entitronTable.tableName != efTable.Name)    //přejmenování tabulky
                        entitronTable.Rename(efTable.Name);
                }


                foreach (DbColumn efColumn in efTable.Columns)
                {
                    List<int> deletedColumns =
                        entitronTable
                        .columns
                        .Select(x => x.ColumnId)
                        .Except(efTable.Columns.Select(x => x.Id))
                        .ToList();                                  //list všech id sloupců které se nenachází ve schématu, ale v entitronu ano(určené ke smazání)

                    foreach (int columnID in deletedColumns)        //mazání sloupců, které nejsou ve schématu
                    {
                        DBColumn column =
                            entitronTable
                            .columns
                            .SingleOrDefault(x => x.ColumnId == columnID);

                        entitronTable.columns.DropFromDB(column.Name);
                    }

                    DBColumn entitronColumn = entitronTable
                        .columns
                        .SingleOrDefault(x => x.ColumnId == efColumn.Id);

                    if (entitronColumn == null)                     //přidání nových sloupců
                    {
                        entitronTable.columns.AddToDB(entitronColumn);
                        efColumn.Id = entitronColumn.ColumnId;
                    }
                    else                                            //úprava sloupců který mají nějaký změněný atribut
                    {
                        if (entitronColumn.Name != efColumn.Name)
                            entitronTable.columns.RenameInDB(entitronColumn.Name, efTable.Name);

                        if (entitronColumn.canBeNull != efColumn.AllowNull ||
                            entitronColumn.maxLength != efColumn.ColumnLength ||
                            entitronColumn.type != efColumn.Type)   
                            entitronTable.columns.ModifyInDB(entitronColumn);

                        if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique == false)
                            entitronTable.columns.AddUniqueValue(efColumn.Name);

                        if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique)
                            entitronTable.DropConstraint($"UN_Entitron_{e.Application.Name}_{entitronTable.tableName}{entitronColumn.Name}");

                        entitronTable.DropConstraint($"PK_Entitron_{e.Application.Name}_{entitronTable.tableName}{entitronColumn.Name}");
                        entitronTable.columns.AddDefaultValue(efColumn.Name,efColumn.DefaultValue);
                    }

                    if (efColumn.PrimaryKey)                        //zaznamenává všechny sloupce, ze kterých se skládá primární klíč tabulky
                    {
                        primaryColumnsForTable.Add(efColumn.Name);
                    }
                }
                if (entitronTable.primaryKeys != primaryColumnsForTable 
                    && entitronTable.primaryKeys.Count>0)           //pokud v entitronu se nachází klíč, a pokud seznam sloupců ze kterého klíč je tvořen je v entitronu jiný než ve schématu, tak se musí smazat starý klíč a až potom vytvořit nový
                {
                    entitronTable.DropConstraint($"PK_Entitron_{e.Application.Name}_{entitronTable.tableName}", true);  
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }else if (entitronTable.primaryKeys.Count == 0)     //pokud v entitronu primární klíč není, přidá primární klíč, který se skládá z následujících sloupců
                {
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }

                List<string> newIndeces =
                    efTable
                    .Indices
                    .Select(x => x.Name)
                    .Except(entitronTable.indices.Select(x => x.indexName))
                    .ToList();                                      //list názvů indexů které jsou ve schématu, ale ne v entitronu

                List<string> deletedIndeces =
                    entitronTable
                    .indices
                    .Select(x => x.indexName)
                    .Except(efTable.Indices.Select(x => x.Name))
                    .ToList();                                      //list názvů indexů, které jsou v entitronu, ale už ne ve schématu

                foreach (string indexName in newIndeces)            //přidá do entitronu pouze indexy které se nenacházejí v entitronu
                {
                    DbIndex index =
                        efTable
                        .Indices
                        .SingleOrDefault(x => x.Name == indexName);

                    entitronTable.indices.AddToDB(index.Name, new List<string>(index.ColumnNames.Split(',')));
                }

                foreach (string indexName in deletedIndeces)        //smaže z entitronu pouze indexy které se nenacházejí ve schématu
                {
                    entitronTable.indices.DropFromDB(indexName);
                }
            } //end foreach efTable

            List<DBForeignKey> entitronsFKs = new List<DBForeignKey>();
            foreach (DBTable table in e.Application.GetTables())
            {
                foreach (DBForeignKey key in table.foreignKeys)
                {
                    entitronsFKs.Add(key);
                }
            }
            List<string> newFK =
                dbSchemeCommit
                .Relations
                .Select(x => x.Name)
                .Except(entitronsFKs.Select(x=>x.name))
                .ToList();                                           //list názvů indexů které jsou ve schématu, ale ne v entitronu

            List<string> deletedFK =
                 entitronsFKs
                 .Select(x => x.name)
                .Except(dbSchemeCommit.Relations.Select(x => x.Name))
                .ToList();                                           //list názvů indexů které jsou v entitronu, ale ne ve schématu

            foreach (string fkname in newFK)                         //přidá do entitronu všechny nové cizí klíče
            {
                DbRelation efRelation = dbSchemeCommit.Relations.SingleOrDefault(x => x.Name == fkname);
                DBForeignKey entitronFK = new DBForeignKey();
                entitronFK.name = efRelation.Name;                   //TODO nějakým způsobem zapisovat název cizího klíče, možná přidat atributy ondelete a onupdate
                entitronFK.sourceTable = e.Application.GetTables().SingleOrDefault(x => x.tableId == efRelation.RightTable);
                entitronFK.targetTable = e.Application.GetTables().SingleOrDefault(x => x.tableId == efRelation.LeftTable);
                entitronFK.sourceColumn = entitronFK.sourceTable.columns.SingleOrDefault(c => c.ColumnId == efRelation.RightColumn).Name;
                entitronFK.sourceColumn = entitronFK.targetTable.columns.SingleOrDefault(c => c.ColumnId == efRelation.LeftColumn).Name;

                entitronFK.sourceTable.foreignKeys.AddToDB(entitronFK);
            }

            foreach (string fkey in deletedFK)
            {
                DBForeignKey foreignKeyForDrop = entitronsFKs.SingleOrDefault(x => x.name == fkey);
                foreignKeyForDrop.sourceTable.DropConstraint(foreignKeyForDrop.name);
            }

            e.Application.SaveChanges();
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}