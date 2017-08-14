﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Helpers;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Table;
using FSS.Omnius.Modules.Entitron.Entity.CORE;
using System;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        private string TablesPrefix = null;

        public string GetTablesPrefix(Application app)
        {
            return this.TablesPrefix != null ? this.TablesPrefix : app.Name;
        }

        public DatabaseGenerateService(string tablesPrefix = null)
        {
            this.TablesPrefix = tablesPrefix;
        }

        private Entitron _entitron;
        private DBEntities _ent;
        private Application _app;

        public delegate void SendWs(string str);
        /// <summary>
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core, SendWs sendWs)
        {
            if (dbSchemeCommit != null)
            {
                _entitron = core.Entitron;
                _ent = DBEntities.appInstance(_entitron.Application);
                _app = _ent.Applications.SingleOrDefault(a => a.Name == _entitron.Application.Name);

                GenerateTables(dbSchemeCommit, sendWs);
                sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace tabulek", id = "entitron-gentables" }));
                GenerateRelation(dbSchemeCommit);
                sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace relací" }));
                GenerateView(dbSchemeCommit);
                sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace pohledů" }));
                DroppingOldTables(dbSchemeCommit, sendWs);
                sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "staré tabulky byly smazány", id = "entitron-deltables" }));
            }
        }

        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core)
        {
            GenerateDatabase(dbSchemeCommit, core, _ => { });
        }

        private void GenerateTables(DbSchemeCommit dbSchemeCommit, SendWs sendWs)
        {
            _ent.ColumnMetadata.RemoveRange(_app.ColumnMetadata);
            _ent.SaveChanges();

            int progress = 0, progressMax = dbSchemeCommit.Tables.Count;
            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                progress++;
                sendWs(Json.Encode(new { childOf = "entitron", id = "entitron-gentables", type = "info",
                    message = $"probíhá aktualizace tabulek <span class='build-progress'>{progress}/{progressMax} <progress value={progress} max={progressMax}>({100.0 * progress / progressMax}%)</progress></span>" }));
                DBTable entitronTable = _entitron.Application.GetTable(efTable.Name);

                //if table doesn't exist, create new one
                if (entitronTable == null)
                {
                    entitronTable = new DBTable();
                    entitronTable.tableName = efTable.Name;
                    entitronTable.Application = _app;

                    foreach (DbColumn column in efTable.Columns)
                    {
                        DBColumn col = new DBColumn()
                        {
                            Name = column.Name,
                            isPrimary = column.PrimaryKey,
                            canBeNull = column.AllowNull,
                            maxLength = column.ColumnLengthIsMax ? 4000 : column.ColumnLength,
                            type = DataType.ByDBColumnTypeName(column.Type).SqlName
                        };
                        entitronTable.columns.Add(col);
                        _app.ColumnMetadata.Add(new ColumnMetadata
                        {
                            TableName = efTable.Name,
                            ColumnName = column.Name,
                            ColumnDisplayName = column.DisplayName ?? column.Name
                        });
                    }
                    entitronTable.Create();
                    _entitron.Application.SaveChanges();
                    //set default values and unique constraints for new tables
                    foreach (DbColumn defColumn in efTable.Columns)
                    {
                        if (!string.IsNullOrEmpty(defColumn.DefaultValue))
                        {
                            entitronTable.columns.AddDefaultValue(defColumn.Name, defColumn.DefaultValue);
                        }
                        if (defColumn.Unique)
                        {
                            entitronTable.columns.AddUniqueValue(defColumn.Name);
                        }
                    }
                }//end of table==null
                else
                {//if table exists, update columns
                    UpdateColumns(entitronTable,efTable);
                }//end updating table columns
                _entitron.Application.SaveChanges();

                //set indeces

                if (efTable.Indices.Count != 0)
                {
                    foreach (DbIndex i in efTable.Indices)
                    {
                        DBIndex index = entitronTable.GetIndex(i.Name);
                        if (index.indexName==null)
                        {
                            entitronTable.indices.AddToDB(i.Name, i.ColumnNames.Split(',').ToList(), i.Unique);
                        }
                        else if(index.isUnique!=i.Unique || index.columns.Select(x=>x.Name)!=i.ColumnNames.Split(',').ToList())
                        {
                            entitronTable.indices.DropFromDB(index.indexName);
                            entitronTable.indices.AddToDB(i.Name, i.ColumnNames.Split(',').ToList(), i.Unique);
                        }
                    }
                }
                _entitron.Application.SaveChanges();

                //list of index names, which are in database, but not in scheme
                List<string> deletedIndeces = entitronTable.indices.Select(x => x.indexName.ToLower())
                    .Except(efTable.Indices.Select(x => "index_" + x.Name.ToLower())).ToList();


                //droping new indeces
                foreach (string indexName in deletedIndeces)
                {
                    entitronTable.indices.DropFromDB(indexName);
                }
                _entitron.Application.SaveChanges();
            } //end foreach efTable
            _ent.SaveChanges();
        }

        private void GenerateRelation(DbSchemeCommit dbSchemeCommit)
        {
            List<string> entitronsFKsNames = new List<string>();
            List<DBForeignKey> entitronFKs = new List<DBForeignKey>();

            //getting FKs for dropping, FK names are for list of new FKs and oldFks
            foreach (DBTable table in _entitron.Application.GetTables())
            {
                foreach (DBForeignKey key in table.foreignKeys)
                {
                    entitronFKs.Add(key);
                    entitronsFKsNames.Add(key.name.ToLower());
                }
            }

            //list of foreign key names, which are in scheme, but not in database
            List<string> newFK = dbSchemeCommit.Relations
                .Where(x1 => !entitronsFKsNames.Any(x2 => x2 == "fk_" + x1.Name.ToLower())).Select(x => x.Name).ToList();

            //list of foreign key names, which are in database, but not in scheme
            List<string> deletedFK = entitronsFKsNames
                .Where(x1 => !dbSchemeCommit.Relations.Any(x2 => "fk_" + x2.Name.ToLower() == x1)).Distinct().ToList();

            //adding new FKs
            foreach (string fkname in newFK)
            {
                DbRelation efRelation = dbSchemeCommit.Relations.SingleOrDefault(x => x.Name.ToLower() == fkname.ToLower());
                DbTable rightTable = efRelation.RightTable;
                DbTable leftTable = efRelation.LeftTable;
                DbColumn rightColumn = efRelation.RightColumn;
                DbColumn leftColumn = efRelation.LeftColumn;

                DBForeignKey entitronFK = new DBForeignKey();
                entitronFK.sourceTable = _entitron.Application.GetTable(rightTable.Name);
                entitronFK.targetTable = _entitron.Application.GetTable(leftTable.Name);
                entitronFK.sourceColumn = entitronFK.sourceTable.columns.SingleOrDefault(c => c.Name.ToLower() == rightColumn.Name.ToLower()).Name;
                entitronFK.targetColumn = entitronFK.targetTable.columns.SingleOrDefault(c => c.Name.ToLower() == leftColumn.Name.ToLower()).Name;
                entitronFK.name = efRelation.Name;

                entitronFK.sourceTable.foreignKeys.AddToDB(entitronFK);
            }
            _entitron.Application.SaveChanges();

            //dropping old FKs
            foreach (string fkey in deletedFK)
            {
                DBForeignKey foreignKeyForDrop = entitronFKs.SingleOrDefault(x => x.name.ToLower() == fkey);
                foreignKeyForDrop.sourceTable.DropConstraint(foreignKeyForDrop.name);
            }
            _entitron.Application.SaveChanges();
        }

        private void GenerateView(DbSchemeCommit dbSchemeCommit)
        {
            foreach (DbView efView in dbSchemeCommit.Views)
            {
                try
                {
                    bool viewExists = DBView.isInDb(_app, efView.Name);

                    DBView newView = new DBView()
                    {
                        Application = _app,
                        dbViewName = efView.Name,
                        sql = efView.Query
                    };

                    if (!viewExists)
                        newView.Create();
                    else
                        newView.Alter();

                    _entitron.Application.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{efView.Name} - {ex.Message}", ex);
                }
            }//end of foreach efViews

            //list of views, which are in database, but not in scheme
            List<string> deleteViews = _entitron.Application.GetViewNames()
                .Except(dbSchemeCommit.Views.Select(x => "Entitron_" + this.GetTablesPrefix(_entitron.Application) + "_" + x.Name)).ToList();

            //dropping views
            foreach (string viewName in deleteViews)
            {
                DBView.Drop(_app, viewName);
            }
            _entitron.Application.SaveChanges();
        }

        private void DroppingOldTables(DbSchemeCommit dbSchemeCommit, SendWs sendWs)
        {
            //list of tables, which are in database, but not in scheme
            List<string> deletedTables = _entitron.Application.GetTables().Select(x => x.tableName.ToLower())
                                .Except(dbSchemeCommit.Tables.Select(x => x.Name.ToLower())).ToList();

            //dropping old tables(must be here, after dropping all constraints)
            foreach (string deleteTable in deletedTables)
            {
                DBTable dropTable = _entitron.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == deleteTable);
                _entitron.Application.GetTable(dropTable.tableName).Drop();
            }
            _entitron.Application.SaveChanges();

            int progress = 0, progressMax = dbSchemeCommit.Tables.Count;

            //foreach for tables again, for getting all columns
            foreach (DbTable schemeTable in dbSchemeCommit.Tables)
            {
                progress++;
                sendWs(Json.Encode(new { childOf = "entitron", id = "entitron-deltables", type = "info",
                    message = $"probíhá odstranění starých tabulek <span class='build-progress'>{progress}/{progressMax} <progress value={progress} max={progressMax}>({100.0 * progress / progressMax}%)</progress></span>" }));
                DBTable entitronTable = _entitron.Application.GetTables()
                    .SingleOrDefault(x => x.tableName.ToLower() == schemeTable.Name.ToLower());

                //list of column names, which are in database,but not in scheme
                List<string> deletedColumns = entitronTable.columns.Select(x => x.Name.ToLower())
                        .Except(schemeTable.Columns.Select(x => x.Name.ToLower())).ToList();

                //dropping columns, must be here for the same reason like tables, it is because FKs must be dropping first
                foreach (string columnName in deletedColumns)
                {
                    DBColumn column = entitronTable.columns.SingleOrDefault(x => x.Name.ToLower() == columnName);
                    if(column.isUnique)
                        entitronTable.DropConstraint($"UN_Entitron_{this.GetTablesPrefix(_entitron.Application)}_{entitronTable.tableName}_{column.Name}");

                    Dictionary<string, string> defaultConstraint = entitronTable.columns.GetSpecificDefault(column.Name);
                    if (defaultConstraint.Count!=0)
                    {
                        entitronTable.DropConstraint(defaultConstraint.Keys.First());
                    }
                    if (entitronTable.indices.Count != 0)
                    {
                        List<DBIndex> columnIndeces = entitronTable.indices.Where(c => c.columns.Contains(column)).ToList();
                        foreach (DBIndex columnIndex in columnIndeces)
                        {
                                entitronTable.indices.DropFromDB(columnIndex.indexName);
                        }
                    }

                    entitronTable.columns.DropFromDB(column.Name);
                }
                _entitron.Application.SaveChanges();
            }//end of foreach schemeTable for dropping old columns

        }

        private void UpdateColumns(DBTable entitronTable, DbTable schemeTable)
        {
            // MN: Protoze SQL Server < 2016 je stupidní a neumí alter / drop na sloupcích s constraintem :/
            entitronTable.DropAllConstraints();

            foreach (DbColumn efColumn in schemeTable.Columns)
            {
                DBColumn entitronColumn = entitronTable.columns
                    .SingleOrDefault(x => x.Name.ToLower() == efColumn.Name.ToLower());

                _app.ColumnMetadata.Add(new ColumnMetadata
                {
                    TableName = schemeTable.Name,
                    ColumnName = efColumn.Name,
                    ColumnDisplayName = efColumn.DisplayName ?? efColumn.Name
                });

                if (entitronColumn == null)                     //adding new column
                {
                    entitronColumn = new DBColumn()
                    {
                        Name = efColumn.Name,
                        canBeNull = efColumn.AllowNull,
                        maxLength = efColumn.ColumnLengthIsMax ? 4000 : efColumn.ColumnLength,
                        type = DataType.ByDBColumnTypeName(efColumn.Type).SqlName,
                        DefaultValue = efColumn.DefaultValue
                    };
                    entitronTable.columns.AddToDB(entitronColumn);
                    if (efColumn.Unique)
                    {
                        entitronTable.columns.AddUniqueValue(efColumn.Name);
                    }
                    _entitron.Application.SaveChanges();
                }//end column==null
                else
                {//updating existing column
                    if (entitronColumn.canBeNull != efColumn.AllowNull ||
                        entitronColumn.maxLength != efColumn.ColumnLength ||
                        entitronColumn.type != efColumn.Type)
                        entitronTable.columns.ModifyInDB(entitronColumn.Name, DataType.ByDBColumnTypeName(efColumn.Type).SqlName, efColumn.ColumnLengthIsMax ? 4000 : efColumn.ColumnLength, entitronColumn.precision, entitronColumn.scale, efColumn.AllowNull);

                    /* MN: Protože jsme na začátku smazali všechny constrainty, vždy je pouze vytvoříme, pokud mají existovat */
                    if (entitronColumn.isUnique && !efColumn.PrimaryKey) { //set column as unique
                        entitronTable.columns.AddUniqueValue(efColumn.Name);
                    }
                    if (!string.IsNullOrEmpty(efColumn.DefaultValue)) { //set column default value
                        entitronTable.columns.AddDefaultValue(efColumn.Name, efColumn.DefaultValue);
                    }
                }//end updating column


            }//end foreach efColumn
            _ent.SaveChanges();
        }
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}