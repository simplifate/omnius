using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSPOC.Models;
using FSPOC.DAL;

namespace FSPOC.Controllers
{
    public class DatabaseApiController : Controller
    {
        [Route("api/database")]
        [HttpGet]
        public ActionResult LoadScheme() 
        {
            try
            {
                AjaxTransferDbScheme result = new AjaxTransferDbScheme();
                using (var context = new WorkflowDbContext())
                {
                    var latestScheme = (from s in context.DbSchemes orderby s.Timestamp descending select s).First();
                    foreach (var table in latestScheme.Tables)
                    {
                        AjaxTransferDbTable ajaxTable = new AjaxTransferDbTable { Name = table.Name,
                            PositionX = table.PositionX, PositionY = table.PositionY };
                        foreach (var column in table.Columns)
                        {
                            ajaxTable.Columns.Add(new AjaxTransferDbColumn { Id = column.Id, Name = column.Name,
                                PrimaryKey = column.PrimaryKey, Type = column.Type });
                        }
                        result.Tables.Add(ajaxTable);
                    }
                    foreach (var relation in latestScheme.Relations)
                    {
                        result.Relations.Add(new AjaxTransferDbRelation { LeftTable = relation.LeftTable, RightTable = relation.RightTable,
                            LeftColumn = relation.LeftColumn, RightColumn = relation.RightColumn, Type = relation.Type });
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }
        [Route("api/database")]
        [HttpPost]
        public ActionResult SaveScheme(AjaxTransferDbScheme postData)
        {
            try
            {
                DbScheme scheme = new DbScheme();
                using (var context = new WorkflowDbContext())
                {
                    scheme.Timestamp = DateTime.Now;
                    context.DbSchemes.Add(scheme);
                    Dictionary<int, int> tableIdMapping = new Dictionary<int, int>();
                    Dictionary<int, int> columnIdMapping = new Dictionary<int, int>();

                    foreach (var ajaxTable in postData.Tables)
                    {
                        int ajaxTableId = ajaxTable.Id;
                        DbTable newTable = new DbTable { Name = ajaxTable.Name, PositionX = ajaxTable.PositionX, PositionY = ajaxTable.PositionY };
                        scheme.Tables.Add(newTable);
                        context.SaveChanges();
                        tableIdMapping.Add(ajaxTableId, newTable.Id);
                        foreach (var column in ajaxTable.Columns)
                        {
                            int ajaxColumnId = column.Id;
                            DbColumn newColumn = new DbColumn { Name = column.Name, PrimaryKey = column.PrimaryKey, Type = column.Type };
                            newTable.Columns.Add(newColumn);
                            context.SaveChanges();
                            columnIdMapping.Add(ajaxColumnId, newColumn.Id);
                        }
                    }
                    foreach (var ajaxRelation in postData.Relations)
                    {
                        int leftTable = tableIdMapping[ajaxRelation.LeftTable];
                        int rightTable = tableIdMapping[ajaxRelation.RightTable];
                        int leftColumn = columnIdMapping[ajaxRelation.LeftColumn];
                        int rightColumn = columnIdMapping[ajaxRelation.RightColumn];
                        scheme.Relations.Add(new DbRelation
                        {
                            LeftTable = leftTable,
                            RightTable = rightTable,
                            LeftColumn = leftColumn,
                            RightColumn = rightColumn,
                            Type = ajaxRelation.Type
                        });
                    }
                    context.SaveChanges();
                    return new HttpStatusCodeResult(200);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }
    }
}