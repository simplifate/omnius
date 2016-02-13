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
                newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" ' + cData.Attributes + ' class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" ' + 'style="left: ' + cData.PositionX + 'px; top: ' + cData.PositionY + 'px; width: '
                    + cData.Width + 'px; height: ' + cData.Height + 'px; ' + cData.Styles + '"></' + cData.Tag + '>');
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
