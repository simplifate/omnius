$(function () {
    saveDialog = $("#save-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 190,
        buttons: {
            "Save": function () {
                SaveWorkflow($("#message").val());
                saveDialog.dialog("close");
            },
            Cancel: function () {
                saveDialog.dialog("close");
            }
        },
        create: function() {
            $("#message").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    SaveWorkflow($("#message").val());
                    saveDialog.dialog("close");
                    return false;
                }
            })},
        open: function () {
            $("#message").val("");
        }
    });

    historyDialog = $("#history-dialog").dialog({
        autoOpen: false,
        width: 700,
        height: 540,
        buttons: {
            "Load": function () {
                if (historyDialog.data("selectedCommitId")) {
                    LoadWorkflow(historyDialog.data("selectedCommitId"));
                    historyDialog.dialog("close");
                }
                else
                    alert("Please select a commit");
            },
            Cancel: function () {
                historyDialog.dialog("close");
            }
        },
        open: function (event, ui) {
            historyDialog.data("selectedCommitId", null);
            $.ajax({
                type: "GET",
                url: "/api/commits",
                dataType: "json",
                error: function () { alert("Error loading commit history") },
                success: function (data) {
                    $("#commit-table:first tbody:nth-child(2) tr").remove();
                    tbody = $("#commit-table:first tbody:nth-child(2)");
                    commitIdArray = [];

                    // Fill in the history rows
                    for (i = 0; i < data.CommitHeaders.length; i++) {
                        commitIdArray.push(data.CommitHeaders[i].Id);
                        if(data.CommitHeaders[i].CommitMessage != null)
                            tbody.append($('<tr class="commitRow"><td>' + data.CommitHeaders[i].TimeString
                                + '</td><td>' + data.CommitHeaders[i].CommitMessage + '</td></tr>'));
                        else
                            tbody.append($('<tr class="commitRow"><td>' + data.CommitHeaders[i].TimeString
                                + '</td><td style="color: darkgrey;">(no message)</td></tr>'));
                    }

                    // Highlight the selected row
                    $(document).on('click', 'tr.commitRow', function (event) {
                        $("#commit-table:first tbody:nth-child(2) tr").removeClass("highlightedCommitRow");
                        $(this).addClass("highlightedCommitRow");
                        var rowIndex = $(this).index();
                        historyDialog.data("selectedCommitId", commitIdArray[rowIndex]);
                    });
                    
                }
            });
        }
    });

    $("#btn-save-workflow").button().on("click", function () {
        saveDialog.dialog("open");
    });

    $("#btn-load-workflow").button().on("click", function () {
        LoadWorkflow("latest");
    });

    $("#btn-open-history").button().on("click", function () {
        historyDialog.dialog("open");
    });

    $("#btn-clear-workflow").button().on("click", function () {
        ClearWorkflow();
    });
});
