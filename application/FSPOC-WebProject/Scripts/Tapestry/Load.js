function LoadBlock(commitId) {
    appId = $("#currentAppId").val();
    blockId = $("#currentBlockId").val();
    if (commitId)
        url = "/api/tapestry/apps/" + appId + "/blocks/" + blockId + "/commits/" + commitId;
    else
        url = "/api/tapestry/apps/" + appId + "/blocks/" + blockId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function (request, status, error) {
            alert(request.responseText);
        },
        success: function (data) {
            ChangedSinceLastSave = false;
            $("#resourceRulesPanel .resourceRule").remove();
            $("#workflowRulesPanel .workflowRule").remove();
            $("#blockHeaderBlockName").text(data.Name);
            for (i = 0; i < data.ResourceRules.length; i++) {
                currentRuleData = data.ResourceRules[i];
                newRule = $('<div class="rule resourceRule" style="width: '+currentRuleData.Width+'px; height: '+currentRuleData.Height+'px; left: '
                    + currentRuleData.PositionX + 'px; top: ' + currentRuleData.PositionY + 'px;"></div>');
                newRule.attr("id", AssingID());
                $("#resourceRulesPanel .scrollArea").append(newRule);
                newRule.draggable({
                    containment: "parent",
                    revert: function (event, ui) {
                        return ($(this).collision("#resourceRulesPanel .resourceRule").length > 1);
                    },
                    stop: function (event, ui) {
                        ChangedSinceLastSave = true;
                    }
                });
                newRule.resizable({
                    start: function (event, ui) {
                        rule = $(this);
                        contentsWidth = 120;
                        contentsHeight = 40;
                        rule.find(".item").each(function (index, element) {
                            rightEdge = $(element).position().left + $(element).width();
                            if (rightEdge > contentsWidth)
                                contentsWidth = rightEdge;
                            bottomEdge = $(element).position().top + $(element).height();
                            if (bottomEdge > contentsHeight)
                                contentsHeight = bottomEdge;
                        });
                        rule.css("min-width", contentsWidth + 40);
                        rule.css("min-height", contentsHeight + 20);

                        limits = CheckRuleResizeLimits(rule, true);
                        rule.css("max-width", limits.horizontal - 10);
                        rule.css("max-height", limits.vertical - 10);
                    },
                    resize: function (event, ui) {
                        rule = $(this);
                        limits = CheckRuleResizeLimits(rule, true);
                        rule.css("max-width", limits.horizontal - 10);
                        rule.css("max-height", limits.vertical - 10);
                    },
                    stop: function (event, ui) {
                        instance = $(this).data("jsPlumbInstance");
                        instance.recalculateOffsets();
                        instance.repaintEverything();
                        ChangedSinceLastSave = true;
                    }
                });
                CreateJsPlumbInstanceForRule(newRule);
                newRule.droppable({
                    containment: ".resourceRule",
                    tolerance: "touch",
                    accept: ".toolboxItem",
                    greedy: true,
                    drop: function (e, ui) {
                        if (dragModeActive) {
                            dragModeActive = false;
                            droppedElement = ui.helper.clone();
                            droppedElement.removeClass("toolboxItem");
                            droppedElement.addClass("item");
                            $(this).append(droppedElement);
                            ruleContent = $(this);
                            leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 20;
                            topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                            droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                            ui.helper.remove();
                            AddToJsPlumb(droppedElement);
                            if (droppedElement.position().left + droppedElement.width() + 35 > ruleContent.width()) {
                                droppedElement.css("left", ruleContent.width() - droppedElement.width() - 40);
                                instance = ruleContent.data("jsPlumbInstance");
                                instance.repaintEverything();
                            }
                            if (droppedElement.position().top + droppedElement.height() + 5 > ruleContent.height()) {
                                console.log("matched");
                                droppedElement.css("top", ruleContent.height() - droppedElement.height() - 15);
                                instance = ruleContent.data("jsPlumbInstance");
                                instance.repaintEverything();
                            }
                            ChangedSinceLastSave = true;
                        }
                    }
                });
                for (j = 0; j < currentRuleData.ResourceItems.length; j++) {
                    currentItemData = currentRuleData.ResourceItems[j];
                    newItem = $('<div id="resItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                        + currentItemData.PositionY + 'px;">'
                        + currentItemData.Label + '</div>');
                    if (currentItemData.ActionId != null)
                        newItem.attr("actionId", currentItemData.ActionId);
                    if (currentItemData.PageId != null)
                        newItem.attr("pageId", currentItemData.PageId);
                    if (currentItemData.ComponentName != null)
                        newItem.attr("componentName", currentItemData.ComponentName);
                    if (currentItemData.TableName != null) {
                        newItem.data("columnFilter", currentItemData.ColumnFilter);
                        newItem.attr("tableName", currentItemData.TableName);
                        newItem.addClass("tableAttribute");
                    }
                    if (currentItemData.ColumnName != null) {
                        newItem.attr("columnName", currentItemData.ColumnName);
                    }
                    if (currentItemData.ConditionSets != null) {
                        newItem.data("conditionSets", currentItemData.ConditionSets);
                    }
                    newItem.addClass(currentItemData.TypeClass);
                    newRule.append(newItem);
                    AddToJsPlumb(newItem);
                }
                currentInstance = newRule.data("jsPlumbInstance");
                for (j = 0; j < currentRuleData.Connections.length; j++) {
                    currentConnectionData = currentRuleData.Connections[j];
                    currentInstance.connect({
                        uuids: ["resItem" + currentConnectionData.Source + "RightMiddle"], target: "resItem" + currentConnectionData.Target
                    });
                }
            }
            for (i = 0; i < data.WorkflowRules.length; i++) {
                currentRuleData = data.WorkflowRules[i];
                newRule = $('<div class="rule workflowRule" style="width: ' + currentRuleData.Width + 'px; height: ' + currentRuleData.Height
                    + 'px; left: ' + currentRuleData.PositionX + 'px; top: ' + currentRuleData.PositionY + 'px;"><div class="workflowRuleHeader">'
                    + '<div class="verticalLabel" style="margin-top: 0px;">' + currentRuleData.Name + '</div></div><div class="swimlaneArea"></div></div>');
                newRule.attr("id", AssingID());
                $("#workflowRulesPanel .scrollArea").append(newRule);
                newRule.draggable({
                    containment: "parent",
                    handle: ".workflowRuleHeader",
                    revert: function (event, ui) {
                        return ($(this).collision("#workflowRulesPanel .workflowRule").length > 1);
                    },
                    stop: function (event, ui) {
                        ChangedSinceLastSave = true;
                    }
                });
                newRule.resizable({
                    start: function (event, ui) {
                        rule = $(this);
                        contentsWidth = 120;
                        contentsHeight = 40;
                        rule.find(".item").each(function (index, element) {
                            rightEdge = $(element).position().left + $(element).width();
                            if (rightEdge > contentsWidth)
                                contentsWidth = rightEdge;
                            bottomEdge = $(element).position().top + $(element).height();
                            if (bottomEdge > contentsHeight)
                                contentsHeight = bottomEdge;
                        });
                        rule.css("min-width", contentsWidth + 40);
                        rule.css("min-height", contentsHeight + 20);

                        limits = CheckRuleResizeLimits(rule, false);
                        rule.css("max-width", limits.horizontal - 10);
                        rule.css("max-height", limits.vertical - 10);
                    },
                    resize: function (event, ui) {
                        rule = $(this);
                        instance = rule.data("jsPlumbInstance");
                        instance.recalculateOffsets();
                        instance.repaintEverything();
                        limits = CheckRuleResizeLimits(rule, false);
                        rule.css("max-width", limits.horizontal - 10);
                        rule.css("max-height", limits.vertical - 10);
                    },
                    stop: function (event, ui) {
                        ChangedSinceLastSave = true;
                    }
                });
                CreateJsPlumbInstanceForRule(newRule);
                for (j = 0; j < currentRuleData.Swimlanes.length; j++) {
                    currentSwimlaneData = currentRuleData.Swimlanes[j];
                    newSwimlane = $('<div class="swimlane" style="height: ' + (100 / currentRuleData.Swimlanes.length) + '%;"><div class="swimlaneRolesArea"><div class="roleItemContainer"></div><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                        + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>');
                    newRule.find(".swimlaneArea").append(newSwimlane);
                    if (currentSwimlaneData.Roles.length > 0)
                        newSwimlane.find(".swimlaneRolesArea .rolePlaceholder").remove();
                    for (k = 0; k < currentSwimlaneData.Roles.length; k++) {
                        newSwimlane.find(".swimlaneRolesArea .roleItemContainer").append($('<div class="roleItem">' + currentSwimlaneData.Roles[k] + '</div>'));
                    }
                    for (k = 0; k < currentSwimlaneData.WorkflowItems.length; k++) {
                        currentItemData = currentSwimlaneData.WorkflowItems[k];
                        newItem = $('<div id="wfItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                            + currentItemData.PositionY + 'px;"><span class="itemLabel">' + currentItemData.Label + '</span></div>');
                        newItem.addClass(currentItemData.TypeClass);
                        if (currentItemData.ActionId != null)
                            newItem.attr('actionId', currentItemData.ActionId);
                        if (currentItemData.InputVariables != null)
                            newItem.data('inputVariables', currentItemData.InputVariables);
                        if (currentItemData.OutputVariables != null)
                            newItem.data('outputVariables', currentItemData.OutputVariables);
                        if (currentItemData.ComponentName != null)
                            newItem.attr('componentName', currentItemData.ComponentName);
                        if (currentItemData.TargetId != null)
                            newItem.attr('targetId', currentItemData.TargetId);
                        if (currentItemData.isAjaxAction != null)
                            newItem.data('isAjaxAction', currentItemData.isAjaxAction);
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneContentArea");
                        targetSwimlane.append(newItem);
                        AddToJsPlumb(newItem);
                    }
                    for (k = 0; k < currentSwimlaneData.WorkflowSymbols.length; k++) {
                        currentSymbolData = currentSwimlaneData.WorkflowSymbols[k];
                        newSymbol = $('<img id="wfSymbol' + currentSymbolData.Id + '" class="symbol" symbolType="' + currentSymbolData.Type +
                            '" src="/Content/images/TapestryIcons/' + currentSymbolData.Type + '.png" style="left: ' + currentSymbolData.PositionX + 'px; top: '
                            + currentSymbolData.PositionY + 'px;" />');
                        if (currentSymbolData.Type == "circle-thick")
                            newSymbol.attr("endpoints", "final");
                        else if (currentSymbolData.Type.substr(0, 8) == "gateway-")
                            newSymbol.attr("endpoints", "gateway");
                        if (currentSymbolData.Condition != null)
                            newSymbol.data("condition", currentSymbolData.Condition);
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneContentArea");
                        targetSwimlane.append(newSymbol);
                        AddToJsPlumb(newSymbol);
                    }
                }
                newRule.find(".swimlaneRolesArea").droppable({
                    tolerance: "touch",
                    accept: ".toolboxItem.roleItem",
                    greedy: true,
                    drop: function (e, ui) {
                        if (dragModeActive) {
                            dragModeActive = false;
                            roleExists = false;
                            $(this).find(".roleItem").each(function (index, element) {
                                if ($(element).text() == ui.helper.text())
                                    roleExists = true;
                            });
                            if (!roleExists) {
                                droppedElement = ui.helper.clone();
                                $(this).find(".rolePlaceholder").remove();
                                $(this).find(".roleItemContainer").append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                                ui.helper.remove();
                                ChangedSinceLastSave = true;
                            }
                        }
                    }
                });
                newRule.find(".swimlaneContentArea").droppable({
                    containment: ".swimlaneContentArea",
                    tolerance: "touch",
                    accept: ".toolboxSymbol, .toolboxItem",
                    greedy: false,
                    drop: function (e, ui) {
                        if (dragModeActive) {
                            dragModeActive = false;
                            droppedElement = ui.helper.clone();
                            if (droppedElement.hasClass("roleItem")) {
                                ui.draggable.draggable("option", "revert", true);
                                return false;
                            }
                            ruleContent = $(this);
                            ruleContent.append(droppedElement);
                            if (droppedElement.hasClass("toolboxSymbol")) {
                                droppedElement.removeClass("toolboxSymbol ui-draggable ui-draggable-dragging");
                                droppedElement.addClass("symbol");
                                leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left;
                                topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top;
                            }
                            else {
                                droppedElement.removeClass("toolboxItem");
                                droppedElement.addClass("item");
                                leftOffset = $("#tapestryWorkspace").offset().left - ruleContent.offset().left + 38;
                                topOffset = $("#tapestryWorkspace").offset().top - ruleContent.offset().top - 18;
                            }
                            droppedElement.offset({ left: droppedElement.offset().left + leftOffset, top: droppedElement.offset().top + topOffset });
                            ui.helper.remove();
                            AddToJsPlumb(droppedElement);
                            if (droppedElement.position().top + droppedElement.height() + 10 > ruleContent.height()) {
                                console.log("matched");
                                droppedElement.css("top", ruleContent.height() - droppedElement.height() - 20);
                                instance = ruleContent.parents(".workflowRule").data("jsPlumbInstance");
                                instance.repaintEverything();
                            }
                            ChangedSinceLastSave = true;
                        }
                    }
                });
                currentInstance = newRule.data("jsPlumbInstance");
                for (j = 0; j < currentRuleData.Connections.length; j++) {
                    currentConnectionData = currentRuleData.Connections[j];
                    sourceId = (currentConnectionData.SourceType == 1 ? "wfSymbol" : "wfItem") + currentConnectionData.Source;
                    targetId = (currentConnectionData.TargetType == 1 ? "wfSymbol" : "wfItem") + currentConnectionData.Target;
                    if (currentConnectionData.SourceSlot == 1)
                        sourceEndpointUuid = "BottomCenter";
                    else
                        sourceEndpointUuid = "RightMiddle";
                    currentInstance.connect({ uuids: [sourceId + sourceEndpointUuid], target: targetId });
                }
            }
            appId = $("#currentAppId").val();
            $.ajax({
                type: "GET",
                url: "/api/database/apps/" + appId + "/commits/latest",
                dataType: "json",
                success: function (tableData) {
                    $("#libraryCategory-Attributes .libraryItem").remove();
                    for (tableIndex = 0; tableIndex < tableData.Tables.length; tableIndex++) {
                        $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="table-attribute" class="libraryItem tableAttribute" tableName="'
                            + tableData.Tables[tableIndex].Name + '">Table: ' + tableData.Tables[tableIndex].Name + '</div>'));
                    }
                    AssociatedTableIds = data.AssociatedTableIds;
                    AssociatedTableName = data.AssociatedTableName;
                    $("#blockHeaderDbResCount").text(data.AssociatedTableIds.length);
                    somethingWasAdded = false;
                    for (tableIndex = 0; tableIndex < data.AssociatedTableIds.length; tableIndex++) {
                        currentTable = tableData.Tables.filter(function (value) {
                            return value.Id == data.AssociatedTableIds[tableIndex];
                        })[0];
                        if (currentTable != undefined) {
                            for (columnIndex = 0; columnIndex < currentTable.Columns.length; columnIndex++) {
                                $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                    + currentTable.Name + '" columnName="' + currentTable.Columns[columnIndex].Name + '">' + currentTable.Name + '.' + currentTable.Columns[columnIndex].Name + '</div>'));
                            }
                        }
                    };
                }
            });
            $('#libraryCategory-Actions .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/tapestry/actions",
                dataType: "json",
                success: function (actionData) {
                    for (actionIndex = 0; actionIndex < actionData.Items.length; actionIndex++)
                    {
                        $('#libraryCategory-Actions').append('<div libId="' + ++lastLibId + '" libType="action" class="libraryItem" actionId="' + actionData.Items[actionIndex].Id + '">' + actionData.Items[actionIndex].Name + '</div>');
                    }
                }
            });
            $('#libraryCategory-Roles .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-roles/" + appId,
                dataType: "json",
                success: function (roleData) {
                    for (roleIndex = 0; roleIndex < roleData.Roles.length; roleIndex++) {
                        $('#libraryCategory-Roles').append('<div libId="' + roleData.Roles[roleIndex].Id + '" libType="role" class="libraryItem">' + roleData.Roles[roleIndex].Name + '</div>');
                    }
                }
            });
            $('#libraryCategory-States .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-states/" + appId,
                dataType: "json",
                success: function (stateData) {
                    for (stateIndex = 0; stateIndex < stateData.States.length; stateIndex++) {
                        $('#libraryCategory-States').append('<div libId="' + ++lastLibId + '" libType="state" class="libraryItem" stateId="' + stateData.States[stateIndex].Id + '">' + stateData.States[stateIndex].Name + '</div>');
                    }
                }
            });
            $('#libraryCategory-Targets .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/tapestry/apps/" + appId + "/blocks",
                dataType: "json",
                success: function(targetData) {
                    for (targetIndex = 0; targetIndex < targetData.ListItems.length; targetIndex++) {
                        $('#libraryCategory-Targets').append('<div libId="' + ++lastLibId + '" libType="target" class="libraryItem" targetId="' + targetData.ListItems[targetIndex].Id + '">' + targetData.ListItems[targetIndex].Name + '</div>');
                    }
                }
            });
            $('#libraryCategory-Templates .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/hermes/" + appId + "/templates",
                dataType: "json",
                success: function (templateData) {
                    for (templateIndex = 0; templateIndex < templateData.length; templateIndex++) {
                        $('#libraryCategory-Templates').append('<div libId="' + templateData[templateIndex].Id + '" libType="template" class="libraryItem">' + templateData[templateIndex].Name + '</div>');
                    }
                }
            });
            $('#libraryCategory-Integration .libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/nexus/" + appId + "/gateways",
                dataType: "json",
                success: function (integrationData) {
                    for (integrationIndex = 0; integrationIndex < integrationData.Ldap.length; integrationIndex++) {
                        $('#libraryCategory-Integration').append('<div libId="' + ++lastLibId + '" libType="ldap" class="libraryItem">LDAP: ' + integrationData.Ldap[integrationIndex].Name + '</div>');
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.WS.length; integrationIndex++) {
                        $('#libraryCategory-Integration').append('<div libId="' + ++lastLibId + '" libType="ws" libSubType="' + integrationData.WS[integrationIndex].Type + '" class="libraryItem">WS: ' + integrationData.WS[integrationIndex].Name + '</div>');
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.SMTP.length; integrationIndex++) {
                        $('#libraryCategory-Integration').append('<div libId="' + ++lastLibId + '" libType="smtp" class="libraryItem">SMTP: ' + integrationData.SMTP[integrationIndex].Name + '</div>');
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.WebDAV.length; integrationIndex++) {
                        $('#libraryCategory-Integration').append('<div libId="' + ++lastLibId + '" libType="webdav" class="libraryItem">WebDAV: ' + integrationData.WebDAV[integrationIndex].Name + '</div>');
                    }
                }
            });
            AssociatedPageIds = data.AssociatedPageIds;
            $("#blockHeaderScreenCount").text(data.AssociatedPageIds.length);
            $("#libraryCategory-UI .libraryItem").remove();
            for (pageIndex = 0; pageIndex < data.AssociatedPageIds.length; pageIndex++) {
                pageId = data.AssociatedPageIds[pageIndex];
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId,
                    dataType: "json",
                    success: function (uiPageData) {
                        for (componentIndex = 0; componentIndex < uiPageData.Components.length; componentIndex++) {
                            if (componentIndex == 0) {
                                $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" libType="ui" class="libraryItem">Screen: '
                                    + uiPageData.Name + '</div>');
                            }
                            $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].Name + '" libType="ui" class="libraryItem">'
                            + uiPageData.Components[componentIndex].Name + '</div>');
                            if (uiPageData.Components[componentIndex].Type == "data-table-with-actions") {
                                $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_edit" libType="ui" class="libraryItem">'
                                    + uiPageData.Components[componentIndex].Name + '_EditAction</div>');
                                $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_detail" libType="ui" class="libraryItem">'
                                    + uiPageData.Components[componentIndex].Name + '_DetailsAction</div>');
                                $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_delete" libType="ui" class="libraryItem">'
                                    + uiPageData.Components[componentIndex].Name + '_DeleteAction</div>');
                            }
                            if (uiPageData.Components[componentIndex].ChildComponents) {
                                for (childComponentIndex = 0; childComponentIndex < uiPageData.Components[componentIndex].ChildComponents.length; childComponentIndex++) {
                                    $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '" libType="ui" class="libraryItem">'
                                    + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '</div>');
                                }
                            }
                        }
                    }
                });
            }
        }
    });
};
