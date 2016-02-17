function LoadMozaicPage(pageId) {
    appId = $("#currentAppId").val();
    if (pageId == "current")
        pageId = $("#currentPageId").val();
    url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function () { alert("ERROR") },
        success: function (data) {
            $("#mozaicPageContainer .uic").remove();

            for (i = 0; i < data.Components.length; i++) {
                cData = data.Components[i];
                newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" uicName="' + cData.Name + '" ' + cData.Attributes + ' class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" uicStyles="' + cData.Styles + '" style="left: ' + cData.PositionX + '; top: ' + cData.PositionY + '; width: '
                    + cData.Width + '; height: ' + cData.Height + '; ' + cData.Styles + '"></' + cData.Tag + '>');
                if (cData.Placeholder)
                    newComponent.attr("placeholder", cData.Placeholder);
                if (newComponent.hasClass("button-simple"))
                    newComponent.text(cData.Label);
                else if (newComponent.hasClass("button-dropdown"))
                    newComponent.html(cData.Label + '<i class="fa fa-caret-down"></i>');
                else if (newComponent.hasClass("info-container")) {
                    newComponent.append($('<div class="fa fa-info-circle info-container-icon"></div>'
                        + '<div class="info-container-header"></div>'
                        + '<div class="info-container-body"></div>'));
                    newComponent.find(".info-container-header").text(cData.Label);
                    newComponent.find(".info-container-body").text(cData.Content);
                }
                else if (newComponent.hasClass("form-heading") || newComponent.hasClass("control-label")) {
                    newComponent.text(cData.Label);
                }
                else if (newComponent.hasClass("checkbox-control")) {
                    newComponent.append($('<input type="checkbox" /><span class="checkbox-label">' + cData.Label + '</span>'));
                }
                else if (newComponent.hasClass("breadcrumb-navigation")) {
                    newComponent.append($('<div class="app-icon fa fa-question"></div><div class="nav-text">APP NAME &gt; Nav</div>'));
                }
                $("#mozaicPageContainer").append(newComponent);
                newComponent.draggable({
                    cancel: false,
                    containment: "parent",
                    drag: function (event, ui) {
                        if (GridResolution > 0) {
                            ui.position.left -= (ui.position.left % GridResolution);
                            ui.position.top -= (ui.position.top % GridResolution);
                        }
                    }
                });
            }
            $("#headerPageName").text(data.Name);
            pageId = $("#currentPageId").val(pageId);
        }
    });
}
