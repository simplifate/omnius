var ZoomFactor = 1.0;
$(function () {
    $("#btnAddTable").on("click", function () {
        addTableDialog.dialog("open");
    });
    $("#btnAddView").on("click", function () {
        addViewDialog.dialog("open");
    });
    $("#switchToWorkflow").on("click", function () {
        window.location = "/workflow";
    });
    $("#btnSaveScheme").on("click", function () {
        saveDialog.dialog("open");
    });
    $("#btnLoadScheme").on("click", function () {
        LoadDbScheme("latest");
    });
    $("#btnOpenHistory").on("click", function () {
        historyDialog.dialog("open");
    });
    $("#btnClearScheme").on("click", function () {
        ClearDbScheme();
    });
    $("#btnZoomIn").on("click", function () {
        ZoomFactor += 0.1;
        $(".database-container").css("transform", "scale(" + ZoomFactor + ")");
        $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        instance.repaintEverything();
    });
    $("#btnZoomOut").on("click", function () {
        if (ZoomFactor >= 0.2)
            ZoomFactor -= 0.1;
        $(".database-container").css("transform", "scale(" + ZoomFactor + ")");
        $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        instance.repaintEverything();
    });

    LoadDbScheme("latest");
});
