using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.DAL;
using FSS.Omnius.Modules.Entitron.Entity.Entitron;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Service;
using Logger;
using static System.String;

namespace FSS.Omnius.Controllers.Entitron
{
    public class DatabaseController : ApiController
    {
        public DatabaseController(IRepository<DbSchemeCommit> repositoryDbSchemeCommit,
            IDatabaseGenerateService databaseGenerateService)
        {
            if (repositoryDbSchemeCommit == null) throw new ArgumentNullException(nameof(repositoryDbSchemeCommit));
            if (databaseGenerateService == null) throw new ArgumentNullException(nameof(databaseGenerateService));
            RepositoryDbSchemeCommit = repositoryDbSchemeCommit;
            DatabaseGenerateService = databaseGenerateService;
        }

        private IRepository<DbSchemeCommit> RepositoryDbSchemeCommit { get; }
        private IDatabaseGenerateService DatabaseGenerateService { get; set; }


        [Route("api/database/commits")]
        [HttpGet]
        public IEnumerable<AjaxTransferCommitHeader> GetCommitList()
        {
            try
            {
                return RepositoryDbSchemeCommit.Get(orderBy: q => q.OrderByDescending(d => d.Timestamp))
                        .Select(c => new AjaxTransferCommitHeader
                        {
                            CommitMessage = c.CommitMessage,
                            Id = c.Id,
                            TimeCommit = c.Timestamp
                        });
            }
            catch (Exception ex)
            {
                var errorMessage = $"DatabaseDesigner: error when loading the commit history (GET api/database/commits). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);

            }
        }



        [Route("api/database/commits/{commitId}")]
        [HttpGet]
        public AjaxTransferDbScheme GetCommitById(int commitId)
        {
            try
            {
                return GetCommit(commitId);
            }
            catch (InstanceNotFoundException ex)
            {
                throw GetHttpInternalServerErrorResponseException(ex.Message);

            }
            catch (Exception ex)
            {
                var errorMessage = Format("DatabaseDesigner: error when loading the commit with id={1} (GET api/database/commits/{1}). "
                    + "Exception message: {0}", ex.Message, commitId);
                throw GetHttpInternalServerErrorResponseException(errorMessage);

            }
        }

