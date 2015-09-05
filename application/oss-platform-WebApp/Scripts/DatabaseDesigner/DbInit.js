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

    LoadDbScheme("latest");
});
