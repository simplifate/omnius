var CurrentRule, CurrentItem, AssociatedPageIds = [], AssociatedTableName = [], AssociatedTableIds = [], CurrentTableColumnArray = [], RoleWhitelist = [], ModelTableName;

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
                renameRuleDialog.find("#rule-name").val(CurrentRule.find(".workflowRuleHeader .verticalLabel").text());
            }
        });
        function renameRuleDialog_SubmitData() {
            renameRuleDialog.dialog("close");
            CurrentRule.find(".workflowRuleHeader .verticalLabel").text(renameRuleDialog.find("#rule-name").val());
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
                historyDialog.find("#commit-table:first tbody:nth-child(2) tr").remove();
                historyDialog.find(" .spinner-2").show();
                historyDialog.data("selectedCommitId", null);
                appId = $("#currentAppId").val();
                blockId = $("#currentBlockId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/tapestry/apps/" + appId + "/blocks/" + blockId + "/commits",
                    dataType: "json",
                    success: function (data) {
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

                        historyDialog.find(".spinner-2").hide();
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
                chooseScreensDialog.find("#screen-table:first tbody:nth-child(2) tr").remove();
                chooseScreensDialog.find(".spinner-2").show();
                appId = $("#currentAppId").val();
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages",
                    dataType: "json",
                    success: function (data) {
                        tbody = chooseScreensDialog.find("#screen-table tbody:nth-child(2)");
                        for (i = 0; i < data.length; i++) {
                            newScreenRow = $('<tr class="screenRow" pageId="' + data[i].Id + '"><td>' + data[i].Name + '</td></tr>');
                            if (AssociatedPageIds.indexOf(data[i].Id) != -1)
                                newScreenRow.addClass("highlightedRow");
                            tbody.append(newScreenRow);
                        }
                        $("#screen-table .screenRow").on("click", function () {
                            $(this).toggleClass("highlightedRow");
                        });
                        chooseScreensDialog.find(".spinner-2").hide();
                    }
                });
            }
        });
        function chooseScreensDialog_SubmitData() {
            chooseScreensDialog.find("#screen-table:first tbody:nth-child(2) tr").hide();
            chooseScreensDialog.find(".spinner-2").show();
            somethingWasAdded = false;
            pageCount = 0;
            appId = $("#currentAppId").val();
            AssociatedPageIds = [];
            $("#libraryCategory-UI .libraryItem").remove();
            setTimeout(function () {
                chooseScreensDialog.find("#screen-table:first tbody:nth-child(2) tr").each(function (index, element) {
                    if ($(element).hasClass("highlightedRow")) {
                        pageCount++;
                        pageId = $(element).attr("pageId");
                        AssociatedPageIds.push(parseInt(pageId));
                        url = "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId;
                        $.ajax({
                            type: "GET",
                            url: url,
                            dataType: "json",
                            async: false,
                            success: function (data) {
                                for (i = 0; i < data.Components.length; i++) {
                                    if (i == 0) {
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" libType="ui" class="libraryItem">Screen: '
                                            + data.Name + '</div>');
                                    }
                                    cData = data.Components[i];
                                    $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + data.Components[i].Name + '" libType="ui" class="libraryItem">'
                                    + cData.Name + '</div>');
                                    if (cData.Type == "data-table-with-actions") {
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.Name + '_EditAction" libType="ui" class="libraryItem">'
                                            + cData.Name + '_EditAction</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.Name + '_DetailsAction" libType="ui" class="libraryItem">'
                                            + cData.Name + '_DetailsAction</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.Name + '_DeleteAction" libType="ui" class="libraryItem">'
                                            + cData.Name + '_DeleteAction</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.Name + '_A_Action" libType="ui" class="libraryItem">'
                                            + cData.Name + '_A_Action</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.Name + '_B_Action" libType="ui" class="libraryItem">'
                                            + cData.Name + '_B_Action</div>');
                                    }
                                    if (cData.ChildComponents) {
                                        for (j = 0; j < cData.ChildComponents.length; j++) {
                                            $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + data.Id + '" componentName="' + cData.ChildComponents[j].Name + '" libType="ui" class="libraryItem">'
                                            + cData.ChildComponents[j].Name + '</div>');
                                        }
                                    }
                                }
                            }
                        });
                    }
                });
                $("#blockHeaderScreenCount").text(pageCount);
                chooseScreensDialog.find(".spinner-2").hide();
                chooseScreensDialog.dialog("close");
            }, 4);
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
                    CurrentItem.removeClass("activeItem");
                }
            },
            open: function (event, ui) {
                var formTable = tableAttributePropertiesDialog.find(".columnFilterTable tbody");
                formTable.find("tr").remove();
                $("#table-attribute-properties-dialog .spinner-2").show();
                $("#btnOpenTableConditions").hide();
                appId = $("#currentAppId").val();
                url = "/api/database/apps/" + appId + "/commits/latest",
                tableName = CurrentItem.attr("tableName");
                $.ajax({
                    type: "GET",
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        CurrentTableColumnArray = [];
                        columnFilter = CurrentItem.data("columnFilter");
                        if (columnFilter == undefined)
                            columnFilter = [];
                        targetTable = data.Tables.filter(function (value, index, ar) {
                            return value.Name == tableName;
                        })[0];
                        if (targetTable == undefined)
                            alert("Požadovaná tabulka již není součástí schématu v Entitronu, nebo má nyní jiné Id.");
                        for (i = 0; i < targetTable.Columns.length; i++) {
                            newRow = $('<tr><td class="nameCell">' + targetTable.Columns[i].Name + '</td>'
                                + '<td><input type="checkbox" class="showColumnCheckbox"></input>Show</td></tr>');
                            formTable.append(newRow);
                            newRow.find(".showColumnCheckbox").prop("checked", columnFilter.indexOf(targetTable.Columns[i].Name) != -1);
                            CurrentTableColumnArray.push({ Id: targetTable.Columns[i].Id, Name: targetTable.Columns[i].Name, Type: targetTable.Columns[i].Type });
                        }
                        $("#btnOpenTableConditions").show();
                        $("#table-attribute-properties-dialog .spinner-2").hide();
                    }
                });
            }
        });
        function tableAttributePropertiesDialog_SubmitData() {
            columnFilter = [];
            formTable = tableAttributePropertiesDialog.find(".columnFilterTable .showColumnCheckbox").each(function (index, checkboxElement) {
                columnName = $(checkboxElement).parents("tr").find(".nameCell").text();
                if($(checkboxElement).is(":checked"))
                    columnFilter.push(columnName);
            });
            CurrentItem.data("columnFilter", columnFilter).removeClass("activeItem");;
            tableAttributePropertiesDialog.dialog("close");
        }
        uiitemPropertiesDialog = $("#uiItem-properties-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 180,
            buttons: {
                "Save": function () {
                    uiitemPropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    uiitemPropertiesDialog.dialog("close");
                    CurrentItem.removeClass("activeItem");
                }
            },
            open: function (event, ui) {
                uiitemPropertiesDialog.find("#ajax-action").prop('checked', CurrentItem.data("isAjaxAction"));
            }
        });
        function uiitemPropertiesDialog_SubmitData() {
            CurrentItem.data("isAjaxAction", uiitemPropertiesDialog.find("#ajax-action").is(':checked'));
            uiitemPropertiesDialog.dialog("close");
            CurrentItem.removeClass("activeItem");
        }
        chooseTablesDialog = $("#choose-tables-dialog").dialog({
            autoOpen: false,
            width: 450,
            height: 550,
            buttons: {
                "Select": function () {
                    chooseTablesDialog_SubmitData();
                },
                Cancel: function () {
                    chooseTablesDialog.dialog("close");
                }
            },
            create: function () {
                $(document).on("click", "tr.tableRow", function (event) {
                    $(this).toggleClass("highlightedRow");
                });
            },
            open: function (event, ui) {
                chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").remove();
                chooseTablesDialog.find(".spinner-2").show();
                appId = $("#currentAppId").val();
                url = "/api/database/apps/" + appId + "/commits/latest";
                $.ajax({
                    type: "GET",
                    url: url,
                    dataType: "json",
                    success: function (data) {
                        tbody = chooseTablesDialog.find("#table-table tbody:nth-child(2)");
                        for (i = 0; i < data.Tables.length; i++) {
                            newTableRow = $('<tr class="tableRow" tableId="' + data.Tables[i].Id + '"><td><span class="tableName">' + data.Tables[i].Name + '</span></td></tr>');
                            if (AssociatedTableName.indexOf(data.Tables[i].Name) != -1)
                                newTableRow.addClass("highlightedRow");
                            if (data.Tables[i].Name == ModelTableName)
                                newTableRow.find("td").append('<div class="modelMarker">Model</div>');
                            tbody.append(newTableRow);
                        }
                        for (i = 0; i < SystemTables.length; i++) {
                            newTableRow = $('<tr class="tableRow" tableId="' + SystemTables[i].Name + '"><td><span class="tableName">' + SystemTables[i].Name + '</span></td></tr>');
                            if (AssociatedTableName.indexOf(SystemTables[i].Name) != -1)
                                newTableRow.addClass("highlightedRow");
                            if (SystemTables[i].Name == ModelTableName)
                                newTableRow.find("td").append('<div class="modelMarker">Model</div>');
                            tbody.append(newTableRow);
                        }
                        chooseTablesDialog.find(".spinner-2").hide();
                    }
                });
            }
        });
        function chooseTablesDialog_SubmitData() {
            chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").hide();
            chooseTablesDialog.find(".spinner-2").show();
            appId = $("#currentAppId").val();
            url = "/api/database/apps/" + appId + "/commits/latest";
            $.ajax({
                type: "GET",
                url: url,
                dataType: "json",
                success: function (data) {
                    $("#libraryCategory-Attributes .columnAttribute").remove();
                    somethingWasAdded = false;
                    tableCount = 0;
                    AssociatedTableIds = [];
                    AssociatedTableName = [];
                    chooseTablesDialog.find("#table-table:first tbody:nth-child(2) tr").each(function (index, element) {
                        if ($(element).hasClass("highlightedRow")) {
                            tableCount++;
                            tableId = $(element).attr("tableId");
                            tableName = $(element).find('td .tableName').text();
                            AssociatedTableIds.push(parseInt(tableId));
                            AssociatedTableName.push(tableName);
                            currentTable = data.Tables.filter(function (value) {
                                return value.Id == tableId;
                            })[0];
                            if (currentTable)
                                for (i = 0; i < currentTable.Columns.length; i++) {
                                    $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                        + currentTable.Name + '" columnName="' + currentTable.Columns[i].Name + '">' + currentTable.Name + '.' + currentTable.Columns[i].Name + '</div>'));
                                }
                            systemTable = SystemTables.filter(function (value) {
                                return value.Name == tableName;
                            })[0];
                            if(systemTable)
                                for (i = 0; i < systemTable.Columns.length; i++) {
                                    $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                        + systemTable.Name + '" columnName="' + systemTable.Columns[i] + '">' + systemTable.Name + '.' + systemTable.Columns[i] + '</div>'));
                                }
                        }
                    });
                    modelMarker = chooseTablesDialog.find("#table-table:first tbody:nth-child(2) .modelMarker");
                    if (modelMarker.length) {
                        ModelTableName = modelMarker.parents("td").find(".tableName").text();
                    }
                    $("#blockHeaderDbResCount").text(tableCount);
                    chooseTablesDialog.dialog("close");
                    chooseTablesDialog.find(".spinner-2").hide();
                }
            });
        }
        actionPropertiesDialog = $("#action-properties-dialog").dialog({
            autoOpen: false,
            width: 900,
            height: 200,
            buttons: {
                "Save": function () {
                    actionPropertiesDialog_SubmitData();
                },
                Cancel: function () {
                    actionPropertiesDialog.dialog("close");
                    CurrentItem.removeClass("activeItem");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        actionPropertiesDialog_SubmitData();
                        return false;
                    }
                })
            },
            open: function () {
                actionPropertiesDialog.find("#input-variables").val(CurrentItem.data("inputVariables"));
                actionPropertiesDialog.find("#output-variables").val(CurrentItem.data("outputVariables"));
            }
        });
        function actionPropertiesDialog_SubmitData() {
            CurrentItem.data("inputVariables", actionPropertiesDialog.find("#input-variables").val());
            CurrentItem.data("outputVariables", actionPropertiesDialog.find("#output-variables").val());
            actionPropertiesDialog.dialog("close");
        }
        labelPropertyDialog = $("#label-property-dialog").dialog({
            autoOpen: false,
            width: 900,
            height: 200,
            buttons: {
                "Save": function () {
                    labelPropertyDialog_SubmitData();
                },
                Cancel: function () {
                    labelPropertyDialog.dialog("close");
                    CurrentItem.removeClass("activeItem processedItem");
                }
            },
            open: function () {
                labelPropertyDialog.find("#label-input").val(CurrentItem.find(".itemLabel").text());
            }
        });
        function labelPropertyDialog_SubmitData() {
            CurrentItem.find(".itemLabel").text(labelPropertyDialog.find("#label-input").val());
            CurrentItem.removeClass("activeItem processedItem");
            labelPropertyDialog.dialog("close");
        }
        conditionsDialog = $("#conditions-dialog").dialog({
            autoOpen: false,
            width: 800,
            height: 560,
            buttons: {
                "Save": function () {
                    conditionsDialog_SubmitData();
                },
                Cancel: function () {
                    conditionsDialog.dialog("close");
                }
            },
            create: function () {
                $(this).keypress(function (e) {
                    if (e.keyCode == $.ui.keyCode.ENTER) {
                        conditionsDialog_SubmitData();
                        return false;
                    }
                });
                $(this).find(".addAndConditionSetIcon").on("click", function () {
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text("AND a");
                    newConditionSet.find(".conditionTable").append($(ConditionTemplate))
                    LoadConditionColumns(newConditionSet);
                    conditionsDialog.find(".conditionSetArea").append(newConditionSet);
                    if (newConditionSet.index() == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                });
                $(this).find(".addOrConditionSetIcon").on("click", function () {
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text("OR a");
                    newConditionSet.find(".conditionTable").append($(ConditionTemplate))
                    LoadConditionColumns(newConditionSet);
                    conditionsDialog.find(".conditionSetArea").append(newConditionSet);
                    if (newConditionSet.index() == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                });
                $(this).on("click", ".addAndConditionIcon", function () {
                    newCondition = $(ConditionTemplate);
                    newCondition.find(".conditionOperator").text("and");
                    LoadConditionColumns(newCondition);
                    $(this).parents("tr").after(newCondition);
                });
                $(this).on("click", ".addOrConditionIcon", function () {
                    newCondition = $(ConditionTemplate);
                    newCondition.find(".conditionOperator").text("or");
                    LoadConditionColumns(newCondition);
                    $(this).parents("tr").after(newCondition);
                });
                $(this).on("click", ".removeConditionIcon", function () {
                    currentCondition = $(this).parents("tr");
                    if (currentCondition.index() == 0)
                        currentCondition.parents("table").find("tr:eq(1)").find(".conditionOperator").text("");
                    if (currentCondition.parents("table").find("tr").length == 1) {
                        if (currentCondition.parents(".conditionSet").index() == 0)
                            currentCondition.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                        currentCondition.parents(".conditionSet").remove();
                    }
                    else
                        currentCondition.remove();
                });
                $(this).on("click", ".removeConditionSetIcon", function () {
                    currentConditionSet = $(this).parents(".conditionSet");
                    if (currentConditionSet.index() == 0)
                        currentConditionSet.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                    currentConditionSet.remove();
                });
                $(this).on("change", ".conditionVariableCell select", function () {
                    currentCondition = $(this).parents("tr");
                    var optionSelected = $("option:selected", this);
                    varType = optionSelected.attr("varType");
                    currentCondition.find(".conditionOperatorCell select, .conditionValueCell select, .conditionValueCell input").remove();
                    switch(varType) {
                        case "bool":
                            currentCondition.find(".conditionValueCell").append($('<select><option selected="selected">b$true</option><option>b$false</option></select>'));
                            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option></select>'));
                            break;
                        case "int":
                            currentCondition.find(".conditionValueCell").append($('<input type="number"></input>'));
                            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option>'));
                            break;
                        case "string":
                            currentCondition.find(".conditionValueCell").append($('<input type="text"></input>'));
                            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                            break;
                        case "unknown":
                        default:
                            currentCondition.find(".conditionValueCell").append($('<input type="text"></input>'));
                            currentCondition.find(".conditionOperatorCell").append($('<select><option selected="selected">==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                    }
                });
                $(this).on("change", ".conditionOperatorCell select", function () {
                    currentCondition = $(this).parents("tr");
                    var optionSelected = $("option:selected", this);
                    inputType = optionSelected.attr("inputType");
                    if (inputType === "none")
                        currentCondition.find(".conditionValueCell input, .conditionValueCell select").hide();
                    else
                        currentCondition.find(".conditionValueCell input, .conditionValueCell select").show();
                });
            },
            open: function () {
                conditionSetArea = conditionsDialog.find(".conditionSetArea");
                conditionSetArea.find(".conditionSet").remove();
                conditionSetData = CurrentItem.data("conditionSets");
                for (conditionSetIndex = 0; conditionSetIndex < conditionSetData.length; conditionSetIndex++) {
                    currentConditionSetData = conditionSetData[conditionSetIndex];
                    if (currentConditionSetData.SetRelation == "OR")
                        prefix = "OR a";
                    else
                        prefix = "AND a";
                    newConditionSet = $(ConditionSetTemplate);
                    newConditionSet.find(".conditionSetPrefix").text(prefix);
                    conditionSetArea.append(newConditionSet);
                    if (conditionSetIndex == 0)
                        newConditionSet.find(".conditionSetPrefix").text("A");
                    conditionTable = newConditionSet.find(".conditionTable");
                    for (conditionIndex = 0; conditionIndex < currentConditionSetData.Conditions.length; conditionIndex++)
                    {
                        currentConditionData = currentConditionSetData.Conditions[conditionIndex];
                        newCondition = $(ConditionTemplate);
                        if (conditionIndex > 0)
                            newCondition.find(".conditionOperator").text(currentConditionData.Relation.toLowerCase());
                        conditionTable.append(newCondition);
                        columnSelect = newCondition.find(".conditionVariableCell select");
                        for (i = 0; i < CurrentTableColumnArray.length; i++) {
                            cData = CurrentTableColumnArray[i];
                            switch (cData.Type) {
                                case "varchar":
                                    columnType = "string";
                                    break;
                                case "boolean":
                                    columnType = "bool";
                                    break;
                                case "integer":
                                    columnType = "int";
                                    break;
                                default:
                                    columnType = "unknown";
                            }
                            columnSelect.append($('<option varType="' + columnType + '">' + cData.Name + '</option>'));
                        }
                        columnSelect.val(currentConditionData.Variable);
                        var optionSelected = $("option:selected", columnSelect);
                        varType = optionSelected.attr("varType");
                        newCondition.find(".conditionOperatorCell select, .conditionValueCell select, .conditionValueCell input").remove();
                        conditionValueCell = newCondition.find(".conditionValueCell");
                        conditionOperatorCell = newCondition.find(".conditionOperatorCell");
                        switch (varType) {
                            case "bool":
                                conditionValueCell.append($('<select><option selected="selected">b$true</option><option>b$false</option></select>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "int":
                                conditionValueCell.append($('<input type="number"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "string":
                                conditionValueCell.append($('<input type="text"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                                break;
                            case "unknown":
                            default:
                                conditionValueCell.append($('<input type="text"></input>'));
                                conditionOperatorCell.append($('<select><option>==</option><option>!=</option><option>&gt;</option><option>&gt;=</option><option>&lt;</option><option>&lt;=</option><option>contains</option><option inputType="none">is empty</option><option inputType="none">is not empty</option></select>'));
                                conditionOperatorCell.find("select").val(currentConditionData.Operator);
                        }
                        var optionSelected = $("option:selected", conditionOperatorCell);
                        inputType = optionSelected.attr("inputType");
                        if (inputType === "none")
                            conditionValueCell.find("input, select").hide();
                        else {
                            if (conditionValueCell.find("input").length > 0) {
                                conditionValueCell.find("input").show();
                                conditionValueCell.find("input").val(currentConditionData.Value);
                            }
                            else if (conditionValueCell.find("select").length > 0) {
                                conditionValueCell.find("select").show();
                                conditionValueCell.find("select").val(currentConditionData.Value);
                            }
                        }
                    }
                }
            }
        });
        function conditionsDialog_SubmitData() {
            setArray = [];
            conditionsDialog.find(".conditionSet").each(function (setIndex, setElement) {
                currentSet = $(setElement);
                conditionArray = [];
                currentSet.find(".conditionTable tr").each(function (index, element) {
                    currentCondition = $(element);
                    relationCellValue = currentCondition.find(".conditionOperator").text();
                    if (relationCellValue == "")
                        relation = "AND";
                    else
                        relation = relationCellValue.toUpperCase();
                    if (currentCondition.find(".conditionValueCell select").length > 0)
                        value = currentCondition.find(".conditionValueCell select option:selected").text();
                    else
                        value = currentCondition.find(".conditionValueCell input").val();
                    conditionArray.push({
                        Index: index,
                        Relation: relation,
                        Variable: currentCondition.find(".conditionVariableCell select option:selected").text(),
                        Operator: currentCondition.find(".conditionOperatorCell select option:selected").text(),
                        Value: value
                    });
                });
                setPrefix = currentSet.find(".conditionSetPrefix").text();
                if (setPrefix == "OR a")
                    setRelation = "OR";
                else
                    setRelation = "AND";
                setArray.push({
                    SetIndex: setIndex,
                    SetRelation: setRelation,
                    Conditions: conditionArray
                });
            });
            CurrentItem.data("conditionSets", setArray);
            conditionsDialog.dialog("close");
        }
    }
    chooseWhitelistRolesDialog = $("#choose-whitelist-roles-dialog").dialog({
        autoOpen: false,
        width: 450,
        height: 500,
        buttons: {
            "Change": function () {
                chooseWhitelistRolesDialog_SubmitData();
            },
            Cancel: function () {
                chooseWhitelistRolesDialog.dialog("close");
            }
        },
        open: function (event, ui) {
            chooseWhitelistRolesDialog.find("#role-table:first tbody:nth-child(2) tr").remove();
            chooseWhitelistRolesDialog.find(".spinner-2").show();
            appId = $("#currentAppId").val();
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-roles/" + appId,
                dataType: "json",
                success: function (data) {
                    tbody = chooseWhitelistRolesDialog.find("#role-table tbody:nth-child(2)");
                    for (i = 0; i < data.Roles.length; i++) {
                        newTableRow = $('<tr class="roleRow"><td>' + data.Roles[i].Name + '</td></tr>');
                        if (RoleWhitelist.indexOf(data.Roles[i].Name) != -1)
                            newTableRow.addClass("highlightedRow");
                        tbody.append(newTableRow);
                        newTableRow.on("click", function (event) {
                            $(this).toggleClass("highlightedRow");
                        });
                    }
                    chooseWhitelistRolesDialog.find(".spinner-2").hide();
                }
            });
        }
    });
    function chooseWhitelistRolesDialog_SubmitData() {
        RoleWhitelist = [];
        roleCount = 0;
        chooseWhitelistRolesDialog.find("#role-table:first tbody:nth-child(2) tr").each(function (index, element) {
            if ($(element).hasClass("highlightedRow")) {
                RoleWhitelist.push($(element).find("td").text());
                roleCount++;
            }
        });
        $("#blockHeaderRolesCount").text(roleCount);
        chooseWhitelistRolesDialog.dialog("close");
    }
    gatewayConditionsDialog = $("#gateway-conditions-dialog").dialog({
        autoOpen: false,
        width: 800,
        height: 560,
        buttons: {
            "Save": function () {
                gatewayConditionsDialog_SubmitData();
            },
            Cancel: function () {
                gatewayConditionsDialog.dialog("close");
                CurrentItem.removeClass("activeItem");
            }
        },
        create: function () {
            $(this).keypress(function (e) {
                if (e.keyCode == $.ui.keyCode.ENTER) {
                    gatewayConditionsDialog_SubmitData();
                    return false;
                }
            });
            $(this).find(".addAndConditionSetIcon").on("click", function () {
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text("AND a");
                newConditionSet.find(".conditionTable").append($(ManualInputConditionTemplate));
                gatewayConditionsDialog.find(".conditionSetArea").append(newConditionSet);
                if (newConditionSet.index() == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
            });
            $(this).find(".addOrConditionSetIcon").on("click", function () {
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text("OR a");
                newConditionSet.find(".conditionTable").append($(ManualInputConditionTemplate));
                gatewayConditionsDialog.find(".conditionSetArea").append(newConditionSet);
                if (newConditionSet.index() == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
            });
            $(this).on("click", ".addAndConditionIcon", function () {
                newCondition = $(ManualInputConditionTemplate);
                newCondition.find(".conditionOperator").text("and");
                $(this).parents("tr").after(newCondition);
            });
            $(this).on("click", ".addOrConditionIcon", function () {
                newCondition = $(ManualInputConditionTemplate);
                newCondition.find(".conditionOperator").text("or");
                $(this).parents("tr").after(newCondition);
            });
            $(this).on("click", ".removeConditionIcon", function () {
                currentCondition = $(this).parents("tr");
                if (currentCondition.index() == 0)
                    currentCondition.parents("table").find("tr:eq(1)").find(".conditionOperator").text("");
                if (currentCondition.parents("table").find("tr").length == 1) {
                    if (currentCondition.parents(".conditionSet").index() == 0)
                        currentCondition.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                    currentCondition.parents(".conditionSet").remove();
                }
                else
                    currentCondition.remove();
            });
            $(this).on("click", ".removeConditionSetIcon", function () {
                currentConditionSet = $(this).parents(".conditionSet");
                if (currentConditionSet.index() == 0)
                    currentConditionSet.parents(".conditionSetArea").find(".conditionSet:eq(1)").find(".conditionSetPrefix").text("A");
                currentConditionSet.remove();
            });
            $(this).on("change", ".conditionOperatorCell select", function () {
                currentCondition = $(this).parents("tr");
                var optionSelected = $("option:selected", this);
                inputType = optionSelected.attr("inputType");
                if (inputType === "none")
                    currentCondition.find(".conditionValueCell input, .conditionValueCell select").hide();
                else
                    currentCondition.find(".conditionValueCell input, .conditionValueCell select").show();
            });
        },
        open: function () {
            conditionSetArea = gatewayConditionsDialog.find(".conditionSetArea");
            conditionSetArea.find(".conditionSet").remove();
            conditionSetData = CurrentItem.data("conditionSets");
            if (!conditionSetData)
                conditionSetData = [];
            for (conditionSetIndex = 0; conditionSetIndex < conditionSetData.length; conditionSetIndex++) {
                currentConditionSetData = conditionSetData[conditionSetIndex];
                if (currentConditionSetData.SetRelation == "OR")
                    prefix = "OR a";
                else
                    prefix = "AND a";
                newConditionSet = $(ConditionSetTemplate);
                newConditionSet.find(".conditionSetPrefix").text(prefix);
                conditionSetArea.append(newConditionSet);
                if (conditionSetIndex == 0)
                    newConditionSet.find(".conditionSetPrefix").text("A");
                conditionTable = newConditionSet.find(".conditionTable");
                for (conditionIndex = 0; conditionIndex < currentConditionSetData.Conditions.length; conditionIndex++) {
                    currentConditionData = currentConditionSetData.Conditions[conditionIndex];
                    newCondition = $(ManualInputConditionTemplate);
                    if (conditionIndex > 0)
                        newCondition.find(".conditionOperator").text(currentConditionData.Relation.toLowerCase());
                    conditionTable.append(newCondition);
                    columnSelect = newCondition.find(".conditionVariableCell input");
                    columnSelect.val(currentConditionData.Variable);
                    conditionOperatorCell = newCondition.find(".conditionOperatorCell");
                    conditionOperatorCell.find("select").val(currentConditionData.Operator);
                    conditionValueCell = newCondition.find(".conditionValueCell");
                    var optionSelected = $("option:selected", conditionOperatorCell);
                    inputType = optionSelected.attr("inputType");
                    if (inputType === "none")
                        conditionValueCell.find("input").hide();
                    else {
                        conditionValueCell.find("input").show();
                        conditionValueCell.find("input").val(currentConditionData.Value);
                    }
                }
            }
        }
    });
    function gatewayConditionsDialog_SubmitData() {
        setArray = [];
        gatewayConditionsDialog.find(".conditionSet").each(function (setIndex, setElement) {
            currentSet = $(setElement);
            conditionArray = [];
            currentSet.find(".conditionTable tr").each(function (index, element) {
                currentCondition = $(element);
                relationCellValue = currentCondition.find(".conditionOperator").text();
                if (relationCellValue == "")
                    relation = "AND";
                else
                    relation = relationCellValue.toUpperCase();
                conditionArray.push({
                    Index: index,
                    Relation: relation,
                    Variable: currentCondition.find(".conditionVariableCell input").val(),
                    Operator: currentCondition.find(".conditionOperatorCell select option:selected").text(),
                    Value: currentCondition.find(".conditionValueCell input").val()
                });
            });
            setPrefix = currentSet.find(".conditionSetPrefix").text();
            if (setPrefix == "OR a")
                setRelation = "OR";
            else
                setRelation = "AND";
            setArray.push({
                SetIndex: setIndex,
                SetRelation: setRelation,
                Conditions: conditionArray
            });
        });
        CurrentItem.data("conditionSets", setArray).removeClass("activeItem");
        gatewayConditionsDialog.dialog("close");
    }
});
