using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.FSPOC.BussinesObjects.DAL;
using FSS.FSPOC.BussinesObjects.Entities.Workflow;
using FSS.FSPOC.BussinesObjects.Service;
using Logger;
using static System.String;

namespace FSPOC_WebProject.Controllers
{
    public class WorkflowController : ApiController
    {
        public WorkflowController(IWorkflowService workflowService,
            IRepository<Workflow> workflowRepository,
            IUnitWork unitWork
            )
        {
            if (workflowService == null) throw new ArgumentNullException(nameof(workflowService));
            if (workflowRepository == null) throw new ArgumentNullException(nameof(workflowRepository));
            if (unitWork == null) throw new ArgumentNullException(nameof(unitWork));

            WorkflowService   = workflowService;
            WorkfloRepository = workflowRepository;
            UnitWork          = unitWork;
        }

        private IWorkflowService WorkflowService { get; set; }
        private IRepository<Workflow> WorkfloRepository { get; set; }
        private IUnitWork UnitWork { get; set; }

        [Route("api/workflows")]
        [HttpGet]
        public IEnumerable<AjaxTransferWorkflowHeader> GetWorkflowList()
        {
            try
            {
                return WorkfloRepository.Get(orderBy: q => q.OrderByDescending(w => w.LastChangeTime))
                    .Select(w => new AjaxTransferWorkflowHeader
                    {
                        Id           = w.Id,
                        Name         = w.Name,
                        CreationTime = w.CreationTime
                    });

            }
            catch (Exception ex)
            {
                var errorMessage = $"WorkflowDesigner: error when loading the workflow list (GET api/workflows). Exception message: {ex.Message}";
                Log.Error(errorMessage);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(errorMessage),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }



        [Route("api/workflow/lastUsed")]
        [HttpGet]
        public AjaxTransferWorkflowHeader Get()
        {

            return WorkfloRepository.Get(orderBy: q => q.OrderBy(w => w.LastChangeTime)).Select(w=> new AjaxTransferWorkflowHeader
            {
                Id           = w.Id,
                Name         = w.Name,
                CreationTime = w.CreationTime
            }).First();

        }

        [Route("api/workflows/commits/{workflowId}")]
        [HttpGet]
        public IEnumerable<AjaxTransferCommitHeader> GetCommitList(int workflowId)
        {
            try
            {
                var workflow = WorkfloRepository.Get(w => w.Id == workflowId, includeProperties: "WorkflowCommits").First();
                return workflow.WorkflowCommits.OrderByDescending(c => c.Timestamp).Select(c => new AjaxTransferCommitHeader
                {
                    Id            = c.Id,
                    CommitMessage = c.CommitMessage,
                    TimeCommit    = c.Timestamp
                });

            }
            catch (Exception ex)
            {
                var errorMessage = Format("WorkflowDesigner: error when loading commit history for workflow with id={1} "
                                              + "(GET api/workflows/{1}/commits). Exception message: {0}", ex.Message, workflowId);
                Log.Error(errorMessage);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content      = new StringContent(errorMessage),
                    ReasonPhrase = "Critical Exception"
                });

            }
        }


