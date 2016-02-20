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
            e.Application.SaveChanges();

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                List<string> primaryColumnsForTable = new List<string>(); //list do kterého se ukládají názvy sloupců které jsou primárním klíčem pro tabulku
                DBTable entitronTable =
                    e.Application.
                    GetTables().
                    SingleOrDefault(x => x.tableName== efTable.Name);

                if (entitronTable == null)                          //pokud se nenachází id ze schématu v databázi, vytváří se nová tabulka
                {
                    entitronTable = new DBTable();
                    entitronTable.tableName = efTable.Name;
                    entitronTable.Application = e.Application;

                    foreach (DbColumn column in efTable.Columns)
                    {
                        DBColumn col = new DBColumn()
                        {
                            Name = column.Name,
                            isUnique = column.Unique,
                            canBeNull = column.AllowNull,
                            maxLength = column.ColumnLength,
                            type = column.Type
                        };
                        entitronTable.columns.Add(col);
                    }
                    entitronTable.Create();
                    e.Application.SaveChanges();

                        //každé nové tabulce je přiděleno id z databáze, která ho vygeneruje
                }
                else                                                //pokud tabulka existuje v entitronu
                {
                    List<string> deletedColumns =
                            entitronTable
                            .columns
                            .Select(x => x.Name)
                            .Except(efTable.Columns.Select(x => x.Name))
                            .ToList();                                  //list všech id sloupců které se nenachází ve schématu, ale v entitronu ano(určené ke smazání)

                        foreach (string columnName in deletedColumns)        //mazání sloupců, které nejsou ve schématu
                        {
                            DBColumn column =
                                entitronTable
                                .columns
                                .SingleOrDefault(x => x.Name == columnName);

                            entitronTable.columns.DropFromDB(column.Name);
                        }
                        e.Application.SaveChanges();

                    foreach (DbColumn efColumn in efTable.Columns)
                    {

                        DBColumn entitronColumn = entitronTable
                            .columns
                            .SingleOrDefault(x => x.Name == efColumn.Name);

                        if (entitronColumn == null)                     //přidání nových sloupců
                        {
                            entitronColumn = new DBColumn()
                            {
                                Name = efColumn.Name,
                                isUnique = efColumn.Unique,
                                canBeNull = efColumn.AllowNull,
                                maxLength = efColumn.ColumnLength,
                                type = efColumn.Type
                            };
                            entitronTable.columns.AddToDB(entitronColumn);
                            e.Application.SaveChanges();
                            efColumn.Id = entitronColumn.ColumnId;
                        }
                        else                                            //úprava sloupců který mají nějaký změněný atribut
                        {
                            if (entitronColumn.canBeNull != efColumn.AllowNull ||
                                entitronColumn.maxLength != efColumn.ColumnLength ||
                                entitronColumn.type != efColumn.Type)
                                entitronTable.columns.ModifyInDB(entitronColumn.Name, efColumn.Type,efColumn.ColumnLength,entitronColumn.precision,entitronColumn.scale,efColumn.AllowNull);

                            if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique == false)
                            {
                                entitronTable.columns.AddUniqueValue(efColumn.Name);
                                entitronColumn.isUnique = true;
                            }

                            if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique)
                            {
                                entitronTable.DropConstraint($"UN_Entitron_{e.Application.Name}_{entitronTable.tableName}{entitronColumn.Name}");
                                entitronColumn.isUnique = false;
                            }

                            e.Application.SaveChanges();
                        }

                    }

                }
                foreach (DbColumn column in efTable.Columns)
                {
                    if (column.PrimaryKey)                        //zaznamenává všechny sloupce, ze kterých se skládá primární klíč tabulky
                    {
                        primaryColumnsForTable.Add(column.Name);
                    }
                    if (!string.IsNullOrEmpty(column.DefaultValue) && entitronTable.columns.GetDefaults()["DEF_Entitron_" + e.Application.Name + "_" + entitronTable.tableName + column.Name]=="("+column.DefaultValue+")")
                    {
                        entitronTable.columns.AddDefaultValue(column.Name, column.DefaultValue);
                    }
                }
                e.Application.SaveChanges();

                if (entitronTable.primaryKeys != primaryColumnsForTable 
                    && entitronTable.primaryKeys.Count>0)           //pokud v entitronu se nachází klíč, a pokud seznam sloupců ze kterého klíč je tvořen je v entitronu jiný než ve schématu, tak se musí smazat starý klíč a až potom vytvořit nový
                {
                    entitronTable.DropConstraint($"PK_Entitron_{e.Application.Name}_{entitronTable.tableName}", true);  
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }else if (entitronTable.primaryKeys.Count == 0 && primaryColumnsForTable.Count!= 0)     //pokud v entitronu primární klíč není, přidá primární klíč, který se skládá z následujících sloupců
                {
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }
                e.Application.SaveChanges();

                List<string> newIndeces =
                    efTable
                    .Indices
                    .Select(x =>"index_"+x.Name)
                    .Except(entitronTable.indices.Select(x=>x.indexName))
                    .ToList();                                      //list názvů indexů které jsou ve schématu, ale ne v entitronu

                List<string> deletedIndeces =
                    entitronTable
                    .indices
                    .Select(x=>x.indexName)
                    .Except(efTable.Indices.Select(x => "index_" + x.Name))
                    .ToList();                                      //list názvů indexů, které jsou v entitronu, ale už ne ve schématu

                foreach (string indexName in newIndeces)            //přidá do entitronu pouze indexy které se nenacházejí v entitronu
                {
                    DbIndex index =
                        efTable
                        .Indices
                        .SingleOrDefault(x => "index_"+x.Name == indexName);
                    //if (index.Unique) todo dodělat unique pro více sloupců zároveň
                    //{
                    //    List<DbColumn> indexcolumns = efTable.Columns.Where(x => x.Name == index.ColumnNames).ToList();
                    //}
                    entitronTable.indices.AddToDB(index.Name, new List<string>(index.ColumnNames.Split(',')));
                }
                e.Application.SaveChanges();

                foreach (string indexName in deletedIndeces)        //smaže z entitronu pouze indexy které se nenacházejí ve schématu
                {
                    entitronTable.indices.DropFromDB(indexName);
                }
                e.Application.SaveChanges();
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
                entitronFK.sourceTable = e.Application.GetTables().SingleOrDefault(x => x.tableId == efRelation.RightTable);
                entitronFK.targetTable = e.Application.GetTables().SingleOrDefault(x => x.tableId == efRelation.LeftTable);
                entitronFK.sourceColumn = entitronFK.sourceTable.columns.SingleOrDefault(c => c.ColumnId == efRelation.RightColumn).Name;
                entitronFK.sourceColumn = entitronFK.targetTable.columns.SingleOrDefault(c => c.ColumnId == efRelation.LeftColumn).Name;
                entitronFK.name = entitronFK.sourceTable.tableName + entitronFK.sourceColumn + "_" + entitronFK.targetTable.tableName + entitronFK.targetColumn;                  

                entitronFK.sourceTable.foreignKeys.AddToDB(entitronFK);
            }
            e.Application.SaveChanges();

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