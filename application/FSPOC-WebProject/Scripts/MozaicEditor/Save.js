function SaveMozaicPage() {
    pageSpinner.show();
    SaveRequested = false;
    componentArray = GetMozaicContainerComponentArray($("#mozaicPageContainer"), false);
    postData = {
        Name: $("#headerPageName").text(),
        IsModal: $("#currentPageIsModal").prop("checked"),
        ModalWidth: $("#modalWidthInput").val(),
        ModalHeight: $("#modalHeightInput").val(),
        Components: componentArray
    }
    appId = $("#currentAppId").val();
    pageId = $("#currentPageId").val();
    $.ajax({
        type: "POST",
        url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId,
        data: postData,
        complete: function () {
            pageSpinner.hide();
        },
        success: function () { alert("OK") },
        error: function (request, status, error) {
            alert(request.responseText);
        }
    });
}
function GetMozaicContainerComponentArray(container, nested) {
    if (nested)
        componentArrayLevel2 = [];
    else
        componentArrayLevel1 = [];
    container.find(".uic").each(function (uicIndex, uicElement) {
        currentUic = $(uicElement);
        if (!nested && currentUic.parents(".panel-component").length > 0)
            return true;
        tag = null;
        label = null;
        content = null;
        if (currentUic.hasClass("button-simple") || currentUic.hasClass("button-dropdown")) {
            label = currentUic.text();
        }
        else if (currentUic.hasClass("info-container")) {
            label = currentUic.find(".info-container-header").text();
            content = currentUic.find(".info-container-body").text();
        }
        if (currentUic.hasClass("info-container"))
            type = "info-container";
        else if (currentUic.hasClass("breadcrumb-navigation"))
            type = "breadcrumb";
        else if (currentUic.hasClass("button-simple"))
            type = "button-simple";
        else if (currentUic.hasClass("button-dropdown"))
            type = "button-dropdown";
        else if (currentUic.hasClass("button-browse"))
            type = "button-browse";
        else if (currentUic.hasClass("checkbox-control")) {
            type = "checkbox";
            label = currentUic.find(".checkbox-label").text();
        }
        else if (currentUic.hasClass("radio-control")) {
            type = "radio";
            label = currentUic.find(".radio-label").text();
        }
        else if (currentUic.hasClass("form-heading") || currentUic.hasClass("control-label")) {
            label = currentUic.html();
            content = currentUic.attr("contentTemplate");
            type = "label";
        }
        else if (currentUic.hasClass("input-single-line"))
            type = "input-single-line";
        else if (currentUic.hasClass("input-multiline"))
            type = "input-multiline";
        else if (currentUic.hasClass("dropdown-select"))
            type = "dropdown-select";
        else if (currentUic.hasClass("multiple-select"))
            type = "multiple-select";
        else if (currentUic.hasClass("data-table-with-actions"))
            type = "data-table-with-actions";
        else if (currentUic.hasClass("data-table"))
            type = "data-table-read-only";
        else if (currentUic.hasClass("name-value-list"))
            type = "name-value-list";
        else if (currentUic.hasClass("tab-navigation")) {
            type = "tab-navigation";
            tabString = "";
            currentUic.find("li").each(function (index, element) {
                if (index > 0)
                    tabString += $(element).find("a").text() + ";";
            });
            content = tabString;
        }
        else if (currentUic.hasClass("color-picker"))
            type = "color-picker";
        else if (currentUic.hasClass("countdown-component"))
            type = "countdown";
        else if (currentUic.hasClass("wizard-phases")) {
            type = "wizard-phases";
            var phaseLabels = "";
            currentUic.find(".phase-label").each(function (index, element) {
                phaseLabels += $(element).text() + ";";
            });
            phaseLabels = phaseLabels.slice(0, -1);
            content = phaseLabels;
        }
        else if (currentUic.hasClass("named-panel")) {
            type = "panel";
            label = currentUic.find(".named-panel-header").text();
        }
        else if (currentUic.hasClass("panel-component"))
            type = "panel";
        else
            type = "control";
        if (currentUic.hasClass("data-table")) {
            wrapper = currentUic.parents("");
            positionX = wrapper.css("left");
            positionY = wrapper.css("top");
        }
        else {
            positionX = currentUic.css("left");
            positionY = currentUic.css("top");
        }
        if (currentUic.hasClass("color-picker"))
            tag = "input";
        else
            tag = currentUic.prop("tagName").toLowerCase();
        name = currentUic.attr("uicName");
        if (!name || name == "")
            name = type + uicIndex;
        componentData = {
            Name: name,
            Type: type,
            PositionX: positionX,
            PositionY: positionY,
            Width: currentUic.css("width"),
            Height: currentUic.css("height"),
            Tag: tag,
            Attributes: currentUic.attr("uicAttributes"),
            Classes: currentUic.attr("uicClasses"),
            Styles: currentUic.attr("uicStyles"),
            Properties: currentUic.attr("uicProperties") ? currentUic.attr("uicProperties") : "",
            Content: content,
            Label: label,
            Placeholder: currentUic.attr("placeholder"),
            TabIndex: currentUic.attr("tabindex"),
            ChildComponents: currentUic.hasClass("panel-component") ? GetMozaicContainerComponentArray(currentUic, true) : []
        };
        if (nested)
            componentArrayLevel2.push(componentData);
        else
            componentArrayLevel1.push(componentData);
    });
    if (nested)
        return componentArrayLevel2;
    else
        return componentArrayLevel1;
}