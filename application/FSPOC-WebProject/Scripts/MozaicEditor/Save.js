function SaveMozaicPage() {
    componentArray = [];
    $("#mozaicPageContainer .uic").each(function (uicIndex, uicElement) {
        currentUic = $(uicElement);
        label = null;
        content = null;
        if (currentUic.hasClass("button-simple") || currentUic.hasClass("button-dropdown")) {
            label = currentUic.text();
        }
        else if (currentUic.hasClass("info-container")) {
            label = currentUic.find(".info-container-header").text();
            content = currentUic.find(".info-container-body").text();
        }
        else if (currentUic.hasClass("form-heading") || currentUic.hasClass("control-label")) {
            label = currentUic.text();
            type = "label";
        }
        if(currentUic.hasClass("info-container"))
            type = "info-container";
        else if (currentUic.hasClass("breadcrumb-navigation"))
            type = "breadcrumb";
        else if (currentUic.hasClass("button-simple"))
            type = "button-simple";
        else if (currentUic.hasClass("button-dropdown"))
            type = "button-dropdown";
        else if (currentUic.hasClass("checkbox-control")) {
            type = "checkbox";
            label = currentUic.find(".checkbox-label").text();
        }
        else
            type = "control";
        componentArray.push({
            Name: currentUic.attr("uicName"),
            Type: type,
            PositionX: currentUic.css("left"),
            PositionY: currentUic.css("top"),
            Width: currentUic.css("width"),
            Height: currentUic.css("height"),
            Tag: currentUic.prop("tagName").toLowerCase(),
            Attributes: "",
            Classes: currentUic.attr("uicClasses"),
            Styles: currentUic.attr("uicStyles"),
            Content: content,
            Label: label,
            Placeholder: currentUic.attr("placeholder")
        });

    });
    postData = {
        Name: $("#headerPageName").text(),
        Components: componentArray
    }
    appId = $("#currentAppId").val();
    pageId = $("#currentPageId").val();
    $.ajax({
        type: "POST",
        url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId,
        data: postData,
        success: function () { alert("OK") },
        error: function () { alert("ERROR") }
    });
}
