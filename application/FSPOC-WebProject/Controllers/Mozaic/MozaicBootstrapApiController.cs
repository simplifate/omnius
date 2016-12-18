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
                    /*foreach (var ajaxComponent in postData.Components)
                        newPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent, newPage, null));*/

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

        /*[Route("api/mozaic-editor/apps/{appId}/pages/{pageId}")]
        [HttpGet]
        public AjaxMozaicEditorPage GetPage(int appId, int pageId)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    var requestedPage = context.MozaicEditorPages.Find(pageId);

                    // TODO: replace with real error checking
                    if (requestedPage == null)
                        return new AjaxMozaicEditorPage();

                    var result = new AjaxMozaicEditorPage
                    {
                        Id = requestedPage.Id,
                        Name = requestedPage.Name ?? "Nepojmenovaná stránka",
                        IsModal = requestedPage.IsModal,
                        ModalWidth = requestedPage.ModalWidth,
                        ModalHeight = requestedPage.ModalHeight
                    };
                    foreach (var component in requestedPage.Components.Where(c => c.ParentComponent == null))
                    {
                        var ajaxComponent = new AjaxMozaicEditorComponent
                        {
                            Id = component.Id,
                            Name = component.Name ?? "",
                            Type = component.Type,
                            PositionX = component.PositionX,
                            PositionY = component.PositionY,
                            Width = component.Width,
                            Height = component.Height,
                            Tag = component.Tag,
                            Attributes = component.Attributes,
                            Classes = component.Classes ?? "",
                            Styles = component.Styles ?? "",
                            Content = component.Content,
                            Label = component.Label,
                            Placeholder = component.Placeholder,
                            TabIndex = component.TabIndex,
                            Properties = component.Properties ?? ""
                        };
                        if (component.ChildComponents.Count > 0)
                        {
                            ajaxComponent.ChildComponents = new List<AjaxMozaicEditorComponent>();
                            foreach (var childComponent in component.ChildComponents)
                            {
                                ajaxComponent.ChildComponents.Add(new AjaxMozaicEditorComponent
                                {
                                    Id = childComponent.Id,
                                    Name = childComponent.Name ?? "",
                                    Type = childComponent.Type,
                                    PositionX = childComponent.PositionX,
                                    PositionY = childComponent.PositionY,
                                    Width = childComponent.Width,
                                    Height = childComponent.Height,
                                    Tag = childComponent.Tag,
                                    Attributes = childComponent.Attributes,
                                    Classes = childComponent.Classes ?? "",
                                    Styles = childComponent.Styles ?? "",
                                    Content = childComponent.Content,
                                    Label = childComponent.Label,
                                    Placeholder = childComponent.Placeholder,
                                    TabIndex = childComponent.TabIndex,
                                    Properties = childComponent.Properties
                                });
                            }
                        }
                        result.Components.Add(ajaxComponent);
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
                var errorMessage = $"Mozaic editor: error loading page with id={pageId} (GET api/mozaic-editor/apps/{appId}/pages/{pageId}) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        
        [Route("api/mozaic-editor/apps/{appId}/pages/{pageId}")]
        [HttpPost]
        public void ReplacePage(int appId, int pageId, AjaxMozaicEditorPage postData)
        {
            try
            {
                using (var context = DBEntities.instance)
                {
                    var app = context.Applications.Find(appId);
                    app.MozaicChangedSinceLastBuild = true;
                    app.TapestryChangedSinceLastBuild = true;
                    var requestedPage = context.MozaicEditorPages.Find(pageId);
                    deleteComponents(requestedPage, context);
                    requestedPage.Name = postData.Name;
                    requestedPage.IsModal = postData.IsModal;
                    requestedPage.ModalWidth = postData.ModalWidth;
                    requestedPage.ModalHeight = postData.ModalHeight;
                    foreach (var ajaxComponent in postData.Components)
                        requestedPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent, requestedPage, null));
                    requestedPage.IsDeleted = false;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Mozaic editor: error replacing page with id={pageId} (POST api/mozaic-editor/apps/{appId}/pages/{pageId}) " +
                    $"Exception message: {ex.Message}";
                throw GetHttpInternalServerErrorResponseException(errorMessage);
            }
        }
        
        private MozaicEditorComponent convertAjaxComponentToDbFormat(AjaxMozaicEditorComponent ajaxComponent,
            MozaicEditorPage page, MozaicEditorComponent parentComponent)
        {
            var newComponent = new MozaicEditorComponent
            {
                Name = ajaxComponent.Name,
                Type = ajaxComponent.Type,
                PositionX = ajaxComponent.PositionX,
                PositionY = ajaxComponent.PositionY,
                Width = ajaxComponent.Width,
                Height = ajaxComponent.Height,
                Tag = ajaxComponent.Tag,
                Attributes = ajaxComponent.Attributes,
                Classes = ajaxComponent.Classes,
                Styles = ajaxComponent.Styles,
                Content = ajaxComponent.Content,
                Label = ajaxComponent.Label,
                Placeholder = ajaxComponent.Placeholder,
                TabIndex = ajaxComponent.TabIndex,
                Properties = ajaxComponent.Properties,
                MozaicEditorPage = page
            };
            if (ajaxComponent.ChildComponents != null && ajaxComponent.ChildComponents.Count > 0)
            {
                newComponent.ChildComponents = new List<MozaicEditorComponent>();
                foreach (var ajaxChildComponent in ajaxComponent.ChildComponents)
                    newComponent.ChildComponents.Add(convertAjaxComponentToDbFormat(ajaxChildComponent, page, newComponent));
            }
            return newComponent;
        }
        private void deleteComponents(MozaicEditorPage page, DBEntities context)
        {
            var componentList = new List<MozaicEditorComponent>();
            foreach (var component in page.Components.Where(c => c.ParentComponent == null))
                componentList.Add(component);
            foreach (var component in componentList)
            {
                var childComponentList = new List<MozaicEditorComponent>();
                if (component.ChildComponents != null)
                {
                    foreach (var childComponent in component.ChildComponents)
                        childComponentList.Add(childComponent);
                    foreach (var childComponent in childComponentList)
                    {
                        component.ChildComponents.Remove(childComponent);
                        context.Entry(childComponent).State = EntityState.Deleted;
                    }
                }
                page.Components.Remove(component);
                context.Entry(component).State = EntityState.Deleted;
            }
            context.SaveChanges();
        }*/
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
