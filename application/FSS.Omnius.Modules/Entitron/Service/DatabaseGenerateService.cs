using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Helpers;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.DB;

namespace FSS.Omnius.Modules.Entitron.Service
{
    public class DatabaseGenerateService : IDatabaseGenerateService
    {
        private DBConnection _db;
        private DBEntities _ent;
        private Application _app;

        private List<DBForeignKey> _entitronFKs;

        public delegate void SendWs(string str);

        public DatabaseGenerateService()
        {
            _entitronFKs = new List<DBForeignKey>();
        }

        /// <summary>
        /// </summary>
        /// <param name="dbSchemeCommit"></param>
        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core, SendWs sendWs)
        {
            if (dbSchemeCommit != null)
            {
                try
                {
                    _db = Entitron.i;
                    _ent = DBEntities.instance;
                    _app = _db.Application;

                    DropOldRelations(dbSchemeCommit);
                    GenerateTables(dbSchemeCommit, sendWs);
                    sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace tabulek", id = "entitron-gentables" }));
                    GenerateRelation(dbSchemeCommit);
                    sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace relací" }));
                    GenerateView(dbSchemeCommit);
                    sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "proběhla aktualizace pohledů" }));
                    DroppingOldTables(dbSchemeCommit, sendWs);
                    sendWs(Json.Encode(new { childOf = "entitron", type = "success", message = "staré tabulky byly smazány", id = "entitron-deltables" }));
                }
                catch(Exception)
                {
                    throw;
                }
                finally
                {
                    DBConnection.ClearCache();
                }
            }
        }

        public void GenerateDatabase(DbSchemeCommit dbSchemeCommit, CORE.CORE core)
        {
            GenerateDatabase(dbSchemeCommit, core, _ => { });
        }

        private void GenerateTables(DbSchemeCommit dbSchemeCommit, SendWs sendWs)
        {
            int progress = 0, progressMax = dbSchemeCommit.Tables.Count;
            
            foreach (DbTable efTable in dbSchemeCommit.Tables)
            {
                sendWs(Json.Encode(new
                {
                    childOf = "entitron",
                    id = "entitron-gentables",
                    type = "info",
                    message = $"probíhá aktualizace tabulek <span class='build-progress'>{progress}/{progressMax} <progress value={progress} max={progressMax}>({100.0 * progress / progressMax}%)</progress></span>"
                }));

                //// Table ////
                DBTable entitronTable = _db.Table(efTable.Name);
                // new
                if (!_db.Exists(entitronTable.Name, ETabloid.ApplicationTables))
                {
                    // columns
                    foreach (DbColumn column in efTable.Columns)
                    {
                        AddColumn(entitronTable, efTable, column);
                    }

                    // add indexes
                    foreach(DBIndex index in MergeSchemeIndexUnique(entitronTable, efTable))
                        entitronTable.Indexes.Add(index);

                    // create table & columns & costraints & indexes
                    entitronTable.Create();
                }
                // update existing
                else
                {
                    /// columns
                    UpdateColumns(entitronTable, efTable);

                    /// Indexes
                    List<DBIndex> designerIndexes = MergeSchemeIndexUnique(entitronTable, efTable);
                    foreach (DBIndex designerIndex in designerIndexes)
                    {
                        // create
                        DBIndex entitronIndex = entitronTable.Indexes.SingleOrDefault(ei => ei.Columns.Count == designerIndex.Columns.Count && ei.Columns.All(eic => designerIndex.Columns.Contains(eic)));
                        if (entitronIndex == null)
                            entitronTable.Indexes.Add(designerIndex);

                        // modify - isUnique changed
                        else if (entitronIndex.isUnique != designerIndex.isUnique)
                        {
                            entitronTable.Indexes.Remove(entitronIndex);
                            entitronTable.Indexes.Add(designerIndex);
                        }
                    }

                    // drop
                    IEnumerable<DBIndex> deletedIndeces = entitronTable.Indexes.Where(ei => !designerIndexes.Any(di => di.Columns.Count == ei.Columns.Count && ei.Columns.All(eic => di.Columns.Contains(eic)))).ToList();
                    foreach (DBIndex index in deletedIndeces)
                    {
                        entitronTable.Indexes.Remove(index);
                    }
                }

                progress++;
            } //end foreach efTable

            /// SAVE
            _db.SaveChanges();
            _ent.SaveChanges();
        }

        private void GenerateRelation(DbSchemeCommit dbSchemeCommit)
        {
            // foreign keys in scheme, but not in database
            IEnumerable<DbRelation> newFK = dbSchemeCommit.Relations
                .Where(rel => !_entitronFKs.Any(fk => fk.Compare(rel)));


            //adding new FKs
            foreach (DbRelation designerFK in newFK)
            {
                DBTable sourceTable = _db.Table(designerFK.LeftTable.Name);
                DBTable targetTable = _db.Table(designerFK.RightTable.Name);
                
                sourceTable.ForeignKeys.Add(new DBForeignKey(_db)
                {
                    SourceTable = sourceTable,
                    TargetTable = targetTable,
                    SourceColumn = designerFK.LeftColumn.Name,
                    TargetColumn = designerFK.RightColumn.Name
                });
            }

            _db.SaveChanges();
        }

        private void GenerateView(DbSchemeCommit dbSchemeCommit)
        {
            Queue<DbView> que = new Queue<DbView>(dbSchemeCommit.Views);
            DbView firstError = null;
            while(que.Any())
            {
                DbView efView = que.Dequeue();

                DBView newView = new DBView(_db)
                {
                    Name = efView.Name,
                    Sql = efView.Query
                };
                
                try
                {
                    if (!_db.Exists(efView.Name, ETabloid.Views))
                        newView.Create();
                    else
                        newView.Alter();

                    _db.SaveChanges();
                    firstError = null;
                }
                catch (Exception)
                {
                    if (firstError == null)
                        firstError = efView;
                    else if (firstError == efView)
                        throw;

                    que.Enqueue(efView);
                }
            }

            //list of views, which are in database, but not in scheme
            List<string> deleteViews = _db.List(ETabloid.Views)
                .Except(dbSchemeCommit.Views.Select(x => x.Name)).ToList();

            //dropping views
            foreach (string viewName in deleteViews)
            {
                _db.ViewDrop(viewName);
            }
            _db.SaveChanges();
        }

        private void DroppingOldTables(DbSchemeCommit dbSchemeCommit, SendWs sendWs)
        {
            //list of tables, which are in database, but not in scheme
            IEnumerable<string> deletedTables = _db.List(ETabloid.ApplicationTables)
                                .Except(dbSchemeCommit.Tables.Select(x => x.Name));

            //dropping old tables(must be here, after dropping all constraints)
            int progress = 0, progressMax = dbSchemeCommit.Tables.Count;
            foreach (string deleteTable in deletedTables)
            {
                sendWs(Json.Encode(new
                {
                    childOf = "entitron",
                    id = "entitron-deltables",
                    type = "info",
                    message = $"probíhá odstranění starých tabulek <span class='build-progress'>{progress}/{progressMax} <progress value={progress} max={progressMax}>({100.0 * progress / progressMax}%)</progress></span>"
                }));
                _db.TableDrop(new DBTable(_db) { Name = deleteTable });
                _ent.ColumnMetadata.RemoveRange(_app.ColumnMetadata.Where(c => c.TableName == deleteTable));
                progress++;
            }
            _db.SaveChanges();
        }
        
        private void UpdateColumns(DBTable entitronTable, DbTable schemeTable)
        {
            // list scheme columns
            foreach (DbColumn efColumn in schemeTable.Columns)
            {
                DBColumn entitronColumn = entitronTable.Columns
                    .SingleOrDefault(x => x.Name.ToLower() == efColumn.Name.ToLower());

                // add column
                if (entitronColumn == null)
                {
                    AddColumn(entitronTable, schemeTable, efColumn);
                }
                //updating existing column if changed
                else
                {
                    // column
                    if (!entitronColumn.Compare(efColumn))
                    {
                        entitronColumn.ModifyInDB(efColumn, reCreateIndex: false, reCreateForeignKeys: false);

                        // meta
                        ColumnMetadata meta = entitronColumn.Metadata;
                        meta.ColumnName = efColumn.Name;
                        meta.ColumnDisplayName = efColumn.DisplayName;
                        _ent.SaveChanges();
                    }
                    // defaults creates with column
                    else
                        // check & default
                        entitronTable.RefreshConstraints(entitronColumn, efColumn);
                }
            }
            _ent.SaveChanges();
            _db.SaveChanges();

            // Drop old
            IEnumerable<string> designerColumnNames = schemeTable.Columns.Select(dc => dc.Name);
            foreach (DBColumn column in entitronTable.Columns.Where(c => !designerColumnNames.Contains(c.Name)).ToList())
            {
                // remove index
                foreach(DBIndex index in entitronTable.Indexes.Where(i => i.Columns.Contains(column.Name)).ToList())
                {
                    entitronTable.Indexes.Remove(index);
                }
                // remove default
                if (column.DefaultValue != null)
                    column.DropDefault();

                entitronTable.Columns.Remove(column);
                _ent.ColumnMetadata.RemoveRange(_app.ColumnMetadata.Where(c => c.TableName == entitronTable.Name && c.ColumnName == column.Name));
            }
            _db.SaveChanges();
        }

        /// <summary>
        /// Add column to table, create Check and defaults
        /// </summary>
        private void AddColumn(DBTable table, DbTable designerTable, DbColumn designerColumn)
        {
            // column
            DbType type = DataType.FromDesignerName(designerColumn.Type);
            DBColumn col = new DBColumn(_db)
            {
                Tabloid = table,
                Name = designerColumn.Name,
                IsNullable = designerColumn.AllowNull,
                MaxLength = designerColumn.ColumnLengthIsMax ? DataType.MaxLength(type) : designerColumn.ColumnLength,
                Type = type,
                DefaultValue = designerColumn.RealDefaultValue(type),
                IsUnique = designerColumn.Unique || designerColumn.Name == DBCommandSet.PrimaryKey
            };
            table.Columns.Add(col);

            // metadata
            ColumnMetadata cm = _app.ColumnMetadata.SingleOrDefault(c => c.TableName == designerTable.Name && c.ColumnName == designerColumn.Name);
            if (cm == null)
            {
                cm = new ColumnMetadata()
                {
                    TableName = designerTable.Name,
                    ColumnName = designerColumn.Name
                };
                _app.ColumnMetadata.Add(cm);
            }
            cm.ColumnDisplayName = designerColumn.DisplayName ?? designerColumn.Name;
        }

        private void DropOldRelations(DbSchemeCommit dbSchemeCommit)
        {
            foreach (string tableName in _db.List(ETabloid.ApplicationTables))
            {
                _entitronFKs.AddRange(new DBTable(_db) { Name = tableName }.ForeignKeys);
            }

            // foreign keys in database, but not in scheme
            IEnumerable<DBForeignKey> deletedFK = _entitronFKs
                .Where(fk => !dbSchemeCommit.Relations.Any(rel => fk.Compare(rel)));

            //dropping old FKs
            foreach (DBForeignKey fk in deletedFK)
            {
                fk.SourceTable.ForeignKeys.Remove(fk);
            }

            _db.SaveChanges();
        }

        private List<DBIndex> MergeSchemeIndexUnique(DBTable table, DbTable designerTable)
        {
            // get designer indexes
            List<DBIndex> indexes = designerTable.Indices.Where(i => i.ColumnNames != "id").Select(i => new DBIndex(_db) { Table = table, Columns = i.ColumnNames.Split(',').ToList(), isUnique = i.Unique }).ToList();

            // get unique columns (not PK)
            IEnumerable<DbColumn> uniqueColumns = designerTable.Columns.Where(c => c.Unique && !c.PrimaryKey);
            // get target of FK (not PK)
            IEnumerable<DbColumn> targetOfForeignKey = designerTable.DbSchemeCommit.Relations.Where(r => r.RightTableId == designerTable.Id).Select(r => r.RightColumn).Where(c => !c.PrimaryKey);
            foreach (DbColumn column in uniqueColumns.Concat(targetOfForeignKey))
            {
                // exists index - update
                DBIndex index = indexes.SingleOrDefault(i => i.Columns.Count == 1 && i.Columns.First() == column.Name);
                if (index != null)
                    index.isUnique = true;

                // create new index
                else
                    indexes.Add(new DBIndex(_db) { Table = table, Columns = new List<string> { column.Name }, isUnique = true });
            }
            
            return indexes;
        }
    }
}