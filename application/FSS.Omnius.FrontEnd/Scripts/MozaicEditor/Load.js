function LoadMozaicPage(pageId) {
    pageSpinner.show();
    appId = $("#currentAppId").val();
    if (pageId == "current")
        pageId = $("#currentPageId").val();
    url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        complete: function () {
            pageSpinner.hide()
        },
        error: function (request, status, error) {
            alert(request.responseText);
        },
        success: function (data) {
            if ($('body').hasClass('mozaicBootstrapEditorModule')) {
                MBE.io.convert(data);
            }
            else
            {
                $("#mozaicPageContainer .uic").remove();
                $("#mozaicPageContainer .dataTables_wrapper").remove();
                $("#mozaicPageContainer .color-picker").remove();

                for (i = 0; i < data.Components.length; i++) {
                    LoadMozaicEditorComponents($("#mozaicPageContainer"), data.Components[i]);
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

                var panels = $(".mozaicEditorAbsolute, .mozaicEditorBootstrap").removeClass("mozaicEditorAbsolute mozaicEditorBootstrap");
                switch (data.version) {
                    case "0":
                    default:
                        panels.addClass("mozaicEditorAbsolute");
                        break;
                    case "1":
                        panels.addClass("mozaicEditorBootstrap");
                        break;
                }
            }
        }
    });
}
function LoadMozaicEditorComponents(targetContainer, cData) {
    newComponent = $('<' + cData.Tag + ' id="' + cData.Id + '" uicName="' + cData.Name + /*'" uicAttributes="' + (cData.Attributes || "") + */'" class="uic ' + cData.Classes
                    + '" uicClasses="' + cData.Classes + '" uicStyles="' + cData.Styles + '" style="left: ' + cData.PositionX + '; top: ' + cData.PositionY + '; width: '
                    + cData.Width + '; height: ' + cData.Height + '; ' + cData.Styles + '"></' + cData.Tag + '>');
    newComponent.data("uicAttributes", cData.Attributes);

    targetContainer.append(newComponent);
    if (cData.Placeholder)
        newComponent.attr("placeholder", cData.Placeholder);
    if (cData.TabIndex)
        newComponent.attr("tabindex", cData.TabIndex);
   
    if (cData.Properties)
        newComponent.attr("uicProperties", cData.Properties);
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
    else if (newComponent.hasClass("named-panel")) {
        newComponent.append($('<div class="named-panel-header"></div>'));
        newComponent.find(".named-panel-header").text(cData.Label);
    }
    else if (newComponent.hasClass("multiple-select")) {
        newComponent.append($('<option value="1">Multiple</option><option value="2">Choice</option><option value="3">Select</option>'));
        newComponent.attr("multiple", "");
    }
    else if (newComponent.hasClass("button-browse")) {
        newComponent.attr("type", "file");
    }
    else if (newComponent.hasClass("form-heading") || newComponent.hasClass("control-label")) {
        newComponent.html(cData.Label);
        newComponent.attr("contentTemplate", cData.Content);
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
        CreateCzechDataTable(newComponent, newComponent.hasClass("data-table-simple-mode"));
        newComponent.css("width", cData.Width);
        wrapper = newComponent.parents(".dataTables_wrapper");
        wrapper.css("position", "absolute");
        wrapper.css("left", cData.PositionX);
        wrapper.css("top", cData.PositionY);
        newComponent.css("position", "relative");
        newComponent.css("left", "0px");
        newComponent.css("top", "0px");
    }
    else if (newComponent.hasClass("name-value-list")) {
        newComponent.append($('<tr><td class="name-cell">Platform</td><td class="value-cell">Omnius</td></tr><tr><td class="name-cell">Country</td>'
            + '<td class="value-cell">Czech Republic</td></tr><tr><td class="name-cell">Year</td><td class="value-cell">2016</td></tr>'));
    }
    else if (newComponent.hasClass("tab-navigation")) {
        tabLabelArray = cData.Content.split(";");
        newComponent.append($('<li class="active"><a class="fa fa-home"></a></li>'));
        for (k = 0; k < tabLabelArray.length; k++) {
            if (tabLabelArray[k].length > 0)
                newComponent.append($("<li><a>" + tabLabelArray[k] + "</a></li>"));
        }
        newComponent.css("width", "auto");
    }
    else if (newComponent.hasClass("color-picker")) {
        CreateColorPicker(newComponent);
        newReplacer = targetContainer.find(".sp-replacer:last");
        newReplacer.css("position", "absolute");
        newReplacer.css("left", newComponent.css("left"));
        newReplacer.css("top", newComponent.css("top"));
        newComponent.removeClass("uic");
        newReplacer.addClass("uic color-picker");
        newReplacer.attr("uicClasses", "color-picker");
        newReplacer.attr("uicName", newComponent.attr("uicName"));
    }
    else if (newComponent.hasClass("countdown-component")) {
        newComponent.html('<span class="countdown-row countdown-show3"><span class="countdown-section"><span class="countdown-amount">0</span>'
            + '<span class="countdown-period">Hodin</span></span><span class="countdown-section"><span class="countdown-amount">29</span>'
            + '<span class="countdown-period">Minut</span></span><span class="countdown-section"><span class="countdown-amount">59</span>'
            + '<span class="countdown-period">Sekund</span></span></span>');
    }
    else if (newComponent.hasClass("wizard-phases")) {
        newComponent.html(WizardPhasesContentTemplate);
        var phaseLabelArray = cData.Content.split(";");
        newComponent.find(".phase1 .phase-label").text(phaseLabelArray[0] ? phaseLabelArray[0] : "Fáze 1");
        newComponent.find(".phase2 .phase-label").text(phaseLabelArray[1] ? phaseLabelArray[1] : "Fáze 2");
        newComponent.find(".phase3 .phase-label").text(phaseLabelArray[2] ? phaseLabelArray[2] : "Fáze 3");
    }

    if (newComponent.hasClass("panel-component")) { //mšebela: odstraněno else před if (kvůli named-component)
        CreateDroppableMozaicContainer(newComponent, false);
    }
    if (newComponent.hasClass("data-table"))
        draggableElement = wrapper;
    else if (newComponent.hasClass("color-picker"))
        draggableElement = newReplacer;
    else
        draggableElement = newComponent;
    draggableElement.draggable({
        cancel: false,
        containment: "parent",
        drag: function (event, ui) {
            if (GridResolution > 0) {
                ui.position.left -= (ui.position.left % GridResolution);
                ui.position.top -= (ui.position.top % GridResolution);
            }
        }
    });
    if (cData.ChildComponents) {
        currentPanel = newComponent;
        for (j = 0; j < cData.ChildComponents.length; j++) {
            LoadMozaicEditorComponents(currentPanel, cData.ChildComponents[j]);
        }
    }
}
