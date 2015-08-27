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

    // Initialization of preloaded sample tables, will be removed later
    $(".deleteColumnIcon").on("click", function () {
        $(this).parents(".dbColumn").remove();
        instance.removeAllEndpoints($(this).parents(".dbColumn"), true);
    });
    $(".addColumnIcon").on("click", function () {
        addColumnDialog.data("currentTable", $(this).parents(".dbTable"));
        addColumnDialog.dialog("open");
    });
    $(".editColumnIcon").on("click", function () {
        CurrentColumn = $(this).parents(".dbColumn");
        editColumnDialog.dialog("open");
    });
    $(".editTableIcon").on("click", function () {
        CurrentTable = $(this).parents(".dbTable");
        editTableDialog.dialog("open");
    });
    $(".deleteTableIcon").on("click", function () {
        $(this).parents(".dbTable").remove();
        instance.removeAllEndpoints($(this).parents(".dbTable"), true);
    });

    LoadDbScheme();
});
