using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Data.Entity;
using Logger;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic.Bootstrap;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    [System.Web.Mvc.PersonaAuthorize(NeedsAdmin = true, Module = "Mozaic")]
    public class MozaicBootstrapApiController : ApiController
    {
        [Route("api/mozaic-bootstrap/apps/{appId}/pages")]
        [HttpGet]
        public IEnumerable<MozaicBootstrapAjaxPageHeader> GetAppPageList(int appId)
        {
            try
            {
                var result = new List<MozaicBootstrapAjaxPageHeader>();
                using (var context = DBEntities.instance)
                {
                    foreach (var page in context.Applications.Find(appId).MozaicEditorPages.Where(p => p.IsDeleted == false).OrderBy(o => o.Name))
                    {
                        result.Add(new MozaicBootstrapAjaxPageHeader
                        {
                            Id = page.Id,
                            Name = page.Name ?? "(nepojmenovaná stránka)",
                            IsBootstrap = false
                        });
                    }
                    foreach (var page in context.Applications.Find(appId).MozaicBootstrapPages.Where(p => p.IsDeleted == false).OrderBy(o => o.Name)) 
                    {
                        result.Add(new MozaicBootstrapAjaxPageHeader
                        {
                            Id = page.Id,
                            Name = page.Name ?? "(nepojmenovaná stránka)",
                            IsBootstrap = true
                        });
                    }
                }
                return result.OrderBy(p => p.Name);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Mozaic editor: error loading page list (GET api/mozaic-bootstrap/apps/{appId}/pages) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/mozaic-bootstrap/apps/{appId}/pages")]
        [HttpPost]
        public int NewPage(int appId, MozaicBootstrapAjaxPage postData)
        {
            try {
                using (var context = DBEntities.instance) 
                {
                    var app = context.Applications.Find(appId);
                    app.MozaicChangedSinceLastBuild = true;
                    app.TapestryChangedSinceLastBuild = true;

                    var newPage = new MozaicBootstrapPage
                    {
                        Name = postData.Name,
                        IsDeleted = false,
                        Version = VersionEnum.Bootstrap
                    };

                    int numOrder = 0;
                    foreach (var ajaxComponent in postData.Components) {
                        newPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent, newPage, null, ++numOrder));
                    }

                    context.Applications.Find(appId).MozaicBootstrapPages.Add(newPage);
                    context.SaveChanges();
                    return newPage.Id;
                }
            }
            catch (Exception ex) {
                var errorMessage = $"Mozaic editor: error creating new page (POST api/mozaic-editor/apps/{appId}/pages) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/mozaic-bootstrap/apps/{appId}/deletedPages")]
        [HttpGet]
        public IEnumerable<MozaicBootstrapAjaxPageHeader> GetDeletedPageList(int appId)
        {
            try {
                var result = new List<MozaicBootstrapAjaxPageHeader>();
                using (var context = DBEntities.instance) {
                    foreach (var page in context.Applications.Find(appId).MozaicEditorPages.Where(p => p.IsDeleted).OrderBy(o => o.Name)) {
                        result.Add(new MozaicBootstrapAjaxPageHeader
                        {
                            Id = page.Id,
                            Name = page.Name ?? "(nepojmenovaná stránka)",
                            IsBootstrap = false
                        });
                    }
                    foreach (var page in context.Applications.Find(appId).MozaicBootstrapPages.Where(p => p.IsDeleted).OrderBy(o => o.Name)) {
                        result.Add(new MozaicBootstrapAjaxPageHeader
                        {
                            Id = page.Id,
                            Name = page.Name ?? "(nepojmenovaná stránka)",
                            IsBootstrap = true
                        });
                    }
                }
                return result.OrderBy(p => p.Name);
            }
            catch (Exception ex) {
                var errorMessage = $"Mozaic editor: error loading deleted page list (GET api/mozaic-bootstrap/apps/{appId}/deletedPages) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/mozaic-bootstrap/apps/{appId}/pages/{pageId}/delete")]
        [HttpPost]
        public void DeletePage(int appId, int pageId)
        {
            try {
                var context = DBEntities.instance;
                var page = context.MozaicBootstrapPages.Find(pageId);
                page.IsDeleted = true;
                context.SaveChanges();
            }
            catch (Exception ex) {
                var errorMessage = $"Mozaic editor: error deleting page with id={pageId} (POST api/mozaic-bootstrap/apps/{appId}/pages/{pageId}) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/mozaic-bootstrap/apps/{appId}/pages/{pageId}")]
        [HttpPost]
        public void ReplacePage(int appId, int pageId, MozaicBootstrapAjaxPage postData)
        {
            try {
                using (var context = DBEntities.instance) 
                {
                    var app = context.Applications.Find(appId);
                    app.MozaicChangedSinceLastBuild = true;
                    app.TapestryChangedSinceLastBuild = true;

                    var requestedPage = context.MozaicBootstrapPages.Find(pageId);
                    deleteComponents(requestedPage, context);

                    requestedPage.Name = postData.Name;
                    requestedPage.Content = postData.Content;
                    requestedPage.IsDeleted = postData.IsDeleted;

                    int numOrder = 0;
                    foreach (var ajaxComponent in postData.Components) {
                        requestedPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent, requestedPage, null, ++numOrder));
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex) {
                var errorMessage = $"Mozaic editor: error replacing page with id={pageId} (POST api/mozaic-bootstrap/apps/{appId}/pages/{pageId}) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        [Route("api/mozaic-bootstrap/apps/{appId}/pages/{pageId}")]
        [HttpGet]
        public MozaicBootstrapAjaxPage GetPage(int appId, int pageId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    var requestedPage = context.MozaicBootstrapPages.Find(pageId);

                    // TODO: replace with real error checking
                    if (requestedPage == null)
                        return new MozaicBootstrapAjaxPage();

                    var result = new MozaicBootstrapAjaxPage
                    {
                        Id = requestedPage.Id,
                        Name = requestedPage.Name ?? "Nepojmenovaná stránka",
                        Content = requestedPage.Content,
                        Components = new List<MozaicBootstrapAjaxComponent>()
                    };

                    foreach (var component in requestedPage.Components.Where(c => c.ParentComponent == null)) {
                        result.Components.Add(convertComponentToAjaxFormat(component));
                    }

                    // Renew deleted page
                    if (requestedPage.IsDeleted)
                    {                       
                        requestedPage.IsDeleted = false;
                        context.SaveChanges();
                    }
                    
                    return result;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Mozaic bootstrap editor: error loading page with id={pageId} (GET api/mozaic-bootstrap/apps/{appId}/pages/{pageId}) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }

        private MozaicBootstrapAjaxComponent convertComponentToAjaxFormat(MozaicBootstrapComponent c)
        {
            var ajaxComponent = new MozaicBootstrapAjaxComponent
            {
                Id = c.Id,
                ElmId = c.ElmId ?? "",
                Tag = c.Tag,
                UIC = c.UIC,
                Attributes = c.Attributes ?? "",
                Content = c.Content,
                Properties = c.Properties ?? "",
                ChildComponents = new List<MozaicBootstrapAjaxComponent>()
            };
            if(c.ChildComponents.Count > 0) {
                foreach(var childComponent in c.ChildComponents) {
                    ajaxComponent.ChildComponents.Add(convertComponentToAjaxFormat(childComponent));
                }
            }

            return ajaxComponent;
        }
        
        private MozaicBootstrapComponent convertAjaxComponentToDbFormat(MozaicBootstrapAjaxComponent c, MozaicBootstrapPage page, MozaicBootstrapComponent parentComponent, int numOrder)
        {
            var newComponent = new MozaicBootstrapComponent
            {
                ElmId = c.ElmId,
                Content = c.Content,
                Tag = c.Tag,
                UIC = c.UIC,
                Properties = c.Properties,
                Attributes = c.Attributes,
                MozaicBootstrapPage = page,
                NumOrder = numOrder          
            };
            if (c.ChildComponents != null && c.ChildComponents.Count > 0)
            {
                int childNumOrder = 0;
                newComponent.ChildComponents = new List<MozaicBootstrapComponent>();
                foreach (var ajaxChildComponent in c.ChildComponents)
                    newComponent.ChildComponents.Add(convertAjaxComponentToDbFormat(ajaxChildComponent, page, newComponent, ++childNumOrder));
            }
            return newComponent;
        }

        private void deleteComponents(MozaicBootstrapPage page, DBEntities context)
        {
            var componentList = new List<MozaicBootstrapComponent>();
            var allComponents = new List<MozaicBootstrapComponent>();

            foreach (var component in page.Components.Where(c => c.ParentComponent == null)) {
                componentList.Add(component);
            }

            while(componentList.Count > 0) {
                var component = componentList[0];

                allComponents.Add(component);
                
                if(component.ChildComponents != null && component.ChildComponents.Count > 0) {
                    foreach(var child in component.ChildComponents) {
                        componentList.Add(child);
                    }
                }

                context.Entry(component).State = EntityState.Deleted;
                componentList.RemoveAt(0);
            }

            foreach(var component in allComponents) {
                page.Components.Remove(component);
            }
            context.SaveChanges();
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
