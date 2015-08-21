var CurrentTable, CurrentColumn;

$(function () {
    addTableDialog = $("#add-table-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 150,
        buttons: {
            "Add": function () {
                addTable($(this).parents(".ui-dialog").find("#new-table-name").val());
                addTableDialog.dialog("close");
            },
            Cancel: function () {
                addTableDialog.dialog("close");
            }
        },
        create: function () {
            $("#new-table-name").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addTable($(this).parents(".ui-dialog").find("#new-table-name").val());
                    addTableDialog.dialog("close");
                    return false;
                }
            })
        },
        open: function () {
            $(this).find("#new-table-name").val("");
        }
    });

    editTableDialog = $("#edit-table-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 150,
        buttons: {
            "Save": function () {
                editTableDialog.dialog("close");
                CurrentTable.find(".dbTableName").text($(this).parents(".ui-dialog").find("#table-name").val());
            },
            Cancel: function () {
                editTableDialog.dialog("close");
            }
        },
        create: function () {
            $("#table-name").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    editTableDialog.dialog("close");
                    CurrentTable.find(".dbTableName").text($(this).parents(".ui-dialog").find("#table-name").val());
                    return false;
                }
            })
        },
        open: function () {
            $(this).find("#table-name").val(CurrentTable.find(".dbTableName").text());
        }
    });

    addColumnDialog = $("#add-column-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 190,
        buttons: {
            "Add": function () {
                addColumnDialog.dialog("close");
                addColumn(addColumnDialog.data("currentTable"),
                    $(this).parents(".ui-dialog").find("#column-name").val(),
                    $(this).parents(".ui-dialog").find("#primary-key-checkbox").is(":checked"));
            },
            Cancel: function () {
                addColumnDialog.dialog("close");
            }
        },
        create: function () {
            $(this).find("#column-name").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addColumnDialog.dialog("close");
                    addColumn(addColumnDialog.data("currentTable"),
                        $(this).parents(".ui-dialog").find("#column-name").val(),
                        $(this).parents(".ui-dialog").find("#primary-key-checkbox").is(":checked"));
                    return false;
                }
            })
        },
        open: function () {
            $(this).find("#column-name").val("");
            $(this).find("#primary-key-checkbox").attr("checked", false);
        }
    });

    editColumnDialog = $("#edit-column-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 190,
        buttons: {
            "Save": function () {
                editColumnDialog.dialog("close");
                CurrentColumn.find(".dbColumnName").text($(this).parents(".ui-dialog").find("#column-name").val());
                if (CurrentColumn.hasClass("dbPrimaryKey") && !$(this).find("#primary-key-checkbox").prop("checked"))
                    CurrentColumn.removeClass("dbPrimaryKey");
                else if (!CurrentColumn.hasClass("dbPrimaryKey") && $(this).find("#primary-key-checkbox").prop("checked")) {
                    CurrentColumn.parents(".dbTable").find(".dbColumn").removeClass("dbPrimaryKey");
                    CurrentColumn.addClass("dbPrimaryKey");
                }
             },
            Cancel: function () {
                editColumnDialog.dialog("close");
            }
        },
        create: function () {
            $("#edit-column-dialog #column-name").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    editColumnDialog.dialog("close");
                    CurrentColumn.find(".dbColumnName").text($(this).parents(".ui-dialog").find("#column-name").val());
                    if (CurrentColumn.hasClass("dbPrimaryKey") && !$(this).find("#primary-key-checkbox").prop("checked"))
                        CurrentColumn.removeClass("dbPrimaryKey");
                    else if (!CurrentColumn.hasClass("dbPrimaryKey") && $(this).find("#primary-key-checkbox").prop("checked")) {
                        CurrentColumn.parents(".dbTable").find(".dbColumn").removeClass("dbPrimaryKey");
                        CurrentColumn.addClass("dbPrimaryKey");
                    }
                    return false;
                }
            })
        },
        open: function () {
            $(this).find("#column-name").val(CurrentColumn.find(".dbColumnName").text());
            $(this).find("#primary-key-checkbox").prop("checked", CurrentColumn.hasClass("dbPrimaryKey"));
        }
    });
});