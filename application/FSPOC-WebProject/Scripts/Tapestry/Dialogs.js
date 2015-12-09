var currentRule;
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
            create: function () {
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
            }
            else
                alert("No template selected");
        }

        editConditionDialog = $("#edit-condition-dialog").dialog({
            autoOpen: false,
            width: 750,
            height: 500,
            buttons: {
                "Save": function () {
                    editConditionDialog_SubmitData();
                },
                Cancel: function () {
                    editConditionDialog.dialog("close");
                }
            },
            open: function (event, ui) {
                $(this).find(".logicTable tr").remove();
                firstRow = $('<tr><td width="71px">IF</td><td>'
                    + '<select class="selectField"></select></td>'
                    + '<td><select class="selectOperator"></select></td>'
                    + '<td><input type="text" class="constantValue"/></td>'
                    + '<td width="91px"></td></tr>');
                $(this).find(".logicTable").append(firstRow);
                FillConditionsForLogicTableRow(firstRow);
                conditionArrayPair = ConditionData.filter(function (val) {
                    return val[0] == CurrentItem.attr("id");
                })[0];
                if (conditionArrayPair) {
                    conditionArray = conditionArrayPair[1];
                    if (conditionArray.length > 0) {
                        firstRow.find(".selectField").val(conditionArray[0].Field);
                        firstRow.find(".selectOperator").val(conditionArray[0].Operator);
                        firstRow.find(".constantValue").val(conditionArray[0].Constant);

                        for (j = 1; j < conditionArray.length; j++) {
                            newRow = $('<tr><td><select class="selectRelation"></select></td>'
                                + '<td><select class="selectField"></select></td>'
                                + '<td><select class="selectOperator"></select></td>'
                                + '<td><input type="text" class="constantValue"/></td>'
                                + '<td><button type="button" class="btnRemoveCondition">Remove</button></td></tr>');
                            FillConditionsForLogicTableRow(newRow);
                            newRow.find(".selectRelation").val(conditionArray[j].Relation);
                            newRow.find(".selectField").val(conditionArray[j].Field);
                            newRow.find(".selectOperator").val(conditionArray[j].Operator);
                            newRow.find(".constantValue").val(conditionArray[j].Constant);

                            newRow.find(".btnRemoveCondition").on("click", function () {
                                $(this).parents("tr").remove();
                            });
                            $(this).find(".logicTable").append(newRow);
                        }
                    }
                }
            }
        });
        function editConditionDialog_SubmitData() {
            conditionArray = [];
            editConditionDialog.find(".logicTable tr").each(function (index, value) {
                logicTableRow = $(value);
                conditionArray.push({
                    Relation: logicTableRow.find(".selectRelation").val(),
                    Field: logicTableRow.find(".selectField").val(),
                    Operator: logicTableRow.find(".selectOperator").val(),
                    Constant: logicTableRow.find(".constantValue").val()
                });
            });
            currentItemDataRecord = ConditionData.filter(function (val) {
                return val[0] == CurrentItem.attr("id");
            })[0];
            dataRecordIndex = ConditionData.indexOf(currentItemDataRecord);
            if (dataRecordIndex != -1)
                ConditionData[dataRecordIndex] = [CurrentItem.attr("id"), conditionArray];
            else
                ConditionData.push([CurrentItem.attr("id"), conditionArray]);

            editConditionDialog.dialog("close");
        }
        $("#btnAddCondition").on("click", function () {
            newRow = $('<tr><td><select class="selectRelation"></select></td>'
                + '<td><select class="selectField"></select></td>'
                + '<td><select class="selectOperator"></select></td>'
                + '<td><input type="text" class="constantValue"/></td>'
                + '<td><button type="button" class="btnRemoveCondition">Remove</button></td></tr>');
            FillConditionsForLogicTableRow(newRow);
            newRow.find(".selectRelation option:first").attr('selected', 'selected');
            newRow.find(".selectField option:first").attr('selected', 'selected');
            newRow.find(".selectOperator option:first").attr('selected', 'selected');

            newRow.find(".btnRemoveCondition").on("click", function () {
                $(this).parents("tr").remove();
            });
            $("#edit-condition-dialog .logicTable").append(newRow);
        });
        $(".btnRemoveCondition").on("click", function () {
            $(this).parents("tr").remove();
        });

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
                $(this).find("#choice-port:first tbody:nth-child(2) tr").remove();
                tbody = $(this).find("#choice-port tbody:nth-child(2)");
                for (i = 1; i <= 5; i++)
                    tbody.append($('<tr class="portRow formRow" portId="' + i + '"><td>' + 'Port' + i + '</td></tr>'));
                if (CurrentItem.data("portId"))
                    tbody.find('tr[portId="' + CurrentItem.data("portId") + '"]').addClass("highlightedRow");
                $(document).on("click", "tr.portRow", function (event) {
                    choosePortDialog.find("#choice-port tbody:nth-child(2) tr").removeClass("highlightedRow");
                    $(this).addClass("highlightedRow");
                });
            }
        });
        function choosePortDialog_SubmitData() {
            selectedRow = choosePortDialog.find("#choice-port:first tbody:nth-child(2) tr.highlightedRow");
            if (selectedRow.length) {
                CurrentItem.data("portId", selectedRow.attr("portId"));
                CurrentItem.text(selectedRow.find("td").text());
                CurrentItem.parents(".rule").data("jsPlumbInstance").recalculateOffsets();
                CurrentItem.parents(".rule").data("jsPlumbInstance").repaintEverything();
                choosePortDialog.dialog("close");
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
                renameRuleDialog.find("#rule-name").val(currentRule.find(".ruleHeader").text());
            }
        });
        function renameRuleDialog_SubmitData() {
            renameRuleDialog.dialog("close");
            currentRule.find(".ruleHeader").text(renameRuleDialog.find("#rule-name").val());
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
                // TODO: replace hardcoded IDs with real app/block IDs
                $.ajax({
                    type: "GET",
                    url: "/api/tapestry/apps/1/blocks/1/commits",
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
                LoadBlock(historyDialog.data("selectedCommitId"));
                historyDialog.dialog("close");
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
    }
});
