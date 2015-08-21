$(function () {
    $(".dbTable").draggable({
        handle: ".dbTableHeader"
    });
    $("#btnAddTable").on("click", function () {
        addTableDialog.dialog("open");
    });
    $("#switchToWorkflow").on("click", function () {
        window.location = "/workflow";
    });
    $(".deleteColumnIcon").on("click", function () {
        $(this).parents(".dbColumn").remove();
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
    });
});

function addTable(tableName) {
    newTable = $('<div class="dbTable"><div class="dbTableHeader"><div class="deleteTableIcon fa fa-remove"></div><span class="dbTableName">'
        + tableName + '</span><div class="editTableIcon fa fa-pencil"></div><div class="addColumnIcon fa fa-plus"></div></div>'
        + '<div class="dbTableBody"><div class="dbColumn dbPrimaryKey"><div class="deleteColumnIcon fa fa-remove"></div>'
        + '<span class="dbColumnName">id</span><div class="editColumnIcon fa fa-pencil"></div></div></div></div>');
    $("body").append(newTable);
    newTable.draggable({
        handle: ".dbTableHeader"
    });
    $(".editTableIcon").on("click", function () {
        CurrentTable = $(this).parents(".dbTable");
        editTableDialog.dialog("open");
    });
    newTable.find(".deleteTableIcon").on("click", function () {
        $(this).parents(".dbTable").remove();
    });
    newTable.find(".addColumnIcon").on("click", function () {
        addColumnDialog.data("currentTable", $(this).parents(".dbTable"));
        addColumnDialog.dialog("open");
    })
    newTable.find(".deleteColumnIcon").on("click", function () {
        $(this).parents(".dbColumn").remove();
    });
    newTable.find(".editColumnIcon").on("click", function () {
        CurrentColumn = $(this).parents(".dbColumn");
        editColumnDialog.dialog("open");
    });
    //AddToJsPlumb(newTable.find(".dbColumn"));
}

function addColumn(table, columnName, isPrimaryKey) {
    newColumn = $('<div class="dbColumn"><div class="deleteColumnIcon fa fa-remove"></div><span class="dbColumnName">'
    + columnName + '</span><div class="editColumnIcon fa fa-pencil"></div></div>');

    newColumn.children(".deleteColumnIcon").on("click", function () {
        $(this).parents(".dbColumn").remove();
    });
    newColumn.children(".editColumnIcon").on("click", function () {
        CurrentColumn = $(this).parents(".dbColumn");
        editColumnDialog.dialog("open");
    });
    table.children(".dbTableBody").append(newColumn);
    if (isPrimaryKey) {
        table.find(".dbColumn").removeClass("dbPrimaryKey");
        newColumn.addClass("dbPrimaryKey");
    }
    //AddToJsPlumb(newColumn);
}