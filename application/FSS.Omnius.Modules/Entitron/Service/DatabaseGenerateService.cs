using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Table;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        /// <summary>
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core)
        {
            if (dbSchemeCommit != null)
            {
                Entitron e = core.Entitron;
                GenerateTables(e, dbSchemeCommit);
                GenerateRelation(e, dbSchemeCommit);
                GenerateView(e, dbSchemeCommit);
                DroppingOldTables(e, dbSchemeCommit);
            }
        }

        private void GenerateTables(Entitron e, DbSchemeCommit dbSchemeCommit)
        {
            DBEntities ent = new DBEntities();
            var app = ent.Applications.Find(e.AppId);
            ent.ColumnMetadata.RemoveRange(app.ColumnMetadata);
            ent.SaveChanges();

            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                DBTable entitronTable = e.Application.GetTable(efTable.Name);

                bool tableExists = DBTable.isInDB(e.Application.Name, efTable.Name);

                //if table doesnt exist, create new one
                if (entitronTable == null || !tableExists)
                {
                    entitronTable = new DBTable();
                    entitronTable.tableName = efTable.Name;
                    entitronTable.Application = e.Application;

                    foreach (DbColumn column in efTable.Columns)
                    {
                        DBColumn col = new DBColumn()
                        {
                            Name = column.Name,
                            isPrimary = column.PrimaryKey,
                            canBeNull = column.AllowNull,
                            maxLength = column.ColumnLength,
                            type = ent.DataTypes.Single(t => t.DBColumnTypeName.Contains(column.Type)).SqlName
                        };
                        entitronTable.columns.Add(col);
                        app.ColumnMetadata.Add(new ColumnMetadata
                        {
                            TableName = efTable.Name,
                            ColumnName = column.Name,
                            ColumnDisplayName = column.DisplayName ?? column.Name
                        });
                    }
                    entitronTable.Create();
                    e.Application.SaveChanges();
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
                    UpdateColumns(entitronTable,efTable,e);
                }//end updating table columns
                e.Application.SaveChanges();

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
                e.Application.SaveChanges();

                //list of index names, which are in database, but not in scheme
                List<string> deletedIndeces = entitronTable.indices.Select(x => x.indexName.ToLower())
                    .Except(efTable.Indices.Select(x => "index_" + x.Name.ToLower())).ToList();


                //droping new indeces
                foreach (string indexName in deletedIndeces)
                {
                    entitronTable.indices.DropFromDB(indexName);
                }
                e.Application.SaveChanges();
            } //end foreach efTable
            ent.SaveChanges();
        }

        private void GenerateRelation(Entitron e, DbSchemeCommit dbSchemeCommit)
        {
            List<string> entitronsFKsNames = new List<string>();
            List<DBForeignKey> entitronFKs = new List<DBForeignKey>();

            //getting FKs for dropping, FK names are for list of new FKs and oldFks
            foreach (DBTable table in e.Application.GetTables())
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
            /*foreach (string fkname in newFK)
            {
                DbRelation efRelation = dbSchemeCommit.Relations.SingleOrDefault(x => x.Name.ToLower() == fkname.ToLower());
                DbTable rightTable = efRelation.RightTable;
                DbTable leftTable = efRelation.LeftTable;
                DbColumn rightColumn = efRelation.RightColumn;
                DbColumn leftColumn = efRelation.LeftColumn;

                DBForeignKey entitronFK = new DBForeignKey();
                entitronFK.sourceTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == rightTable.Name.ToLower());
                entitronFK.targetTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == leftTable.Name.ToLower());
                entitronFK.sourceColumn = entitronFK.sourceTable.columns.SingleOrDefault(c => c.Name.ToLower() == rightColumn.Name.ToLower()).Name;
                entitronFK.targetColumn = entitronFK.targetTable.columns.SingleOrDefault(c => c.Name.ToLower() == leftColumn.Name.ToLower()).Name;
                entitronFK.name = efRelation.Name;

                entitronFK.sourceTable.foreignKeys.AddToDB(entitronFK);
            }*/
            e.Application.SaveChanges();

            //dropping old FKs
            foreach (string fkey in deletedFK)
            {
                DBForeignKey foreignKeyForDrop = entitronFKs.SingleOrDefault(x => x.name.ToLower() == fkey);
                foreignKeyForDrop.sourceTable.DropConstraint(foreignKeyForDrop.name);
            }
            e.Application.SaveChanges();

        }

        private void GenerateView(Entitron e, DbSchemeCommit dbSchemeCommit)
        {
            foreach (DbView efView in dbSchemeCommit.Views)
            {
                bool viewExists = DBView.isInDb(e.Application.Name, efView.Name);

                DBView newView = new DBView()
                {
                    Application = e.Application,
                    dbViewName = efView.Name,
                    sql = efView.Query
                };

                if (!viewExists)
                {
                    newView.Create();
                }
                else
                {
                    newView.Alter();
                }
                e.Application.SaveChanges();
            }//end of foreach efViews

            //list of views, which are in database, but not in scheme
            List<string> deleteViews = e.Application.GetViewNames()
                .Except(dbSchemeCommit.Views.Select(x => "Entitron_" + e.Application.Name + "_" + x.Name)).ToList();

            //dropping views
            foreach (string viewName in deleteViews)
            {
                DBView.Drop(viewName);
            }
            e.Application.SaveChanges();
        }

        private void DroppingOldTables(Entitron e, DbSchemeCommit dbSchemeCommit)
        {
            //list of tables, which are in database, but not in scheme
            List<string> deletedTables = e.Application.GetTables().Select(x => x.tableName.ToLower())
                                .Except(dbSchemeCommit.Tables.Select(x => x.Name.ToLower())).ToList();

            //dropping old tables(must be here, after dropping all constraints)
            foreach (string deleteTable in deletedTables)
            {
                DBTable dropTable = e.Application.GetTables().SingleOrDefault(x => x.tableName.ToLower() == deleteTable);
                e.Application.GetTable(dropTable.tableName).Drop();
            }
            e.Application.SaveChanges();

            //foreach for tables again, for getting all columns
            foreach (DbTable schemeTable in dbSchemeCommit.Tables)
            {
                DBTable entitronTable = e.Application.GetTables()
                    .SingleOrDefault(x => x.tableName.ToLower() == schemeTable.Name.ToLower());

                //list of column names, which are in database,but not in scheme
                List<string> deletedColumns = entitronTable.columns.Select(x => x.Name.ToLower())
                        .Except(schemeTable.Columns.Select(x => x.Name.ToLower())).ToList();

                //dropping columns, must be here for the same reason like tables, it is because FKs must be dropping first
                foreach (string columnName in deletedColumns)
                {
                    DBColumn column = entitronTable.columns.SingleOrDefault(x => x.Name.ToLower() == columnName);
                    if(column.isUnique)
                        entitronTable.DropConstraint($"UN_Entitron_{e.Application.Name}_{entitronTable.tableName}_{column.Name}");

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
                e.Application.SaveChanges();
            }//end of foreach schemeTable for dropping old columns

        }

        private void UpdateColumns(DBTable entitronTable, DbTable schemeTable, Entitron e)
        {
            DBEntities ent = new DBEntities();
            var app = ent.Applications.Find(e.AppId);
            ent.SaveChanges();

            foreach (DbColumn efColumn in schemeTable.Columns)
            {
                DBColumn entitronColumn = entitronTable.columns
                    .SingleOrDefault(x => x.Name.ToLower() == efColumn.Name.ToLower());

                app.ColumnMetadata.Add(new ColumnMetadata
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
                        maxLength = efColumn.ColumnLength,
                        type = ent.DataTypes.Single(t => t.DBColumnTypeName.Contains(efColumn.Type)).SqlName,
                        DefaultValue = efColumn.DefaultValue
                    };
                    entitronTable.columns.AddToDB(entitronColumn);
                    if (efColumn.Unique)
                    {
                        entitronTable.columns.AddUniqueValue(efColumn.Name);
                    }
                    e.Application.SaveChanges();
                }//end column==null
                else
                {//updating existing column
                    if (entitronColumn.canBeNull != efColumn.AllowNull ||
                        entitronColumn.maxLength != efColumn.ColumnLength ||
                        entitronColumn.type != efColumn.Type)
                        entitronTable.columns.ModifyInDB(entitronColumn.Name, ent.DataTypes.Single(t => t.DBColumnTypeName.Contains(efColumn.Type)).SqlName, efColumn.ColumnLength, entitronColumn.precision, entitronColumn.scale, efColumn.AllowNull);

                    if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique == false && !efColumn.PrimaryKey)
                    {
                        entitronTable.columns.AddUniqueValue(efColumn.Name);
                    }
                    else if (entitronColumn.isUnique != efColumn.Unique && entitronColumn.isUnique)
                    {
                        entitronTable.DropConstraint($"UN_Entitron_{e.Application.Name}_{entitronTable.tableName}_{entitronColumn.Name}");
                    }

                    //set column default value
                    Dictionary<string, string> defaultConstraint = entitronTable.columns.GetSpecificDefault(entitronColumn.Name);

                    if (!string.IsNullOrEmpty(efColumn.DefaultValue))
                    {
                        if (defaultConstraint.Count != 0 && efColumn.DefaultValue != defaultConstraint.Values.First())
                        {
                            entitronTable.DropConstraint(defaultConstraint.Keys.First());
                            entitronTable.columns.AddDefaultValue(efColumn.Name, efColumn.DefaultValue);
                        }
                        else if (defaultConstraint.Count == 0)
                        {
                            entitronTable.columns.AddDefaultValue(efColumn.Name, efColumn.DefaultValue);
                        }
                    }
                    else if (defaultConstraint.Count != 0)
                    {
                        entitronTable.DropConstraint(defaultConstraint.Keys.First());
                    }

                }//end updating column


            }//end foreach efColumn
            ent.SaveChanges();
        }
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EntitronTesting"].ConnectionString;
        }
    }
}