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
        componentArray.push({
            PositionX: parseInt(currentUic.css("left")),
            PositionY: parseInt(currentUic.css("top")),
            Width: parseInt(currentUic.css("width")),
            Height: parseInt(currentUic.css("height")),
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
