using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Master;
using Logger;

namespace FSPOC_WebProject.Controllers.Tapestry
{
    public class AppManagerApiController : ApiController
    {
        [Route("api/master/apps/{appId}/properties")]
        [HttpGet]
        public AjaxAppProperties LoadAppProperties(int appId)
        {
            try
            {
                using (var context = new DBEntities())
                {
                    Application app = context.Applications.Where(a => a.Id == appId).First();
                    AjaxAppProperties result = new AjaxAppProperties
                    {
                        Id = app.Id,
                        Name = app.Name,
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
                    app.Name = postData.Name;
                    app.TileWidth = postData.TileWidth;
                    app.TileHeight = postData.TileHeight;
                    app.Color = postData.Color;
                    app.Icon = postData.Icon;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"App manager: error changing app properties (POST api/master/apps/{appId}/properties). Exception message: {ex.Message}";
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
    }
}
