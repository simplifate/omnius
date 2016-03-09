function LoadMozaicPage(pageId) {
    appId = $("#currentAppId").val();
    if (pageId == "current")
        pageId = $("#currentPageId").val();
    url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function (request, status, error) {
            alert(request.responseText);
        },
        success: function (data) {
            $("#mozaicPageContainer .uic").remove();
            $("#mozaicPageContainer .dataTables_wrapper").remove();
            $("#mozaicPageContainer .color-picker").remove();

            for (i = 0; i < data.Components.length; i++) {
                cData = data.Components[i];
                newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" uicName="' + cData.Name + '" ' + cData.Attributes + ' class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" uicStyles="' + cData.Styles + '" style="left: ' + cData.PositionX + '; top: ' + cData.PositionY + '; width: '
                    + cData.Width + '; height: ' + cData.Height + '; ' + cData.Styles + '"></' + cData.Tag + '>');
                $("#mozaicPageContainer").append(newComponent);
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
                else if (newComponent.hasClass("radio-control")) {
                    newComponent.append($('<input type="radio" name="' + cData.Name + '" /><span class="radio-label">' + cData.Label + '</span>'));
                }
                else if (newComponent.hasClass("breadcrumb-navigation")) {
                    newComponent.append($('<div class="app-icon fa fa-question"></div><div class="nav-text">APP NAME &gt; Nav</div>'));
                }
                else if (newComponent.hasClass("data-table")) {
                    newComponent.append($('<thead><tr><th>Column 1</th><th>Column 2</th><th>Column 3</th></tr></thead>'
                        + '<tbody><tr><td>Value1</td><td>Value2</td><td>Value3</td></tr><tr><td>Value4</td><td>Value5</td><td>Value6</td></tr>'
                        + '<tr><td>Value7</td><td>Value8</td><td>Value9</td></tr></tbody>'));
                    CreateCzechDataTable(newComponent);
                    newComponent.css("width", cData.Width);
                    wrapper = newComponent.parents(".dataTables_wrapper");
                    wrapper.css("position", "absolute");
                    wrapper.css("left", cData.PositionX);
                    wrapper.css("top", cData.PositionY);
                    newComponent.css("position", "relative");
                    newComponent.css("left", "0px");
                    newComponent.css("top", "0px");
                }
                else if (newComponent.hasClass("tab-navigation")) {
                    tabLabelArray = cData.Content.split(";");
                    newComponent.append($('<li class="active"><a class="fa fa-home"></a></li>'));
                    for (i = 0; i < tabLabelArray.length; i++) {
                        if (tabLabelArray[i].length > 0)
                            newComponent.append($("<li><a>" + tabLabelArray[i] + "</a></li>"));
                    }
                    newComponent.css("width", "auto");
                }
                else if (newComponent.hasClass("color-picker")) {
                    CreateColorPicker(newComponent);
                    newReplacer = $("#mozaicPageContainer .sp-replacer:last");
                    newReplacer.css("position", "absolute");
                    newReplacer.css("left", newComponent.css("left"));
                    newReplacer.css("top", newComponent.css("top"));
                    newComponent.removeClass("uic color-picker");
                    newReplacer.addClass("uic color-picker");
                    newReplacer.attr("uicClasses", "color-picker");
                    newReplacer.attr("uicName", newComponent.attr("uicName"));
                }
                if (newComponent.hasClass("data-table"))
                    wrapper.draggable({
                        cancel: false,
                        containment: "parent",
                        drag: function (event, ui) {
                            if (GridResolution > 0) {
                                ui.position.left -= (ui.position.left % GridResolution);
                                ui.position.top -= (ui.position.top % GridResolution);
                            }
                        }
                    });
                else
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
            $("#currentPageId").val(data.Id);
            $("#headerPageName").text(data.Name);
            $("#currentPageIsModal").prop("checked", data.IsModal);
            $("#modalWidthInput").val(data.ModalWidth);
            $("#modalHeightInput").val(data.ModalHeight);
            if ($("#currentPageIsModal").is(":checked")) {
                $("#modalSizeVisualization").css("width", parseInt($("#modalWidthInput").val()));
                $("#modalSizeVisualization").css("height", parseInt($("#modalHeightInput").val()));
                $("#modalSizeVisualization").show();
            }
        }
    });
}
