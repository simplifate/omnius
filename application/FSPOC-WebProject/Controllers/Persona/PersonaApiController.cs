using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Logger;

namespace FSPOC_WebProject.Controllers.Persona
{
    public class PersonaApiController : ApiController
    {
        [Route("api/persona/module-permissions")]
        [HttpPost]
        public void SavePermissions(AjaxModuleAccessPermissionSettings postData)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    foreach(AjaxModuleAccessPermission ajaxPermission in postData.PermissionList)
                    {
                        var permission = context.ModuleAccessPermissions.Find(ajaxPermission.Id);

                        permission.Core = ajaxPermission.Core;
                        permission.Master = ajaxPermission.Master;
                        permission.Tapestry = ajaxPermission.Tapestry;
                        permission.Entitron = ajaxPermission.Entitron;
                        permission.Mozaic = ajaxPermission.Mozaic;
                        permission.Persona = ajaxPermission.Persona;
                        permission.Nexus = ajaxPermission.Nexus;
                        permission.Sentry = ajaxPermission.Sentry;
                        permission.Hermes = ajaxPermission.Hermes;
                        permission.Athena = ajaxPermission.Athena;
                        permission.Watchtower = ajaxPermission.Watchtower;
                        permission.Cortex = ajaxPermission.Cortex;
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Persona: error saving module permissions (POST api/persona/module-permissions). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/persona/module-permissions")]
        [HttpGet]
        public AjaxModuleAccessPermissionSettings LoadPermissions()
        {
            try
            {
                using (var context = new DBEntities())
                {
                    AjaxModuleAccessPermissionSettings result = new AjaxModuleAccessPermissionSettings();
                    result.PermissionList = context.ModuleAccessPermissions.Select(c => new AjaxModuleAccessPermission
                    {
                        Id = c.Id,
                        UserName = c.User.DisplayName,
                        Core = c.Core,
                        Master = c.Master,
                        Tapestry = c.Tapestry,
                        Entitron = c.Entitron,
                        Mozaic = c.Mozaic,
                        Persona =c.Persona,
                        Nexus = c.Nexus,
                        Sentry = c.Sentry,
                        Hermes = c.Hermes,
                        Athena = c.Athena,
                        Watchtower = c.Watchtower,
                        Cortex = c.Cortex
                    }).ToList();
                    return result;
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
