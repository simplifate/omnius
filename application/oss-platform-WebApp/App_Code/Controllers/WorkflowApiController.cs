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
        [Route("api/workflows")]
        [HttpGet]
        public ActionResult GetWorkflowList()
        {
            try
            {
                List<AjaxTransferWorkflowHeader> workflowHeaderList = new List<AjaxTransferWorkflowHeader>();
                using (var context = new WorkflowDbContext())
                {
                    var workflows = from w in context.Workflows orderby w.LastChangeTime descending select w;
                    foreach (Workflow w in workflows)
                        workflowHeaderList.Add(new AjaxTransferWorkflowHeader
                        {
                            Id = w.Id,
                            Name = w.Name,
                            TimeString = w.CreationTime.ToString("d. M. yyyy H:mm:ss")
                        });
                }

                return Json(workflowHeaderList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/workflows/last-used")]
        [HttpGet]
        public ActionResult GetTheWorkflowLastUsed()
        {
            try
            {
                using (var context = new WorkflowDbContext())
                {
                    var workflow = (from w in context.Workflows orderby w.LastChangeTime descending select w).First();
                    return Json(new AjaxTransferWorkflowHeader
                    {
                        Id = workflow.Id,
                        Name = workflow.Name,
                        TimeString = workflow.CreationTime.ToString("d. M. yyyy H:mm:ss")
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/workflows/{workflowId:int}/commits")]
        [HttpGet]
        public ActionResult GetCommitList(int workflowId)
        {
            try
            {
                List<AjaxTransferCommitHeader> transferHistory = new List<AjaxTransferCommitHeader>();
                using (var context = new WorkflowDbContext())
                {
                    Workflow workflow = (from w in context.Workflows where w.Id.Equals(workflowId) select w).First();
                    var commits = from c in workflow.Commits orderby c.Timestamp descending select c;
                    foreach (Commit c in commits)
                        transferHistory.Add(new AjaxTransferCommitHeader
                        {
                            Id = c.Id,
                            CommitMessage = c.CommitMessage,
                            TimeString = c.Timestamp.ToString("d. M. yyyy H:mm:ss")
                        });
                }

                return Json(transferHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/workflows")]
        [HttpPost]
        public ActionResult CreateWorkflow(AjaxTransferWorkflowHeader postData)
        {
            try
            {
                using (var context = new WorkflowDbContext())
                {
                    DateTime saveTime = DateTime.Now;
                    Workflow newWorkflow = new Workflow
                    {
                        Name = postData.Name,
                        CreationTime = saveTime,
                        LastChangeTime = saveTime
                    };
                    context.Workflows.Add(newWorkflow);
                    context.SaveChanges();
                    return Json(new AjaxTransferWorkflowHeader { Id = newWorkflow.Id, Name = newWorkflow.Name,
                        TimeString = newWorkflow.CreationTime.ToString("d. M. yyyy H:mm:ss")});
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/workflows/{workflowId:int}/commits")]
        [HttpPost]
        public ActionResult SaveToExistingWorkflow(int workflowId, AjaxTransferWorkflowSate postData)
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
                        Workflow workflow = (from w in context.Workflows where w.Id.Equals(workflowId) select w).First();
                        workflow.Commits.Add(newCommit);

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
                        workflow.LastChangeTime = DateTime.Now;
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

        private ActionResult getCommit(int workflowId, int commitId = -1)
        {
            try
            {
                AjaxTransferWorkflowSate transferState = new AjaxTransferWorkflowSate();
                List<AjaxTransferConnection> connections = new List<AjaxTransferConnection>();
                using (var context = new WorkflowDbContext())
                {
                    Workflow workflow = (from w in context.Workflows where w.Id.Equals(workflowId) select w).First();
                    Commit requestedCommit = new Commit();
                    try
                    {
                        if (commitId == -1) // No commitId specified, get the lates commit by default
                            requestedCommit = (from c in workflow.Commits where c.Workflow.Id.Equals(workflowId) orderby c.Timestamp descending select c).First();
                        else
                            requestedCommit = (from c in workflow.Commits where c.Workflow.Id.Equals(workflowId) && c.Id.Equals(commitId) select c).First();
                    }
                    catch(InvalidOperationException) // Workflow history is empty, no commits yet
                    {
                        return Json(new AjaxTransferWorkflowSate(), JsonRequestBehavior.AllowGet);
                    }
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

        [Route("api/workflows/{workflowId:int}/commits/latest")]
        [HttpGet]
        public ActionResult GetLatestCommit(int workflowId)
        {
            return getCommit(workflowId);
        }

        [Route("api/workflows/{workflowId:int}/commits/{commitId:int}")]
        [HttpGet]
        public ActionResult GetCommitById(int workflowId, int commitId)
        {
            return getCommit(workflowId, commitId);
        }
    }
}