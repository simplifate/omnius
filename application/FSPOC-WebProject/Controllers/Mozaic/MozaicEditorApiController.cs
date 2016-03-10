using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using FSS.Omnius.Modules.Entitron.Entity;
using FSS.Omnius.Modules.Entitron.Entity.Mozaic;
using System.Data.Entity;

namespace FSPOC_WebProject.Controllers.Mozaic
{
    public class MozaicEditorApiController : ApiController
    {
        [Route("api/mozaic-editor/apps/{appId}/pages")]
        [HttpGet]
        public IEnumerable<AjaxMozaicEditorPageHeader> GetAppPageList(int appId)
        {
            var result = new List<AjaxMozaicEditorPageHeader>();
            using (var context = new DBEntities())
            {
                foreach(var page in context.Applications.Find(appId).MozaicEditorPages)
                {
                    result.Add(new AjaxMozaicEditorPageHeader
                    {
                        Id = page.Id,
                        Name = page.Name ?? "Unnamed"
                    });
                }
            }
            return result;
        }
        [Route("api/mozaic-editor/apps/{appId}/pages/{pageId}")]
        [HttpGet]
        public AjaxMozaicEditorPage GetPage(int appId, int pageId)
        {
            using (var context = new DBEntities())
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
                foreach (var component in requestedPage.Components)
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
                        Properties = component.Properties
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
                                Properties = childComponent.Properties
                            });
                        }
                    }
                    result.Components.Add(ajaxComponent);
                }
                return result;
            }
        }
        [Route("api/mozaic-editor/apps/{appId}/pages")]
        [HttpPost]
        public int NewPage(int appId, AjaxMozaicEditorPage postData)
        {
            using (var context = new DBEntities())
            {
                var newPage = new MozaicEditorPage
                {
                    Name = postData.Name
                };
                foreach (var ajaxComponent in postData.Components)
                    newPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent));
                context.Applications.Find(appId).MozaicEditorPages.Add(newPage);
                context.SaveChanges();
                return newPage.Id;
            }
        }
        [Route("api/mozaic-editor/apps/{appId}/pages/{pageId}")]
        [HttpPost]
        public void ReplacePage(int appId, int pageId, AjaxMozaicEditorPage postData)
        {
            using (var context = new DBEntities())
            {
                var requestedPage = context.MozaicEditorPages.Find(pageId);
                deleteComponents(requestedPage, context);
                requestedPage.Name = postData.Name;
                requestedPage.IsModal = postData.IsModal;
                requestedPage.ModalWidth = postData.ModalWidth;
                requestedPage.ModalHeight = postData.ModalHeight;
                foreach (var ajaxComponent in postData.Components)
                    requestedPage.Components.Add(convertAjaxComponentToDbFormat(ajaxComponent));
                context.SaveChanges();
            }
        }

        private MozaicEditorComponent convertAjaxComponentToDbFormat(AjaxMozaicEditorComponent ajaxComponent)
        {
            var newcomponent = new MozaicEditorComponent
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
                Properties = ajaxComponent.Properties
            };
            if (ajaxComponent.ChildComponents != null && ajaxComponent.ChildComponents.Count > 0)
            {
                newcomponent.ChildComponents = new List<MozaicEditorComponent>();
                foreach (var ajaxChildComponent in ajaxComponent.ChildComponents)
                    newcomponent.ChildComponents.Add(convertAjaxComponentToDbFormat(ajaxChildComponent));
            }
            return newcomponent;
        }
        private void deleteComponents(MozaicEditorPage page, DBEntities context)
        {
            var componentList = new List<MozaicEditorComponent>();
            foreach (var component in page.Components)
                componentList.Add(component);
            foreach (var component in componentList)
                context.Entry(component).State = EntityState.Deleted;
            context.SaveChanges();
        }
    }
}
