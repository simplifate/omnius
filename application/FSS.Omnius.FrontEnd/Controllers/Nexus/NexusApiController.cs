using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Nexus;
using FSS.Omnius.Modules.Entitron.Entity.Hermes;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Master;

namespace FSS.Omnius.FrontEnd.Controllers.Nexus
{
    [System.Web.Mvc.PersonaAuthorize(NeedsAdmin = true, Module = "Nexus")]
    public class NexusApiController : ApiController
    {
        [Route("api/nexus/{appId}/gateways")]
        [HttpGet]
        public AjaxTransferNexusModel GetIntegrations(int appId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    Application app = context.Applications.First(a => a.Id == appId);
                    AjaxTransferNexusModel result = new AjaxTransferNexusModel();

                    foreach(Ldap item in context.Ldaps.OrderBy(l => l.Domain_Server)) {
                        result.Ldap.Add(new AjaxTransferNexusLDAP()
                        {
                            Id = item.Id,
                            Name = item.Domain_Server
                        });
                    }
                    foreach(WS item in context.WSs.OrderBy(w => w.Name)) {
                        result.WS.Add(new AjaxTrasferNexuWS()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Type = item.Type
                        });
                    }
                    foreach(WebDavServer item in context.WebDavServers.OrderBy(s => s.Name)) {
                        result.WebDAV.Add(new AjaxTransferNexusWebDAV()
                        {
                            Id = item.Id,
                            Name = item.Name
                        });
                    }
                    foreach(Smtp item in context.SMTPs.OrderBy(s => s.Name)) {
                        result.SMTP.Add(new AjaxTransferHermesSMTP()
                        {
                            Id = item.Id,
                            Name = item.Name
                        });
                    }
                    foreach(ExtDB item in context.ExtDBs.OrderBy(d => d.DB_Name)) {
                        result.ExtDB.Add(new AjaxTransferNexusExtDB()
                        {
                            Id = item.Id,
                            Name = item.DB_Name
                        });
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Nexus: error when loading integrations (GET api/nexus/{appId}/gateways). Exception message: {ex.Message}";
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

        public NexusApiController() { }
    }
}
