using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using FSPOC.Models;
using FSPOC.DAL;
using Logger;

namespace FSPOC.Controllers
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
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when loading the workflow list (GET api/workflows). Exception message: {0}", ex.Message));
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
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when loading the most recently saved workflow (GET api/workflows/last-used)."
                    + "Exception message: {0}", ex.Message));
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
                    var commits = from c in workflow.WorkflowCommits orderby c.Timestamp descending select c;
                    foreach (WorkflowCommit c in commits)
                        transferHistory.Add(new AjaxTransferCommitHeader
                        {
                            Id = c.Id,
                            CommitMessage = c.CommitMessage,
                            TimeString = c.Timestamp.ToString("d. M. yyyy H:mm:ss")
                        });
                }

                return Json(transferHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when loading commit history for workflow with id={1} "
                    + "(GET api/workflows/{1}/commits). Exception message: {0}", ex.Message, workflowId));
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
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when creating a new workflow (POST api/workflows). "
                    + "Exception message: {0}", ex.Message));
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
                        WorkflowCommit newCommit = new WorkflowCommit
                        {
                            CommitMessage = postData.CommitMessage,
                            Timestamp = DateTime.Now,
                            Activities = new List<Activity>()
                        };
                        Workflow workflow = (from w in context.Workflows where w.Id.Equals(workflowId) select w).First();
                        workflow.WorkflowCommits.Add(newCommit);

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
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when saving state of the workflow with id={1} "
                    + "(POST api/workflows/{workflowId:int}/commits). Exception message: {0}", ex.Message, workflowId));
                return new HttpStatusCodeResult(500);
            }
        }

        private ActionResult getCommit(int workflowId, int commitId = -1)
        {
            AjaxTransferWorkflowSate transferState = new AjaxTransferWorkflowSate();
            List<AjaxTransferConnection> connections = new List<AjaxTransferConnection>();
            using (var context = new WorkflowDbContext())
            {
                Workflow workflow = (from w in context.Workflows where w.Id.Equals(workflowId) select w).First();
                WorkflowCommit requestedCommit = new WorkflowCommit();
                try
                {
                    if (commitId == -1) // No commitId specified, get the lates commit by default
                        requestedCommit = (from c in workflow.WorkflowCommits where c.Workflow.Id.Equals(workflowId) orderby c.Timestamp descending select c).First();
                    else
                        requestedCommit = (from c in workflow.WorkflowCommits where c.Workflow.Id.Equals(workflowId) && c.Id.Equals(commitId) select c).First();
                }
                catch(InvalidOperationException ex) // Workflow history is empty, no commits yet
                {
                    if (commitId == -1)
                    {
                        Log.Info(String.Format("WorkflowDesigner: latest commit was requested for workflow with id={0} which has no history. Returning an empty commit.", workflowId));
                        return Json(new AjaxTransferWorkflowSate(), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Log.Error(String.Format("WorkflowDesigner: the requested commit doesn't exist. Workflow with id={1} doesn't have a commit with id={2} "
                            + "Exception message: {0}", ex.Message, workflowId, commitId));
                        return new HttpStatusCodeResult(404);
                    }
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

        [Route("api/workflows/{workflowId:int}/commits/latest")]
        [HttpGet]
        public ActionResult GetLatestCommit(int workflowId)
        {
            try
            {
                return getCommit(workflowId);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when loading the latest commit from workflow with id={1} "
                    + "(GET api/workflows/{1}/commits/latest). Exception message: {0}", ex.Message, workflowId));
                return new HttpStatusCodeResult(500);
            }
        }

        [Route("api/workflows/{workflowId:int}/commits/{commitId:int}")]
        [HttpGet]
        public ActionResult GetCommitById(int workflowId, int commitId)
        {
            try
            {
                return getCommit(workflowId, commitId);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("WorkflowDesigner: error when loading commit with id={2} from workflow with id={1} "
                    + "(GET api/workflows/{1}/commits/{2}). Exception message: {0}", ex.Message, workflowId, commitId));
                return new HttpStatusCodeResult(500);
            }
        }
    }
}