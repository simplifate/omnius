using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using Logger;

namespace FSS.Omnius.FrontEnd.Controllers.Persona
{
    [System.Web.Mvc.PersonaAuthorize(NeedsAdmin = true, Module = "Persona")]
    public class PersonaApiController : ApiController
    {
        [Route("api/persona/module-permissions")]
        [HttpPost]
        public void SavePermissions(AjaxModuleAccessPermissionSettings postData)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    foreach(AjaxModuleAccessPermission ajaxPermission in postData.PermissionList)
                    {
                        var permission = context.ModuleAccessPermissions.Find(ajaxPermission.UserId);

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
                using (var context = DBEntities.instance)
                {
                    AjaxModuleAccessPermissionSettings result = new AjaxModuleAccessPermissionSettings();
                    result.PermissionList = context.ModuleAccessPermissions.Select(c => new AjaxModuleAccessPermission
                    {
                        UserId = c.UserId,
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

        [Route("api/Persona/app-roles/{appId}")]
        [HttpGet]
        public AjaxPersonaAppRoles LoadAppRoles(int appId)
        {
            try {
                using (var context = DBEntities.instance) {
                    AjaxPersonaAppRoles result = new AjaxPersonaAppRoles();

                    result.Roles = context.Roles.Where(r => r.ApplicationId == appId).Select(r => new AjaxPersonaAppRoles_Role()
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList();
                    return result;
                }
            }
            catch (Exception ex) {
                string errorMessage = $"Persona: error loading app roles (GET api/persona/app-roles). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/Persona/app-states/{appId}")]
        [HttpGet]
        public AjaxPersonaAppstates LoadAppStates(int appId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    AjaxPersonaAppstates result = new AjaxPersonaAppstates();
                    var entitron = new CORE().Entitron;
                    entitron.AppId = appId;
                    var table = entitron.GetDynamicTable("WF_states");
                    if (table != null)
                    {
                        var statesList =
                            table.Select().ToList();

                        foreach (var state in statesList)
                        {
                            result.States.Add(new AjaxPersonaAppRoles_State()
                            {
                                Id = Convert.ToInt32(state["id"]),
                                Name = Convert.ToString(state["name"])
                            });
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Persona: error loading app states (GET api/persona/app-states). Exception message: {ex.Message}";
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