        [Route("api/database/commits/latest")]
        [HttpGet]
        public AjaxTransferDbScheme LoadLatest()
        {
            try
            {
                return GetCommit();
            }
            catch (InstanceNotFoundException ex)
            {
                throw GetHttpInternalServerErrorResponseException(ex.Message);
            }
            catch (Exception ex)
            {
                var errorMessage = "DatabaseDesigner: error when loading the latest commit (GET api/database/commits/latest). " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/database/generate")]
        [HttpGet]
        //not add reference Entitron
        public void Generate(int AppId)
        {
            try
            {
                Modules.CORE.CORE core = new Modules.CORE.CORE();
                core.Entitron.AppId = AppId;
                var dbSchemeCommit = RepositoryDbSchemeCommit.Get(orderBy: q => q.OrderByDescending(d => d.Timestamp)).First();
                DatabaseGenerateService.GenerateDatabase(core.Entitron.Application, dbSchemeCommit);
            }
            catch (Exception ex)
            {
                throw GetHttpInternalServerErrorResponseException(ex.Message);
            }
        }

        [Route("api/database/commits")]
        [HttpPost]
        public void SaveScheme(AjaxTransferDbScheme postData)
        {
            try
            {
                DbSchemeCommit commit = new DbSchemeCommit();
                using (var context = new DBEntities())
                {
                    commit.Timestamp = DateTime.Now;
                    commit.CommitMessage = postData.CommitMessage;
                    context.DBSchemeCommits.Add(commit);
                    Dictionary<int, int> tableIdMapping = new Dictionary<int, int>();
                    Dictionary<int, int> columnIdMapping = new Dictionary<int, int>();

                    foreach (var ajaxTable in postData.Tables)
                    {
                        int ajaxTableId = ajaxTable.Id;
                        DbTable newTable = new DbTable { Name = ajaxTable.Name, PositionX = ajaxTable.PositionX, PositionY = ajaxTable.PositionY };
                        commit.Tables.Add(newTable);
                        context.SaveChanges();
                        tableIdMapping.Add(ajaxTableId, newTable.Id);
                        foreach (var column in ajaxTable.Columns)
                        {
                            int ajaxColumnId = column.Id;
                            DbColumn newColumn = new DbColumn
                            {
                                Name = column.Name,
                                Type = column.Type,
                                PrimaryKey = column.PrimaryKey,
                                AllowNull = column.AllowNull,
                                DefaultValue = column.DefaultValue,
                                ColumnLength = column.ColumnLength,
                                ColumnLengthIsMax = column.ColumnLengthIsMax,
                                Unique = column.Unique
                            };
                            newTable.Columns.Add(newColumn);
                            context.SaveChanges();
                            columnIdMapping.Add(ajaxColumnId, newColumn.Id);
                        }
                        foreach (var index in ajaxTable.Indices)
                        {
                            string columnNamesString = "";
                            if (index.ColumnNames.Count > 0)
                            {
                                for (int i = 0; i < index.ColumnNames.Count - 1; i++)
                                    columnNamesString += index.ColumnNames[i] + ",";
                                columnNamesString += index.ColumnNames.Last();
                            }
                            DbIndex newIndex = new DbIndex
                            {
                                Name = index.Name,
                                Unique = index.Unique,
                                ColumnNames = columnNamesString
                            };
                            newTable.Indices.Add(newIndex);
                        }
                    }
                    foreach (var ajaxRelation in postData.Relations)
                    {
                        int leftTable = tableIdMapping[ajaxRelation.LeftTable];
                        int rightTable = tableIdMapping[ajaxRelation.RightTable];
                        int leftColumn = columnIdMapping[ajaxRelation.LeftColumn];
                        int rightColumn = columnIdMapping[ajaxRelation.RightColumn];
                        commit.Relations.Add(new DbRelation
                        {
                            LeftTable = leftTable,
                            RightTable = rightTable,
                            LeftColumn = leftColumn,
                            RightColumn = rightColumn,
                            Type = ajaxRelation.Type
                        });
                    }
                    foreach (var ajaxView in postData.Views)
                    {
                        commit.Views.Add(new DbView
                        {
                            Name = ajaxView.Name,
                            Query = ajaxView.Query,
                            PositionX = ajaxView.PositionX,
                            PositionY = ajaxView.PositionY
                        });
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "DatabaseDesigner: an error occurred when saving the database scheme (POST api/database/commits). " +
                        $"Exception message: {ex.Message}";
                Log.Error(errorMessage);
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }

        }

        public void SaveChanges(DbSchemeCommit SchemeCommit)
        {
            DBEntities e = new DBEntities();
            foreach (DbTable schemeTable in SchemeCommit.Tables)
            {
                IEnumerable<DbTable> removeTables = SchemeCommit.Tables.Where(x1 => !e.DbTables.Any(x2 => x2.Id == x1.Id));
                e.DbTables.Except<DbTable>(removeTables); //maže všechny tabulky, které se už nenachází na schématu uživatele

                if (e.DbTables.SingleOrDefault(x => x.Id == schemeTable.Id) == null) //pokud je ve schématu uživatele vytvořena nová tabulka
                {
                    e.DbTables.Add(schemeTable);
                }
                else
                {
                    DbTable DatabaseTable = e.DbTables.SingleOrDefault(x => x.Id == schemeTable.Id);
                    if (DatabaseTable.Name != schemeTable.Name) DatabaseTable.Name = schemeTable.Name;

                    foreach (DbColumn schemeColumn in schemeTable.Columns)
                    {
                        IEnumerable<DbColumn> removeColumns = schemeTable.Columns.Where(x1 => !DatabaseTable.Columns.Any(x2 => x2.Id == x1.Id));
                        DatabaseTable.Columns.Except<DbColumn>(removeColumns); //maže všechny sloupce tabulky, které se už nenachází na schématu uživatele

                        if (DatabaseTable.Columns.SingleOrDefault(x=>x.Id==schemeColumn.Id) == null) //pokud je ve schématu uživatele vytvořen nový sloupec
                        {
                            DatabaseTable.Columns.Add(schemeColumn);
                        }
                        else
                        {
                            DbColumn DatabaseColumn = DatabaseTable.Columns.SingleOrDefault(x => x.Id == schemeColumn.Id);

                            if (DatabaseColumn.Name != schemeColumn.Name) DatabaseColumn.Name = schemeColumn.Name;
                            if (DatabaseColumn.PrimaryKey != schemeColumn.PrimaryKey) DatabaseColumn.PrimaryKey = schemeColumn.PrimaryKey;
                            if (DatabaseColumn.Type != schemeColumn.Type) DatabaseColumn.Type = schemeColumn.Type;
                            if (DatabaseColumn.Unique != schemeColumn.Unique) DatabaseColumn.Unique = schemeColumn.Unique;
                            if (DatabaseColumn.AllowNull != schemeColumn.AllowNull) DatabaseColumn.AllowNull = schemeColumn.AllowNull;
                            if (DatabaseColumn.ColumnLength != schemeColumn.ColumnLength) DatabaseColumn.ColumnLength = schemeColumn.ColumnLength;
                            if (DatabaseColumn.ColumnLengthIsMax != schemeColumn.ColumnLengthIsMax) DatabaseColumn.ColumnLengthIsMax = schemeColumn.ColumnLengthIsMax;
                            if (DatabaseColumn.DefaultValue != schemeColumn.DefaultValue) DatabaseColumn.DefaultValue = schemeColumn.DefaultValue;
                        }
                    }
                    foreach(DbIndex schemeIndex in schemeTable.Indices)
                    {
                        IEnumerable<DbIndex> removeIndeces = schemeTable.Indices.Where(x1 => !DatabaseTable.Indices.Any(x2 => x2.Id == x1.Id));
                        DatabaseTable.Indices.Except<DbIndex>(removeIndeces); //maže všechny indexy tabulky, které se už nenachází na schématu uživatele

                        if (DatabaseTable.Indices.SingleOrDefault(x => x.Id == schemeIndex.Id) == null) //pokud je ve schématu uživatele vytvořen nový index
                        {
                            DatabaseTable.Indices.Add(schemeIndex);
                        }
                        else
                        {
                            DbIndex databaseIndex = DatabaseTable.Indices.SingleOrDefault(x => x.Id == schemeIndex.Id);

                            if (databaseIndex.ColumnNames != schemeIndex.ColumnNames) databaseIndex.ColumnNames = schemeIndex.ColumnNames;
                            if (databaseIndex.Name != schemeIndex.Name) databaseIndex.Name = schemeIndex.Name;
                            if (databaseIndex.Unique != schemeIndex.Unique) databaseIndex.Unique = schemeIndex.Unique;
                        }
                    }

                }
                
            }

            foreach (DbRelation schemeRelation in SchemeCommit.Relations)
            {
                IEnumerable<DbRelation> removeRelations = SchemeCommit.Relations.Where(x1 => !e.DbRelation.Any(x2 => x2.Id == x1.Id));
                e.DbRelation.Except<DbRelation>(removeRelations); //maže všechny vztahy, které se už nenachází ve schématu uživatele

                if (e.DbRelation.SingleOrDefault(x => x.Id == schemeRelation.Id) == null) //pokud je ve schématu uživatele vytvořen nový vztah
                {
                    e.DbRelation.Add(schemeRelation);
                }
                else
                {
                    DbRelation databaseRelation = e.DbRelation.SingleOrDefault(x => x.Id == schemeRelation.Id);

                    if (databaseRelation.LeftTable != schemeRelation.LeftTable) databaseRelation.LeftTable = schemeRelation.LeftTable;
                    if (databaseRelation.RightTable != schemeRelation.RightTable) databaseRelation.RightTable = schemeRelation.RightTable;
                    if (databaseRelation.LeftColumn != schemeRelation.LeftColumn) databaseRelation.LeftColumn = schemeRelation.LeftColumn;
                    if (databaseRelation.RightColumn != schemeRelation.RightColumn) databaseRelation.RightColumn = schemeRelation.RightColumn;
                    if (databaseRelation.Type != schemeRelation.Type) databaseRelation.Type = schemeRelation.Type;
                }
            }

            foreach(DbView schemeView in SchemeCommit.Views)
            {
                IEnumerable<DbView> removeRelations = SchemeCommit.Views.Where(x1 => !e.DbView.Any(x2 => x2.Id == x1.Id));
                e.DbView.Except<DbView>(removeRelations); //maže všechny pohledy, které se už nenachází ve schématu uživatele

                if (e.DbView.SingleOrDefault(x => x.Id == schemeView.Id) == null) //pokud je ve schématu uživatele vytvořen nový pohled
                {
                    e.DbView.Add(schemeView);
                }
                else
                {
                    DbView databaseView = e.DbView.SingleOrDefault(x => x.Id == schemeView.Id);

                    if (databaseView.Name != schemeView.Name) databaseView.Name = schemeView.Name;
                    if (databaseView.Query != schemeView.Query) databaseView.Query = schemeView.Query;
                }
            }
        }

        /// <exception cref="InstanceNotFoundException">Not found commit for commitId</exception>
        private AjaxTransferDbScheme GetCommit(int commitId = -1)
        {
            var result = new AjaxTransferDbScheme();
            var requestedCommit = FetchDbSchemeCommit(commitId);
            //Latest commit was requested, but there are no commits yet. Returning an empty commit.
            if (requestedCommit == null)
            {
                return new AjaxTransferDbScheme();
            }
            SetAttributesRequestCommitTables(requestedCommit, result);
            SetAttributesRequestCommitRelations(requestedCommit, result);
            SetAttributesRequestCommitViews(requestedCommit, result);

            return result;
        }
        private static HttpResponseException GetHttpInternalServerErrorResponseException(string errorMessage)
        {
            Log.Error(errorMessage);
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(errorMessage),
                ReasonPhrase = "Critical Exception"
            });
        }


        private static void SetAttributesRequestCommitViews(DbSchemeCommit requestedCommit, AjaxTransferDbScheme result)
        {
            foreach (var view in requestedCommit.Views)
            {
                result.Views.Add(new AjaxTransferDbView
                {
                    Id = view.Id,
                    Name = view.Name,
                    PositionX = view.PositionX,
                    PositionY = view.PositionY,
                    Query = view.Query,
                });
            }
        }

        private static void SetAttributesRequestCommitRelations(DbSchemeCommit requestedCommit, AjaxTransferDbScheme result)
        {
            foreach (var relation in requestedCommit.Relations)
            {
                result.Relations.Add(new AjaxTransferDbRelation
                {
                    LeftColumn = relation.LeftColumn,
                    LeftTable = relation.LeftTable,
                    RightColumn = relation.RightColumn,
                    RightTable = relation.RightTable,
                    Type = relation.Type
                });
            }
        }

        private static void SetAttributesRequestCommitTables(DbSchemeCommit requestedCommit, AjaxTransferDbScheme result)
        {
            foreach (var table in requestedCommit.Tables)
            {
                var ajaxTable = new AjaxTransferDbTable
                {
                    Id = table.Id,
                    Name = table.Name,
                    PositionX = table.PositionX,
                    PositionY = table.PositionY
                };
                foreach (var column in table.Columns)
                {
                    ajaxTable.Columns.Add(new AjaxTransferDbColumn
                    {
                        AllowNull = column.AllowNull,
                        ColumnLength = column.ColumnLength,
                        ColumnLengthIsMax = column.ColumnLengthIsMax,
                        DefaultValue = column.DefaultValue,
                        Id = column.Id,
                        Name = column.Name,
                        PrimaryKey = column.PrimaryKey,
                        Type = column.Type,
                        Unique = column.Unique,
                    });
                }
                foreach (var index in table.Indices)
                {
                    ajaxTable.Indices.Add(new AjaxTransferDbIndex
                    {
                        ColumnNames = index.ColumnNames.Split(',').ToList(),
                        Id = index.Id,
                        Name = index.Name,
                        Unique = index.Unique,
                    });
                }
                result.Tables.Add(ajaxTable);
            }
        }
        /// <exception cref="InstanceNotFoundException">Not found commit for commitId</exception>
        private DbSchemeCommit FetchDbSchemeCommit(int commitId)
        {
            DbSchemeCommit requestedCommit;
            try
            {
                requestedCommit = commitId == -1
                    ? RepositoryDbSchemeCommit.Get(orderBy: q => q.OrderByDescending(d => d.Timestamp)).FirstOrDefault()
                    : RepositoryDbSchemeCommit.FindObjectById(commitId);
            }
            catch (InvalidOperationException ex)
            {
                if (commitId == -1)
                {
                    Log.Info("DatabaseDesigner: latest commit was requested, but there are no commits yet. Returning an empty commit.");
                    return null;
                }
                var errorString = Format("DatabaseDesigner: the requested commit with id={1} doesn't exist. " + "Exception message: {0}",
                        ex.Message, commitId);
                Log.Error(errorString);
                throw new InstanceNotFoundException(errorString);
            }
            return requestedCommit;
        }
    }
}