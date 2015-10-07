$(function () {
    renameBlockDialog = $("#rename-block-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 190,
        buttons: {
            "Save": function () {
                renameBlockDialog_SubmitData();
            },
            Cancel: function () {
                renameBlockDialog.dialog("close");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    renameBlockDialog_SubmitData();
                    return false;
                }
            })
        },
        open: function () {
            renameBlockDialog.find("#block-name").val($("#headerBlockName").text());
        }
    });
    function renameBlockDialog_SubmitData() {
        renameBlockDialog.dialog("close");
        $("#headerBlockName").text(renameBlockDialog.find("#block-name").val());
    }
    addActionsDialog = $("#add-actions-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 550,
        buttons: {
            "Add": function () {
                addActionsDialog_SubmitData();
            },
            Cancel: function () {
                addActionsDialog.dialog("close");
            }
        },
        create: function() {
            $(document).on("click", "tr.actionRow", function (event) {
                $(this).toggleClass("highlightedRow");
            });
        },
        open: function (event, ui) {
            $(this).find("#action-table:first tbody:nth-child(2) tr").remove();
            tbody = $(this).find("#action-table tbody:nth-child(2)");
            for (i = 1; i <= 10; i++)
                tbody.append($('<tr class="actionRow formRow"><td>' + 'Action' + i + '</td></tr>'));
        }
    });
    function addActionsDialog_SubmitData() {
        somethingWasAdded = false;
        addActionsDialog.find("#action-table:first tbody:nth-child(2) tr").each(function (index, element) {
            if ($(element).hasClass("highlightedRow")) {
                newActionLabel = $(element).find("td").text();
                newAction = $('<div class="menuItem action">' + newActionLabel + '</div>');
                $("#actionsPanel").append(newAction);
                newAction.draggable({
                    helper: "clone",
                    tolerance: "fit",
                    revert: true
                });
                somethingWasAdded = true;
            }
        });
        if (somethingWasAdded)
            addActionsDialog.dialog("close");
        else
            alert("No actions selected");
    }
    chooseTableDialog = $("#choose-table-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 500,
        buttons: {
            "Change": function () {
                chooseTableDialog_SubmitData();
            },
            Cancel: function () {
                chooseTableDialog.dialog("close");
            }
        },
        open: function (event, ui) {
            $(this).find("#choice-table:first tbody:nth-child(2) tr").remove();
            tbody = $(this).find("#choice-table tbody:nth-child(2)");
            for (i = 1; i <= 5; i++)
                tbody.append($('<tr class="tableNameRow formRow"><td>' + 'Table' + i + '</td></tr>'));
            $(document).on("click", "tr.tableNameRow", function (event) {
                chooseTableDialog.find("#choice-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                $(this).addClass("highlightedRow");
            });
        }
    });
    function chooseTableDialog_SubmitData() {
        somethingWasAdded = false;
        selectedRow = chooseTableDialog.find("#choice-table:first tbody:nth-child(2) tr.highlightedRow");
        if (selectedRow.length) {
            chooseTableDialog.dialog("close");
            $("#headerTableName").text(selectedRow.find("td").text());
        }
        else
            alert("No table selected");
    }
});
