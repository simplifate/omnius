using System.Collections.Generic;
using System.Web.Http;
using FSPOC_WebProject.Models;
using FSS.Omnius.BussinesObjects.Common;
using FSS.Omnius.BussinesObjects.Service;
using Newtonsoft.Json;

namespace FSPOC_WebProject.Controllers
{
    public class ActionHandlerController : ApiController
    {
        public ActionHandlerController(IExecuteActionService executeActionService)
        {
            ExecuteActionService = executeActionService;
        }

        private IExecuteActionService ExecuteActionService { get; set; }

        [HttpPost]
        [Route("api/ActionHandler")]
        public IEnumerable<ResultAction> Post([FromBody] ActionModel sourceObject)
        {
            var serialize         = JsonConvert.SerializeObject(sourceObject.SourceObject);
            var deserializeObject = JsonConvert.DeserializeObject(serialize);

            return ExecuteActionService.RunAction(sourceObject.BlockId,deserializeObject);
        }
    }
}