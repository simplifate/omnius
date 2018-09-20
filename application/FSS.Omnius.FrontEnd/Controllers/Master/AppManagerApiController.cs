using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.CORE;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using FSS.Omnius.Modules.Entitron.Entity.Tapestry;
using Logger;

namespace FSS.Omnius.Controllers.Tapestry
{
    public class AppManagerApiController : ApiController
    {
        [System.Web.Mvc.PersonaAuthorize]
        [Route("api/master/apps/{appId}/properties")]
        [HttpGet]
        public AjaxAppProperties LoadAppProperties(int appId)
        {
            try
            {
                DBEntities context = COREobject.i.Context;
                Application app = context.Applications.Include("CSSTemplate").FirstOrDefault(a => a.Id == appId);
                AjaxAppProperties result = new AjaxAppProperties
                {
                    Id = app.Id,
                    DisplayName = app.DisplayName,
                    TileWidth = app.TileWidth,
                    TileHeight = app.TileHeight,
                    Color = app.Color,
                    Icon = app.Icon,
                    IsAllowedForAll = app.IsAllowedForAll,
                    IsAllowedGuests = app.IsAllowedGuests
                };
                return result;
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error loading app properties (GET api/master/apps/{appId}/properties). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/master/apps/{appId}/properties")]
        [HttpPost]
        public void SaveAppProperties(int appId, AjaxAppProperties postData)
        {
            try
            {
                DBEntities context = COREobject.i.Context;
                Application app = context.Applications.Where(a => a.Id == appId).First();
                app.DisplayName = postData.DisplayName;
                app.TileWidth = postData.TileWidth;
                app.TileHeight = postData.TileHeight;
                app.Color = postData.Color;
                app.Icon = postData.Icon;
                app.IsAllowedForAll = postData.IsAllowedForAll;
                app.IsAllowedGuests = postData.IsAllowedGuests;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error changing app properties (POST api/master/apps/{appId}/properties) Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/master/apps")]
        [HttpPost]
        public void AddNewApp(AjaxAppProperties postData)
        {
            try
            {
                DBEntities context = COREobject.i.Context;
                var newApp = new Application
                {
                    Name = postData.DisplayName.RemoveDiacritics(),
                    DisplayName = postData.DisplayName,
                    TileWidth = postData.TileWidth,
                    TileHeight = postData.TileHeight,
                    Color = postData.Color,
                    Icon = postData.Icon
                };
                context.Applications.Add(newApp);
                context.SaveChanges();
                var newRootMetablock = new TapestryDesignerMetablock
                {
                    Name = "Root metablock",
                    ParentApp = newApp
                };
                context.TapestryDesignerMetablocks.Add(newRootMetablock);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error creating new application (POST api/master/apps). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        [Route("api/master/apps/{appId}/state")]
        [HttpPost]
        public void SetEnabaled(int appId, AjaxAppState postData)
        {
            try
            {
                DBEntities context = COREobject.i.Context;
                Application app = context.Applications.Where(a => a.Id == appId).First();
                app.IsEnabled = postData.IsEnabled;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error setting app state (POST api/master/apps/{appId}/state). Exception message: {ex.Message}";
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
        [Route("api/master/apps/{appName}/saveAppPosition")]
        [HttpPost]
        public void SaveAppPosition(string appName, AjaxAppCoordinates coords)
        {
            try
            {
                COREobject core = COREobject.i;
                DBEntities db = core.Context;
                User currentUser = core.User;
                var app = db.Applications.SingleOrDefault(a => a.Name == appName);
                if (app != null) {
                    UsersApplications uapp = app.UsersApplications.SingleOrDefault(ua => ua.UserId == currentUser.Id);
                    if (uapp != null)
                    {
                        uapp.PositionX = coords.positionX;
                        uapp.PositionY = coords.positionY;
                    }
                    else
                    {
                        uapp = new UsersApplications
                        {
                            UserId = currentUser.Id,
                            ApplicationId = app.Id,
                            PositionX = coords.positionX,
                            PositionY = coords.positionY
                        };
                        db.UsersApplications.Add(uapp);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error saving app position (POST api/master/apps/{appName}/saveAppPosition). Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
    }
}
