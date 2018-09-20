using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FSS.Omnius.FrontEnd.Controllers.Hermes
{
    public class HermesApiController : ApiController
    {
        [Route("api/hermes/{appId}/templates")]
        [HttpGet]
        public IEnumerable<HermesAjaxTransferEmailTemplate> GetTemplateList(int appId)
        {
            try
            {
                List<HermesAjaxTransferEmailTemplate> result = new List<HermesAjaxTransferEmailTemplate>();
                var context = COREobject.i.Context;
                foreach (var template in context.EmailTemplates.Where(t => t.AppId == appId)) {
                    result.Add(new HermesAjaxTransferEmailTemplate()
                    {
                        Id = template.Id,
                        Name = template.Name
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Hermes: error loading template list (GET api/hermes/{appId}/templates) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
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
    }
}
