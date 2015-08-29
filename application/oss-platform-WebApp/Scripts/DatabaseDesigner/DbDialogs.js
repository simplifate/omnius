var CurrentTable, CurrentColumn, CurrentConnection;

$(function () {
    addTableDialog = $("#add-table-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        height: 150,
        buttons: {
            "Add": function () {
                addTableDialog_SubmitData();
            },
            Cancel: function () {
                addTableDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addTableDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            addTableDialog.find("#new-table-name").val("");
        }
    });
    function addTableDialog_SubmitData() {
        AddTable(addTableDialog.find("#new-table-name").val());
        addTableDialog.dialog("close");
    }

    editTableDialog = $("#edit-table-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        height: 150,
        buttons: {
            "Save": function () {
                editTableDialog_SubmitData();
            },
            Cancel: function () {
                editTableDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    editTableDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            editTableDialog.find("#table-name").val(CurrentTable.find(".dbTableName").text());
        }
    });
    function editTableDialog_SubmitData() {
        CurrentTable.find(".dbTableName").text(editTableDialog.find("#table-name").val());
        editTableDialog.dialog("close");
    }

    addColumnDialog = $("#add-column-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        height: 230,
        buttons: {
            "Add": function () {
                addColumnDialog_SubmitData();
            },
            Cancel: function () {
                addColumnDialog.dialog("close");
            }
        },
        create: function () {
            for (i = 0; i < SqlServerDataTypes.length; i++) {
                $("#add-column-dialog #column-type-dropdown").append(
                    $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
            }
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    addColumnDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            addColumnDialog.find("#column-name").val("");
            addColumnDialog.find("#primary-key-checkbox").attr("checked", false);
            addColumnDialog.find("#allow-null-checkbox").prop("checked", false);
            addColumnDialog.find("#column-type-dropdown").val("varchar");
        }
    });
    function addColumnDialog_SubmitData() {
        AddColumn(addColumnDialog.data("currentTable"),
            addColumnDialog.find("#column-name").val(),
            addColumnDialog.find("#column-type-dropdown").val(),
            addColumnDialog.find("#primary-key-checkbox").prop("checked"),
            addColumnDialog.find("#allow-null-checkbox").prop("checked"));
        addColumnDialog.dialog("close");
    }

    editColumnDialog = $("#edit-column-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        height: 230,
        buttons: {
            "Save": function () {
                editColumnDialog_SubmitData();
             },
            Cancel: function () {
                editColumnDialog.dialog("close");
            }
        },
        create: function () {
            for (i = 0; i < SqlServerDataTypes.length; i++) {
                $("#edit-column-dialog #column-type-dropdown").append(
                    $('<option value="' + SqlServerDataTypes[i][0] + '">' + SqlServerDataTypes[i][1] + '</option>'));
            }
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    editColumnDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            editColumnDialog.find("#column-name").val(CurrentColumn.find(".dbColumnName").text());
            editColumnDialog.find("#primary-key-checkbox").prop("checked", CurrentColumn.hasClass("dbPrimaryKey"));
            editColumnDialog.find("#allow-null-checkbox").prop("checked", CurrentColumn.attr("dbAllowNull") == "true");
            editColumnDialog.find("#column-type-dropdown").val(CurrentColumn.attr("dbColumnType"));
        }
    });
    function editColumnDialog_SubmitData() {
        CurrentColumn.find(".dbColumnName").text(editColumnDialog.find("#column-name").val());
        CurrentColumn.attr("dbColumnType", editColumnDialog.find("#column-type-dropdown").val());
        CurrentColumn.attr("dbAllowNull", editColumnDialog.find("#allow-null-checkbox").prop("checked"));
        if (CurrentColumn.hasClass("dbPrimaryKey") && !editColumnDialog.find("#primary-key-checkbox").prop("checked"))
            CurrentColumn.removeClass("dbPrimaryKey");
        else if (!CurrentColumn.hasClass("dbPrimaryKey") && editColumnDialog.find("#primary-key-checkbox").prop("checked")) {
            //CurrentColumn.parents(".dbTable").find(".dbColumn").removeClass("dbPrimaryKey"); // Uncomment this line to allow only one primary key per table
            CurrentColumn.addClass("dbPrimaryKey");
        }
        editColumnDialog.dialog("close");
    }

    editRelationDialog = $("#edit-relation-dialog").dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        height: 250,
        buttons: {
            "Save": function () {
                editRelationDialog_SubmitData()
            },
            Cancel: function () {
                editRelationDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    editRelationDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            if ($(CurrentConnection).data("relationType"))
                editRelationDialog.find("input:radio[value=" + $(CurrentConnection).data("relationType") + "]").prop("checked", "checked");
            else
                editRelationDialog.find("input:radio[value=1]").prop("checked", "checked");
        }
    });
    function editRelationDialog_SubmitData() {
        $(CurrentConnection).data("relationType", editRelationDialog.find("input[type='radio']:checked").val());
        switch (editRelationDialog.find("input[type='radio']:checked").val()) {
            case "1":
                EditRelation(CurrentConnection, "1", "1");
                break;
            case "2":
                EditRelation(CurrentConnection, "1", "N");
                break;
            case "3":
                EditRelation(CurrentConnection, "N", "1");
                break;
            case "4":
                EditRelation(CurrentConnection, "M", "N");
                break;
            case "Delete":
                instance.detach(CurrentConnection);
                break;
        }
        editRelationDialog.dialog("close");
    }
});