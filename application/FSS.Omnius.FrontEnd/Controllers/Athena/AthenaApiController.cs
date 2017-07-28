using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Athena;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Logger;

namespace FSS.Omnius.FrontEnd.Controllers.Athena
{
    [System.Web.Mvc.PersonaAuthorize(NeedsAdmin = true, Module = "Athena")]
    public class AthenaApiController : ApiController
    {
        [Route("api/athena/getGraphList")]
        [HttpGet]
        public List<Graph> GetGraphList()
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    return context.Graph.ToList();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Persona: error loading module permissions (GET api/persona/module-permissions). Exception message: {ex.Message}";
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
