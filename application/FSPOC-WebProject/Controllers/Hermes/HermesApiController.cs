using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FSPOC_WebProject.Controllers.Hermes
{
    public class HermesApiController : ApiController
    {
        [Route("api/hermes/{appId}/templates")]
        [HttpGet]
        public IEnumerable<HermesAjaxTransferEmailTemplate> GetTemplateList(int appId)
        {
            List<HermesAjaxTransferEmailTemplate> result = new List<HermesAjaxTransferEmailTemplate>();
            using (var context = new DBEntities()) 
            {
                foreach(var template in context.EmailTemplates.Where(t => t.AppId == appId)) {
                    result.Add(new HermesAjaxTransferEmailTemplate()
                    {
                        Id = template.Id,
                        Name = template.Name
                    });
                }
            }
            return result;
        }
    }
}
