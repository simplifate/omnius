using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Persona;
using System.Web;

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
                using (var context = new DBEntities())
                {
                    Application app = context.Applications.First(a => a.Id == appId);
                    AjaxAppProperties result = new AjaxAppProperties
                    {
                        Id = app.Id,
                        DisplayName = app.DisplayName,
                        TileWidth = app.TileWidth,
                        TileHeight = app.TileHeight,
                        Color = app.Color,
                        Icon = app.Icon
                    };
                    return result;
                }
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
                using (var context = new DBEntities())
                {
                    Application app = context.Applications.Where(a => a.Id == appId).First();
                    app.DisplayName = postData.DisplayName;
                    app.TileWidth = postData.TileWidth;
                    app.TileHeight = postData.TileHeight;
                    app.Color = postData.Color;
                    app.Icon = postData.Icon;
                    context.SaveChanges();
                }
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
                using (var context = new DBEntities())
                {
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
                }
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
                using (var context = new DBEntities())
                {
                    Application app = context.Applications.Where(a => a.Id == appId).First();
                    app.IsEnabled = postData.IsEnabled;
                    context.SaveChanges();
                }
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
                DBEntities db = new DBEntities();
                Modules.CORE.CORE core = new Modules.CORE.CORE();
                User currentUser = User.GetLogged(core);
                var app = db.Applications.SingleOrDefault(a => a.Name == appName);
                if (app != null) {
                    UsersApplications uapp = app.UsersApplications.SingleOrDefault(ua => ua.UserId == currentUser.Id);
                    if (uapp != null)
                    {
                        uapp.PositionX = Convert.ToInt32(coords.positionX.Substring(0, coords.positionX.Length - 2));
                        uapp.PositionY = Convert.ToInt32(coords.positionY.Substring(0, coords.positionY.Length - 2));
                    }
                    else
                    {
                        uapp = new UsersApplications
                        {
                            UserId = currentUser.Id,
                            ApplicationId = app.Id,
                            PositionX = Convert.ToInt32(coords.positionX),
                            PositionY = Convert.ToInt32(coords.positionY)
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