        [Route("api/workflows/commits/latest/{workflowId}")]
        [HttpGet]
        public AjaxTransferWorkflowSate GetLatestCommit(int workflowId)
        {
            try
            {
                return getCommit(workflowId);
            }
            catch (InstanceNotFoundException e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content      = new StringContent(e.Message),
                    ReasonPhrase = "Critical Exception"
                });

            }
            catch (Exception ex)
            {
                var errorString = Format("WorkflowDesigner: error when loading the latest commit from workflow with id={1} "
                                  + "(GET api/workflows/{1}/commits/latest). Exception message: {0}", ex.Message,
                                   workflowId);
                Log.Error(errorString);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content      = new StringContent(errorString),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }

        [Route("api/workflows/commits/{commitId}/{workflowId}")]
        [HttpGet]
        public AjaxTransferWorkflowSate GetCommitById(int commitId, int workflowId)
        {
            try
            {
                return getCommit(workflowId, commitId);
            }
            catch (InstanceNotFoundException e)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content      = new StringContent(e.Message),
                    ReasonPhrase = "Critical Exception"
                });

            }
            catch (Exception ex)
            {
                var errorString = Format("WorkflowDesigner: error when loading the latest commit from workflow with id={1} "
                                  + "(GET api/workflows/{1}/commits/latest). Exception message: {0}", ex.Message,
                                   workflowId);
                Log.Error(errorString);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content      = new StringContent(errorString),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }


        [Route("api/workflows")]
        [HttpPost]
        public AjaxTransferWorkflowHeader CreateWorkflow([FromBody] AjaxTransferWorkflowHeader postData)
        {
            try
            {
                var saveTime    = DateTime.Now;
                var newWorkflow = new Workflow
                {
                    Name           = postData.Name,
                    CreationTime   = saveTime,
                    LastChangeTime = saveTime
                };
                WorkfloRepository.AddObject(newWorkflow);
                UnitWork.SaveChanges();
                return new AjaxTransferWorkflowHeader
                {
                    Id           = newWorkflow.Id,
                    Name         = newWorkflow.Name,
                    CreationTime = newWorkflow.CreationTime
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "WorkflowDesigner: error when creating a new workflow (POST api/workflows). " +
                                   $"Exception message: {ex.Message}";
                Log.Error(errorMessage);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(errorMessage),
                    ReasonPhrase = "Critical Exception"
                });
            }
        }



        [Route("api/workflows/commits")]
        [HttpPost]
        public void SaveToExistingWorkflow([FromBody] AjaxTransferWorkflowSate postData)
        {
            try
            {
                if (postData.Activities == null) return;
                using (var context = new OmniusDbContext())
                {
                    WorkflowCommit newCommit = new WorkflowCommit
                    {
                        CommitMessage = postData.CommitMessage,
                        Timestamp = DateTime.Now,
                        Activities = new List<Activity>()
                    };
                    Workflow workflow = (from w in context.Workflows where w.Id.Equals(postData.WorkflowId) select w).First();
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

                            newCommit.Activities.First(a => a.Id == source).Outputs.Add(new Output
                            {
                                Target = target,
                                SourceSlot = postData.Connections[i].SourceSlot,
                                TargetSlot = postData.Connections[i].TargetSlot
                            });
                            newCommit.Activities.First(a => a.Id == target).Inputs.Add(new Input { Source = source, Slot = postData.Connections[i].TargetSlot });
                        }
                    }
                    workflow.LastChangeTime = DateTime.Now;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var errorMessage =
                    Format("WorkflowDesigner: error when saving state of the workflow with id={1} "
                                  + "(POST api/workflows/commits). Exception message: {0}", ex.Message,postData.WorkflowId);
                Log.Error(errorMessage);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(errorMessage),
                    ReasonPhrase = "Critical Exception"
                });
            }

        }


        private AjaxTransferWorkflowSate getCommit(int workflowId, int commitId = -1)
        {
            var transferState = new AjaxTransferWorkflowSate();
            using (var context = new OmniusDbContext())
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
                catch (InvalidOperationException ex) // Workflow history is empty, no commits yet
                {
                    if (commitId == -1)
                    {
                        Log.Info(Format("WorkflowDesigner: latest commit was requested for workflow with id={0} which has no history. Returning an empty commit.", workflowId));
                        return new AjaxTransferWorkflowSate();
                    }
                    else
                    {
                        var errorString = Format(
                                "WorkflowDesigner: the requested commit doesn't exist. Workflow with id={1} doesn't have a commit with id={2} "
                                + "Exception message: {0}", ex.Message, workflowId, commitId);
                        Log.Error(errorString);
                        throw new InstanceNotFoundException(errorString);
                    }
                }
                transferState.CommitMessage = requestedCommit.CommitMessage;

                foreach (var item in requestedCommit.Activities)
                {
                    transferState.Activities.Add(new AjaxTransferActivity
                    {
                        Id = item.Id,
                        ActType = item.Type,
                        PositionX = item.PositionX,
                        PositionY = item.PositionY
                    });

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
            return transferState;
        }

    }
}