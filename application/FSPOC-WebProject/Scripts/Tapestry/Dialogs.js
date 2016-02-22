var currentRule, CurrentItem;
$(function () {
    if (CurrentModuleIs("tapestryModule")) {
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
                renameBlockDialog.find("#block-name").val($("#blockHeaderBlockName").text());
            }
        });
        function renameBlockDialog_SubmitData() {
            renameBlockDialog.dialog("close");
            $("#blockHeaderBlockName").text(renameBlockDialog.find("#block-name").val());
            ChangedSinceLastSave = true;
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
                ChangedSinceLastSave = true;
            }
            else
                alert("No table selected");
        }
        chooseEmailTemplateDialog = $("#choose-email-template-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Choose": function () {
                    chooseEmailTemplateDialog_SubmitData();
                },
                Cancel: function () {
                    chooseEmailTemplateDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                $(this).find("#choice-template:first tbody:nth-child(2) tr").remove();
                tbody = $(this).find("#choice-template tbody:nth-child(2)");
                for (i = 1; i <= 5; i++)
                    tbody.append($('<tr class="emailTemplateRow formRow" templateId="' + i + '"><td>' + 'Email template ' + i + '</td></tr>'));
                if (CurrentItem.data("emailTemplate"))
                    tbody.find('tr[templateId="' + CurrentItem.data("emailTemplate") + '"]').addClass("highlightedRow");
                $(document).on("click", "tr.emailTemplateRow", function (event) {
                    chooseEmailTemplateDialog.find("#choice-template tbody:nth-child(2) tr").removeClass("highlightedRow");
                    $(this).addClass("highlightedRow");
                });
            }
        });
        function chooseEmailTemplateDialog_SubmitData() {
            selectedRow = chooseEmailTemplateDialog.find("#choice-template:first tbody:nth-child(2) tr.highlightedRow");
            if (selectedRow.length) {
                CurrentItem.data("emailTemplate", selectedRow.attr("templateId"));
                chooseEmailTemplateDialog.dialog("close");
                ChangedSinceLastSave = true;
            }
            else
                alert("No template selected");
        }
        choosePortDialog = $("#choose-port-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Choose": function () {
                    choosePortDialog_SubmitData();
                },
                Cancel: function () {
                    choosePortDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                $("#choose-port-dialog").find("#choice-port tbody:nth-child(2) tr").remove();
                $.ajax({
                    type: "GET",
                    url: "/api/tapestry/apps/" + appId + "/blocks",
                    dataType: "json",
                    error: function () { alert("Error loading block list") },
                    success: function (data) {
                        tbody = $("#choose-port-dialog").find("#choice-port tbody:nth-child(2)");
                        for (i = 0; i < data.ListItems.length; i++) {
                            tbody.append($('<tr class="portRow formRow" portId="' + data.ListItems[i].Id + '"><td>'
                                + data.ListItems[i].Name + '</td></tr>'));
                        }
                        if (CurrentItem.data("portId"))
                            tbody.find('tr[portId="' + CurrentItem.data("portId") + '"]').addClass("highlightedRow");
                        $(document).on("click", "tr.portRow", function (event) {
                            choosePortDialog.find("#choice-port tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                        });
                    }
                });
            }
        });
        function choosePortDialog_SubmitData() {
            selectedRow = choosePortDialog.find("#choice-port:first tbody:nth-child(2) tr.highlightedRow");
            if (selectedRow.length) {
                CurrentItem.data("portId", selectedRow.attr("portId"));
                CurrentItem.html('<i class="fa fa-sign-out" style="margin-left: 1px; margin-right: 5px;"></i>'
                    + selectedRow.find("td").text());
                CurrentItem.parents(".rule").data("jsPlumbInstance").recalculateOffsets();
                CurrentItem.parents(".rule").data("jsPlumbInstance").repaintEverything();
                choosePortDialog.dialog("close");
                ChangedSinceLastSave = true;
            }
            else
                alert("No port selected");
        }
        renameRuleDialog = $("#rename-rule-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    renameRuleDialog_SubmitData();
                },
                Cancel: function () {
                    renameRuleDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        renameRuleDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                renameRuleDialog.find("#rule-name").val(currentRule.find(".workflowRuleHeader .verticalLabel").text());
            }
        });
        function renameRuleDialog_SubmitData() {
            renameRuleDialog.dialog("close");
            currentRule.find(".workflowRuleHeader .verticalLabel").text(renameRuleDialog.find("#rule-name").val());
            ChangedSinceLastSave = true;
        }
        historyDialog = $("#history-dialog").dialog({
            autoOpen: false,
            width: 700,
            height: 540,
            buttons: {
                "Load": function () {
                    historyDialog_SubmitData();
                },
                Cancel: function () {
                    historyDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                historyDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                blockId = $("#currentBlockId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId + "/commits",
                    dataType: "json",
                    error: function () { alert("Error loading commit history") },
                    success: function (data) {
                        historyDialog.find("#commit-table:first tbody:nth-child(2) tr").remove();
                        tbody = historyDialog.find("#commit-table tbody:nth-child(2)");
                        commitIdArray = [];

                        // Fill in the history rows
                        for (i = 0; i < data.length; i++) {
                            commitIdArray.push(data[i].Id);
                            if (data[i].CommitMessage != null)
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td>' + data[i].CommitMessage + '</td></tr>'));
                            else
                                tbody.append($('<tr class="commitRow"><td>' + data[i].TimeString
                                    + '</td><td style="color: darkgrey;">(no message)</td></tr>'));
                        }

                        // Highlight the selected row
                        $(document).on('click', 'tr.commitRow', function (event) {
                            historyDialog.find("#commit-table tbody:nth-child(2) tr").removeClass("highlightedRow");
                            $(this).addClass("highlightedRow");
                            var rowIndex = $(this).index();
                            historyDialog.data("selectedCommitId", commitIdArray[rowIndex]);
                        });
                    }
                });
            }
        });
        function historyDialog_SubmitData() {
            if (historyDialog.data("selectedCommitId")) {
                historyDialog.dialog("close");
                if (ChangedSinceLastSave)
                    confirmed = confirm("Máte neuložené změny, opravdu si přejete tyto změny zahodit?");
                else
                    confirmed = true;
                if (confirmed) {
                    LoadBlock(historyDialog.data("selectedCommitId"));
                }
            }
            else
                alert("Please select a commit");
        }
        saveDialog = $("#save-dialog").dialog({
            autoOpen: false,
            width: 400,
            height: 190,
            buttons: {
                "Save": function () {
                    saveDialog_SubmitData();
                },
                Cancel: function () {
                    saveDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        saveDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                saveDialog.find("#message").val("");
            }
        });
        function saveDialog_SubmitData() {
            saveDialog.dialog("close");
            SaveBlock(saveDialog.find("#message").val());
        }
        chooseScreensDialog = $("#choose-screens-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 550,
            buttons: {
                "Select": function () {
                    chooseScreensDialog_SubmitData();
                },
                Cancel: function () {
                    chooseScreensDialog.dialog("close");
                }
            },
            create: function () {
                $(document).on("click", "tr.actionRow", function (event) {
                    $(this).toggleClass("highlightedRow");
                });
            },
            open: function (event, ui) {
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages",
                    dataType: "json",
                    error: function () { alert("Error loading page list") },
                    success: function (data) {
                        chooseScreensDialog.find("#screen-table:first tbody:nth-child(2) tr").remove();
                        tbody = chooseScreensDialog.find("#screen-table tbody:nth-child(2)");
                        for (i = 0; i < data.length; i++) {
                            tbody.append($('<tr class="screenRow" pageId="' + data[i].Id + '"><td>' + data[i].Name + '</td></tr>'));
                        }
                        $("#screen-table .screenRow").on("click", function () {
                            $(this).toggleClass("highlightedRow");
                        });
                    }
                });
            }
        });
        function chooseScreensDialog_SubmitData() {
            somethingWasAdded = false;
            pageCount = 0;
            appId = $("#currentAppId").val();
            AssociatedPageIds = [];
            chooseScreensDialog.find("#screen-table:first tbody:nth-child(2) tr").each(function (index, element) {
                if ($(element).hasClass("highlightedRow")) {
                    pageCount++;
                    pageId = $(element).attr("pageId");
                    AssociatedPageIds.push(pageId);
                    url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
                    $.ajax({
                        type: "GET",
                        url: url,
                        dataType: "json",
                        success: function (data) {
                            for (i = 0; i < data.Components.length; i++) {
                                cData = data.Components[i];
                                lastLibId++;
                                newLibItem = $('<div libId="' + lastLibId + '" pageId="' + data.Id + '" componentId="' + cData.Id + '" libType="ui" class="libraryItem">'
                                    + cData.Name + '</div>');
                                $("#libraryCategory-UI").append(newLibItem);
                            }
                        }
                    });
                }
            });
            $("#blockHeaderScreenCount").text(pageCount);
            chooseScreensDialog.dialog("close");
        }
        tableAttributePropertiesDialog = $("#table-attribute-properties-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 500,
            buttons: {
                "Change": function () {
                    tableAttributePropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    tableAttributePropertiesDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                appId = $("#currentAppId").val();
                url = "/api/database/apps/" + appId + "/commits/latest",
                tableId = CurrentItem.attr("tableId");
                $.ajax({
                    type: "GET",
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        columnFilter = CurrentItem.data("columnFilter");
                        if (columnFilter == undefined)
                            columnFilter = [];
                        formTable = tableAttributePropertiesDialog.find(".columnFilterTable tbody");
                        formTable.find("tr").remove();
                        targetTable = data.Tables.filter(function (value, index, ar) {
                            return value.Id == tableId;
                        })[0];
                        data.Tables[tableId];
                        for (i = 0; i < targetTable.Columns.length; i++) {
                            newRow = $('<tr columnId="' + targetTable.Columns[i].Id + '"><td>' + targetTable.Columns[i].Name + '</td>'
                                + '<td><input type="checkbox" class="showColumnCheckbox"></input>Show</td></tr>');
                            formTable.append(newRow);
                            newRow.find(".showColumnCheckbox").prop("checked", columnFilter.indexOf(targetTable.Columns[i].Id) != -1);
                        }
                    }
                });
            }
        });
        function tableAttributePropertiesDialog_SubmitData() {
            columnFilter = [];
            formTable = tableAttributePropertiesDialog.find(".columnFilterTable .showColumnCheckbox").each(function (index, checkboxElement) {
                columnId = $(checkboxElement).parents("tr").attr("columnId");
                if($(checkboxElement).is(":checked"))
                    columnFilter.push(parseInt(columnId));
            });
            CurrentItem.data("columnFilter", columnFilter);
            tableAttributePropertiesDialog.dialog("close");
        }
    }
});
