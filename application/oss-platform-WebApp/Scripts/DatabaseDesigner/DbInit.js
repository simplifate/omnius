$(function () {
    $("#btnAddTable").on("click", function () {
        addTableDialog.dialog("open");
    });
    $("#switchToWorkflow").on("click", function () {
        window.location = "/workflow";
    });
    $("#btnSaveScheme").on("click", function () {
        SaveDbScheme();
    });
    $("#btnLoadScheme").on("click", function () {
        LoadDbScheme();
    });
    $("#btnClearScheme").on("click", function () {
        ClearDbScheme();
    });

    LoadDbScheme();
});
