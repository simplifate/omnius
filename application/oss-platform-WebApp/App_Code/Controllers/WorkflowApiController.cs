using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSSWorkflowDesigner.Models;
using FSSWorkflowDesigner.DAL;

namespace FSSWorkflowDesigner.Controllers
{
    public class WorkflowApiController : Controller
    {
        [Route("api/commits")]
        [HttpGet]
        public ActionResult GetCommitList()
        {
            AjaxTransferHistory transferHistory = new AjaxTransferHistory();
            using (var context = new WorkflowDbContext())
            {
                var commits = from c in context.Commits orderby c.Timestamp descending select c;
                foreach (Commit c in commits)
                    transferHistory.CommitHeaders.Add(new AjaxTransferCommitHeader
                    {
                        Id = c.Id,
                        CommitMessage = c.CommitMessage,
                        TimeString = c.Timestamp.ToString("d. M. yyyy H:mm:ss")
                    });
            }

            return Json(transferHistory, JsonRequestBehavior.AllowGet);
        }

        [Route("api/commits")]
        [HttpPost]
        public ActionResult Save(AjaxTransferWorkflowSate postData)
        {
            try
            {
                if (postData.Activities != null)
                {
                    using (var context = new WorkflowDbContext())
                    {
                        Commit newCommit = new Commit
                        {
                            CommitMessage = postData.CommitMessage,
                            Timestamp = DateTime.Now,
                            Activities = new List<Activity>()
                        };
                        context.Commits.Add(newCommit);

                        // All connection keys will be re-mapped to the primary IDs assigned to activities by the database provider
                        Dictionary<int, int> idMapping = new Dictionary<int, int>();

                        for (int i = 0; i < postData.Activities.Count; i++)
                        {
                            AjaxTransferActivity transferActivity = postData.Activities[i];

                            Activity storedActivity = new Activity
                            {
                                Type = transferActivity.ActType,
                                PositionX = transferActivity.PositionX,
                                PositionY = transferActivity.PositionY
                            };

                            newCommit.Activities.Add(storedActivity);
                            context.SaveChanges();
                            idMapping.Add(i, storedActivity.Id);
                        }

                        if (postData.Connections != null)
                        {
                            for (int i = 0; i < postData.Connections.Count; i++)
                            {
                                int source = idMapping[postData.Connections[i].Source];
                                int target = idMapping[postData.Connections[i].Target];
                                
                                newCommit.Activities.First(a => a.Id == source).Outputs.Add(new Output { Target = target,
                                    SourceSlot = postData.Connections[i].SourceSlot, TargetSlot = postData.Connections[i].TargetSlot });
                                newCommit.Activities.First(a => a.Id == target).Inputs.Add(new Input { Source = source, Slot = postData.Connections[i].TargetSlot });
                            }
                        }
                        context.SaveChanges();
                    }
                }
                return new HttpStatusCodeResult(200);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        private ActionResult getCommit(bool getLatest, int commitId = -1)
        {
            try
            {
                AjaxTransferWorkflowSate transferState = new AjaxTransferWorkflowSate();
                List<AjaxTransferConnection> connections = new List<AjaxTransferConnection>();
                using (var context = new WorkflowDbContext())
                {
                    Commit requestedCommit;
                    if(getLatest)
                        requestedCommit = (from c in context.Commits orderby c.Timestamp descending select c).First();
                    else
                        requestedCommit = (from c in context.Commits where c.Id.Equals(commitId) select c).First();
                    transferState.CommitMessage = requestedCommit.CommitMessage;

                    foreach (var item in requestedCommit.Activities)
                    {
                        transferState.Activities.Add(new AjaxTransferActivity { Id = item.Id, ActType = item.Type,
                            PositionX = item.PositionX, PositionY = item.PositionY });

                        foreach (var output in item.Outputs)
                        {
                            AjaxTransferConnection currentConnection = new AjaxTransferConnection
                            {
                                Source = item.Id,
                                SourceSlot = output.SourceSlot,
                                Target = output.Target,
                                TargetSlot = output.TargetSlot
                            };
                            transferState.Connections.Add(currentConnection);
                        }
                    }
                }
                return Json(transferState, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/commits/latest")]
        [HttpGet]
        public ActionResult GetLatestCommit()
        {
            return getCommit(true);
        }

        [Route("api/commits/{commitId:int}")]
        [HttpGet]
        public ActionResult GetCommitById(int commitId)
        {
            return getCommit(false, commitId);
        }
    }
}