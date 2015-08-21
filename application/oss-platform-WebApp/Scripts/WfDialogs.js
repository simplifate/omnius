$(function () {
    newWorkflowDialog = $("#new-workflow-dialog").dialog({
        autoOpen: false,
        width: 400,
        height: 190,
        buttons: {
            "Create": function () {
                newWorkflowName = $("#new-workflow-name").val();
                if (newWorkflowName) {
                    newWorkflowDialog.dialog("close");
                    CreateWorkflow(newWorkflowName);
                }
                else {
                    alert("Please choose a name for the workflow.");
                }
            },
            Cancel: function () {
                newWorkflowDialog.dialog("close");
            }
        },
        create: function() {
            $("#new-workflow-name").keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    newWorkflowName = $("#new-workflow-name").val();
                    if (newWorkflowName) {
                        newWorkflowDialog.dialog("close");
                        CreateWorkflow(newWorkflowName);
                    }
                    else {
                        alert("Please choose a name for the workflow.");
                    }
                }
            })},
        open: function () {
            $("#new-workflow-name").val("");
        }
    });

    changeWorkflowDialog = $("#change-workflow-dialog").dialog({
        autoOpen: false,
        width: 700,
        height: 540,
        buttons: {
            "Load": function () {
                if (changeWorkflowDialog.data("selectedWorkflowId")) {
                    CurrentWorkflowId = changeWorkflowDialog.data("selectedWorkflowId");
                    CurrentWorkflowName = changeWorkflowDialog.data("selectedWorkflowName");
                    LoadWorkflow("latest");
                    $(".top-bar-container").text(CurrentWorkflowName);
                    changeWorkflowDialog.dialog("close");
                }
                else
                    alert("Please select a workflow");
            },
            Cancel: function () {
                changeWorkflowDialog.dialog("close");
            }
        },
        open: function (event, ui) {
            changeWorkflowDialog.data("selectedWorkflowId", null);
            $.ajax({
                type: "GET",
                url: "/api/workflows",
                dataType: "json",
                error: function () { alert("Error loading the workflow list") },
                success: function (data) {
                    $("#workflow-list-table:first tbody:nth-child(2) tr").remove();
                    tbody = $("#workflow-list-table:first tbody:nth-child(2)");
                    workflowIdArray = [];
                    workflowNameArray = [];

                    // Fill in the workflow list rows
                    for (i = 0; i < data.length; i++) {
                        workflowIdArray.push(data[i].Id);
                        workflowNameArray.push(data[i].Name);
                        tbody.append($('<tr class="workflowItemRow"><td>' + data[i].TimeString
                                + '</td><td>' + data[i].Name + '</td></tr>'));
                    }

                    // Highlight the selected row
                    $(document).on('click', 'tr.workflowItemRow', function (event) {
                        $("#workflow-list-table:first tbody:nth-child(2) tr").removeClass("highlightedCommitRow");
                        $(this).addClass("highlightedCommitRow");
                        var rowIndex = $(this).index();
                        changeWorkflowDialog.data("selectedWorkflowId", workflowIdArray[rowIndex]);
                        changeWorkflowDialog.data("selectedWorkflowName", workflowNameArray[rowIndex]);
                    });
                }
            });
        }
    });

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
                url: "/api/workflows/" + CurrentWorkflowId + "/commits",
                dataType: "json",
                error: function () { alert("Error loading commit history") },
                success: function (data) {
                    $("#commit-table:first tbody:nth-child(2) tr").remove();
                    tbody = $("#commit-table:first tbody:nth-child(2)");
                    commitIdArray = [];

                    // Fill in the history rows
                    for (i = 0; i < data.length; i++) {
                        commitIdArray.push(data[i].Id);
                        if(data[i].CommitMessage != null)
                            tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                + '</td><td>' + data[i].CommitMessage + '</td></tr>'));
                        else
                            tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
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

    $("#btn-new-workflow").button().on("click", function () {
        newWorkflowDialog.dialog("open");
    });

    $("#btn-change-workflow").button().on("click", function () {
        changeWorkflowDialog.dialog("open");
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
    $("#switchToDatabase").on("click", function () {
        window.location = "/database";
    });

    $.ajax({
        type: "GET",
        url: "/api/workflows/last-used",
        dataType: "json",
        success: function (data) {
            CurrentWorkflowId = data.Id;
            CurrentWorkflowName = data.Name;
            $(".top-bar-container").text(CurrentWorkflowName);
            LoadWorkflow("latest");
        }
    });
});
