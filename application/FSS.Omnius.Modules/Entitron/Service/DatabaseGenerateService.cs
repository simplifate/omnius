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

            

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                List<string> primaryColumnsForTable = new List<string>(); //list do kterého se ukládají názvy sloupců které jsou primárním klíčem pro tabulku
                DBTable entitronTable =
                    e.Application.
                    GetTables().
                    SingleOrDefault(x => x.tableName.ToLower() == efTable.Name.ToLower());

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
                    foreach (DbColumn efColumn in efTable.Columns)
                    {

                        DBColumn entitronColumn = entitronTable
                            .columns
                            .SingleOrDefault(x => x.Name.ToLower() == efColumn.Name.ToLower());

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
                                entitronTable.columns.ModifyInDB(entitronColumn.Name, efColumn.Type, efColumn.ColumnLength, entitronColumn.precision, entitronColumn.scale, efColumn.AllowNull);

                            if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique == false)
                            {
                                if(!efColumn.PrimaryKey)
                                entitronTable.columns.AddUniqueValue(efColumn.Name);
                            }

                            if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique)
                            {
                                entitronTable.DropConstraint($"UN_Entitron_{e.Application.Name}_{entitronTable.tableName}{entitronColumn.Name}");
                            }

                            e.Application.SaveChanges();
                        }

                    }

                }
                foreach (DbColumn column in efTable.Columns)
                {
                    if (column.PrimaryKey) //zaznamenává všechny sloupce, ze kterých se skládá primární klíč tabulky
                    {
                        primaryColumnsForTable.Add(column.Name);
                    }
                    string def = "DEF_Entitron_" + e.Application.Name + "_" + entitronTable.tableName + column.Name;
                    if (!string.IsNullOrEmpty(column.DefaultValue))
                    {
                        bool isChange = false;
                        foreach (string key in entitronTable.columns.GetDefaults().Keys)
                        {
                            if ((key.ToLower() == def.ToLower()) &&
                                entitronTable.columns.GetDefaults()[key] != "('" + column.DefaultValue + "')")
                            {
                                isChange = true;
                                entitronTable.DropConstraint(key);
                                entitronTable.columns.AddDefaultValue(column.Name.ToLower(), column.DefaultValue);
                                break;
                            }
                            else if((key.ToLower() == def.ToLower()) &&
                                entitronTable.columns.GetDefaults()[key] == "('" + column.DefaultValue + "')")
                            {
                                isChange = true;
                                break;
                            }
                        }
                        if (isChange == false)
                        {
                            entitronTable.columns.AddDefaultValue(column.Name, column.DefaultValue);
                        }
                    }
                    else
                    {
                        foreach (string key in entitronTable.columns.GetDefaults().Keys)
                        {
                            if (key == def)
                            {
                                entitronTable.DropConstraint(key);
                                break;
                            }
                        }
                    }
                }
                e.Application.SaveChanges();
                List<string> primaryList1 = primaryColumnsForTable.Except(entitronTable.primaryKeys).ToList();
                List<string> primaryList2 = entitronTable.primaryKeys.Except(primaryColumnsForTable).ToList();
                if (primaryList1.Count>0 || primaryList2.Count>0)           //pokud v entitronu se nachází klíč, a pokud seznam sloupců ze kterého klíč je tvořen je v entitronu jiný než ve schématu, tak se musí smazat starý klíč a až potom vytvořit nový
                {
                    //todo ošetřit, že nebude moci být smazán primární klíč pokud je spojen v nějaké relaci, nebo smazat relaci jako první
                    entitronTable.DropConstraint($"PK_Entitron_{e.Application.Name}_{entitronTable.tableName}", true);
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }
                else if (entitronTable.primaryKeys.Count == 0 && primaryColumnsForTable.Count != 0)     //pokud v entitronu primární klíč není, přidá primární klíč, který se skládá z následujících sloupců
                {
                    entitronTable.AddPrimaryKey(primaryColumnsForTable);
                }
                e.Application.SaveChanges();

                List<string> newIndeces =
                    efTable
                    .Indices
                    .Select(x => "index_" + x.Name.ToLower())
                    .Except(entitronTable.indices.Select(x => x.indexName.ToLower()))
                    .ToList();                                      //list názvů indexů které jsou ve schématu, ale ne v entitronu

                List<string> deletedIndeces =
                    entitronTable
                    .indices
                    .Select(x => x.indexName.ToLower())
                    .Except(efTable.Indices.Select(x => "index_" + x.Name.ToLower()))
                    .ToList();                                      //list názvů indexů, které jsou v entitronu, ale už ne ve schématu

                foreach (string indexName in newIndeces)            //přidá do entitronu pouze indexy které se nenacházejí v entitronu
                {
                    DbIndex index =
                        efTable
                        .Indices
                        .SingleOrDefault(x => "index_" + x.Name.ToLower() == indexName.ToLower());

                    entitronTable.indices.AddToDB(index.Name, new List<string>(index.ColumnNames.Split(',')), index.Unique);
                }
                e.Application.SaveChanges();

                foreach (string indexName in deletedIndeces)        //smaže z entitronu pouze indexy které se nenacházejí ve schématu
                {
                    entitronTable.indices.DropFromDB(indexName);
                }
                e.Application.SaveChanges();
            } //end foreach efTable

                List<string> entitronsFKsNames = new List<string>();
                List<DBForeignKey> entitronFKs = new List<DBForeignKey>();
                foreach (DBTable table in e.Application.GetTables())
                {
                    foreach (DBForeignKey key in table.foreignKeys)
                    {
                        entitronFKs.Add(key);
                        entitronsFKsNames.Add(key.name.ToLower());
                    }
                }
                List<string> newFK =
                    dbSchemeCommit
                        .Relations
                        .Where(x1 => !entitronsFKsNames.Any(x2 => x2 == x1.Name.ToLower()))
                        .Select(x=>x.Name)
                        .ToList();                                           //list názvů indexů které jsou ve schématu, ale ne v entitronu

                List<string> deletedFK =
                     entitronsFKsNames
                    .Where(x1 => !dbSchemeCommit.Relations.Any(x2 => x2.Name.ToLower() == x1)).Distinct()
                        .ToList();                                           //list názvů indexů které jsou v entitronu, ale ne ve schématu

                foreach (string fkname in newFK)                         //přidá do entitronu všechny nové cizí klíče
                {
                    DbRelation efRelation = dbSchemeCommit.Relations.SingleOrDefault(x => x.Name.ToLower() == fkname.ToLower());
                    DbTable rightTable = dbSchemeCommit.Tables.SingleOrDefault(x1 => x1.Id == efRelation.RightTable);
                    DbTable leftTable = dbSchemeCommit.Tables.SingleOrDefault(x1 => x1.Id == efRelation.LeftTable);
                    DbColumn rightColumn = rightTable.Columns.SingleOrDefault(x => x.Id == efRelation.RightColumn);
                    DbColumn leftColumn = leftTable.Columns.SingleOrDefault(x => x.Id == efRelation.LeftColumn);
                    DBForeignKey entitronFK = new DBForeignKey();
                    entitronFK.sourceTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == rightTable.Name.ToLower());
                    entitronFK.targetTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == leftTable.Name.ToLower());
                    entitronFK.sourceColumn = entitronFK.sourceTable.columns.SingleOrDefault(c => c.Name.ToLower() == rightColumn.Name.ToLower()).Name;
                    entitronFK.targetColumn = entitronFK.targetTable.columns.SingleOrDefault(c => c.Name.ToLower() == leftColumn.Name.ToLower()).Name;
                    entitronFK.name = efRelation.Name;

                    entitronFK.sourceTable.foreignKeys.AddToDB(entitronFK);
                }
                e.Application.SaveChanges();

                foreach (string fkey in deletedFK)
                {
                    DBForeignKey foreignKeyForDrop = entitronFKs.SingleOrDefault(x => x.name.ToLower() == fkey);
                    foreignKeyForDrop.sourceTable.DropConstraint(foreignKeyForDrop.name);
                }

                e.Application.SaveChanges();
            List<string> deletedTables =
                            e.Application.GetTables()
                                .Select(x => x.tableName.ToLower())
                                .Except(dbSchemeCommit.Tables.Select(x => x.Name.ToLower()))
                                .ToList();                          //list tabulek které nejsou ve schématu, ale jsou ještě v entitronu

            foreach (string deleteTable in deletedTables)           //mazání všech tabulek z entitronu, které nejsou ve schématu
            {
                DBTable dropTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == deleteTable);
                e.Application.GetTable(dropTable.tableName).Drop();
            }
            e.Application.SaveChanges();

            foreach (DbTable schemeTable in dbSchemeCommit.Tables)
            {
                DBTable entitronTable = e.Application.
                    GetTables().
                    SingleOrDefault(x => x.tableName.ToLower() == schemeTable.Name.ToLower());

                List<string> deletedColumns =
                    entitronTable
                        .columns
                        .Select(x => x.Name.ToLower())
                        .Except(schemeTable.Columns.Select(x => x.Name.ToLower()))
                        .ToList();
                    //list všech id sloupců které se nenachází ve schématu, ale v entitronu ano(určené ke smazání)


                foreach (string columnName in deletedColumns) //mazání sloupců, které nejsou ve schématu
                {
                    DBColumn column =
                        entitronTable
                            .columns
                            .SingleOrDefault(x => x.Name == columnName);

                    entitronTable.columns.DropFromDB(column.Name);
                }
                e.Application.SaveChanges();
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}