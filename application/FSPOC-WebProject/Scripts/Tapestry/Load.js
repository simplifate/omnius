function LoadBlock(commitId) {
    pageSpinner.show();

    appId = $("#currentAppId").val();
    blockId = $("#currentBlockId").val();
    if (commitId) {
        url = "/api/tapestry/apps/" + appId + "/blocks/" + blockId + "/commits/" + commitId;
    } else {
        url = "/api/tapestry/apps/" + appId + "/blocks/" + blockId;
    }

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
                    if (currentItemData.StateId != null)
                        newItem.attr("stateId", currentItemData.StateId);
                    if (currentItemData.PageId != null)
                        newItem.attr("pageId", currentItemData.PageId);
                    if (currentItemData.ComponentName != null)
                        newItem.attr("componentName", currentItemData.ComponentName);
                    if (currentItemData.TableName != null) {
                        newItem.data("columnFilter", currentItemData.ColumnFilter);
                        newItem.attr("tableName", currentItemData.TableName);
                        if (currentItemData.Label.indexOf("View:") == 0)
                            newItem.addClass("viewAttribute");
                        else
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
                        uuids: ["resItem" + currentConnectionData.SourceId + "RightMiddle"], target: "resItem" + currentConnectionData.TargetId
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
                        if (currentItemData.TypeClass == "circle-thick" || currentItemData.TypeClass.substr(0, 8) == "gateway-" || currentItemData.Condition != null)
                            newItem = $('<img id="wfItem' + currentItemData.Id + '" class="item" symbolType="' + currentItemData.TypeClass +
                            '" src="/Content/images/TapestryIcons/' + currentItemData.TypeClass + '.png" style="left: ' + currentItemData.PositionX + 'px; top: '
                            + currentItemData.PositionY + 'px;" />');
                        else
                            newItem = $('<div id="wfItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                                + currentItemData.PositionY + 'px;"><span class="itemLabel">' + currentItemData.Label + '</span></div>');
                        newItem.addClass(currentItemData.TypeClass);
                        if (currentItemData.ActionId != null)
                            newItem.attr('actionId', currentItemData.ActionId);
                        if (currentItemData.InputVariables != null)
                            newItem.data('inputVariables', currentItemData.InputVariables);
                        if (currentItemData.OutputVariables != null)
                            newItem.data('outputVariables', currentItemData.OutputVariables);
                        if (currentItemData.PageId != null)
                            newItem.attr("pageId", currentItemData.PageId);
                        if (currentItemData.ComponentName != null)
                            newItem.attr('componentName', currentItemData.ComponentName);
                        if (currentItemData.TargetId != null)
                            newItem.attr('targetId', currentItemData.TargetId);
                        if (currentItemData.StateId != null)
                            newItem.attr("stateId", currentItemData.StateId);
                        if (currentItemData.isAjaxAction != null)
                            newItem.data('isAjaxAction', currentItemData.isAjaxAction);
                        if (currentItemData.TypeClass == "circle-thick")
                            newItem.attr("endpoints", "final");
                        if (currentItemData.TypeClass.substr(0, 8) == "gateway-")
                            newItem.attr("endpoints", "gateway");
                        if (currentItemData.Condition != null)
                            newItem.data("condition", currentItemData.Condition);
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneContentArea");
                        targetSwimlane.append(newItem);
                        AddToJsPlumb(newItem);
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
                    sourceId = "wfItem" + currentConnectionData.SourceId;
                    targetId = "wfItem" + currentConnectionData.TargetId;
                    if (currentConnectionData.SourceSlot == 1)
                        sourceEndpointUuid = "BottomCenter";
                    else
                        sourceEndpointUuid = "RightMiddle";
                    currentInstance.connect({ uuids: [sourceId + sourceEndpointUuid], target: targetId });
                }
            }
            RoleWhitelist = data.RoleWhitelist;
            $("#blockHeaderRolesCount").text(RoleWhitelist.length);
            appId = $("#currentAppId").val();
            $.ajax({
                type: "GET",
                url: "/api/database/apps/" + appId + "/commits/latest",
                dataType: "json",
                success: function (tableData) {
                    attributesInToolboxState = data.ToolboxState ? data.ToolboxState.Attributes : [];
                    $(".tapestryToolbox .toolboxLi_Attributes").remove();
                    for (tableIndex = 0; tableIndex < tableData.Tables.length; tableIndex++) {
                        attributeLibId = ++lastLibId;
                        attributeLibraryItem = $('<div libId="' + ++attributeLibId + '" libType="table-attribute" class="libraryItem tableAttribute" tableName="'
                            + tableData.Tables[tableIndex].Name + '">Table: ' + tableData.Tables[tableIndex].Name + '</div>')
                        $("#libraryCategory-Attributes").append(attributeLibraryItem);
                        attributeMatch = attributesInToolboxState.filter(function (value) {
                            return !value.ColumnName && value.TableName == tableData.Tables[tableIndex].Name;
                        }).length;
                        if (attributeMatch) {
                            newToolboxLiAttribute = $('<li libId="' + attributeLibId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + tableData.Tables[tableIndex].Name + '">'
                                + '<span class="itemLabel">Table: ' + tableData.Tables[tableIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLiAttribute);
                            ToolboxItemDraggable(newToolboxLiAttribute);
                            attributeLibraryItem.addClass("highlighted");
                        }
                    }
                    for (viewIndex = 0; viewIndex < tableData.Views.length; viewIndex++) {
                        attributeLibId = ++lastLibId;
                        attributeLibraryItem = $('<div libId="' + ++attributeLibId + '" libType="view-attribute" class="libraryItem viewAttribute" tableName="'
                            + tableData.Views[viewIndex].Name + '">View: ' + tableData.Views[viewIndex].Name + '</div>')
                        $("#libraryCategory-Attributes").append(attributeLibraryItem);
                        attributeMatch = attributesInToolboxState.filter(function (value) {
                            return !value.ColumnName && value.TableName == tableData.Views[viewIndex].Name;
                        }).length;
                        if (attributeMatch) {
                            newToolboxLiAttribute = $('<li libId="' + attributeLibId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem viewAttribute" tableName="' + tableData.Views[viewIndex].Name + '">'
                                + '<span class="itemLabel">View: ' + tableData.Views[viewIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLiAttribute);
                            ToolboxItemDraggable(newToolboxLiAttribute);
                            attributeLibraryItem.addClass("highlighted");
                        }
                    }
                    AssociatedTableIds = data.AssociatedTableIds;
                    AssociatedTableName = data.AssociatedTableName;
                    ModelTableName = data.ModelTableName;
                    $("#blockHeaderDbResCount").text(data.AssociatedTableName.length);
                    somethingWasAdded = false;
                    for (tableIndex = 0; tableIndex < data.AssociatedTableName.length; tableIndex++) {
                        currentTable = tableData.Tables.filter(function (value) {
                            return value.Name == data.AssociatedTableName[tableIndex];
                        })[0];
                        if (currentTable != undefined) {
                            for (columnIndex = 0; columnIndex < currentTable.Columns.length; columnIndex++) {
                                attributeLibId = ++lastLibId;
                                attributeLibraryItem = $('<div libId="' + attributeLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                    + currentTable.Name + '" columnName="' + currentTable.Columns[columnIndex].Name + '">' + currentTable.Name + '.' + currentTable.Columns[columnIndex].Name + '</div>');
                                $("#libraryCategory-Attributes").append(attributeLibraryItem);
                                attributeMatch = attributesInToolboxState.filter(function (value) {
                                    return value.ColumnName == currentTable.Columns[columnIndex].Name && value.TableName == currentTable.Name;
                                }).length;
                                if (attributeMatch) {
                                    newToolboxLiAttribute = $('<li libId="' + attributeLibId + '" class="toolboxLi toolboxLi_Attributes"><div class="toolboxItem attributeItem tableAttribute" tableName="' + currentTable.Name + '" columnName="' + currentTable.Columns[columnIndex].Name + '"><span class="itemLabel">'
                                        + currentTable.Name + '.' + currentTable.Columns[columnIndex].Name + '</span></div></li>');
                                    $(".tapestryToolbox .toolboxCategoryHeader_UI").before(newToolboxLiAttribute);
                                    ToolboxItemDraggable(newToolboxLiAttribute);
                                    attributeLibraryItem.addClass("highlighted");
                                }
                            }
                        }
                        systemTable = SystemTables.filter(function (value) {
                            return value.Name == data.AssociatedTableName[tableIndex];
                        })[0];
                        if (systemTable)
                            for (i = 0; i < systemTable.Columns.length; i++) {
                                $("#libraryCategory-Attributes").append($('<div libId="' + ++lastLibId + '" libType="column-attribute" class="libraryItem columnAttribute" tableName="'
                                    + systemTable.Name + '" columnName="' + systemTable.Columns[i] + '">' + systemTable.Name + '.' + systemTable.Columns[i] + '</div>'));
                            }
                    };
                }
            });
            
            $('.libraryItem').remove();
            $.ajax({
                type: "GET",
                url: "/api/tapestry/actions",
                dataType: "json",
                success: function (actionData) {
                    actionsInToolboxState = data.ToolboxState ? data.ToolboxState.Actions : [];
                    $(".tapestryToolbox .toolboxLi_Actions").remove();
                    for (actionIndex = 0; actionIndex < actionData.Items.length; actionIndex++)
                    {
                        actionLibId = ++lastLibId;
                        actionLibraryItem = $('<div libId="' + actionLibId + '" libType="action" class="libraryItem" actionId="' + actionData.Items[actionIndex].Id + '">' + actionData.Items[actionIndex].Name + '</div>');
                        $('#libraryCategory-Actions').append(actionLibraryItem);
                        actionMatch = actionsInToolboxState.filter(function (value) {
                            return value.ActionId == actionData.Items[actionIndex].Id;
                        }).length;
                        if (actionMatch) {
                            newToolboxLiAction = $('<li libId="' + actionLibId + '" class="toolboxLi toolboxLi_Actions"><div class="toolboxItem actionItem" actionId="' + actionData.Items[actionIndex].Id + '"><span class="itemLabel">'
                            + actionData.Items[actionIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_Attributes").before(newToolboxLiAction);
                            ToolboxItemDraggable(newToolboxLiAction);
                            actionLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-roles/" + appId,
                dataType: "json",
                success: function (roleData) {
                    rolesInToolboxState = data.ToolboxState ? data.ToolboxState.Roles : [];
                    $(".tapestryToolbox .toolboxLi_Roles").remove();
                    for (roleIndex = 0; roleIndex < roleData.Roles.length; roleIndex++) {
                        roleLibId = ++lastLibId;
                        roleLibraryItem = $('<div libId="' + roleLibId + '" libType="role" class="libraryItem">' + roleData.Roles[roleIndex].Name + '</div>');
                        $('#libraryCategory-Roles').append(roleLibraryItem);
                        roleMatch = rolesInToolboxState.filter(function (value) {
                            return value.Label == roleData.Roles[roleIndex].Name;
                        }).length;
                        if (roleMatch) {
                            newToolboxLiRole = $('<li libId="' + roleLibId + '" class="toolboxLi toolboxLi_Roles"><div class="toolboxItem roleItem"><span class="itemLabel">'
                            + roleData.Roles[roleIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_States").before(newToolboxLiRole);
                            ToolboxItemDraggable(newToolboxLiRole);
                            roleLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });
            $.ajax({
                type: "GET",
                url: "/api/Persona/app-states/" + appId,
                dataType: "json",
                success: function (stateData) {
                    statesInToolboxState = data.ToolboxState ? data.ToolboxState.States : [];
                    $(".tapestryToolbox .toolboxLi_States").remove();
                    for (stateIndex = 0; stateIndex < stateData.States.length; stateIndex++) {
                        stateLibId = ++lastLibId;
                        stateLibraryItem = $('<div libId="' + stateLibId + '" libType="state" class="libraryItem" stateId="' + stateData.States[stateIndex].Id + '">' + stateData.States[stateIndex].Name + '</div>');
                        $('#libraryCategory-States').append(stateLibraryItem);
                        stateMatch = statesInToolboxState.filter(function (value) {
                            return value.StateId == stateData.States[stateIndex].Id;
                        }).length;
                        if (stateMatch) {
                            newToolboxLiState = $('<li libId="' + stateLibId + '" class="toolboxLi toolboxLi_States"><div class="toolboxItem stateItem" stateId="' + stateData.States[stateIndex].Id + '"><span class="itemLabel">'
                            + stateData.States[stateIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_Targets").before(newToolboxLiState);
                            ToolboxItemDraggable(newToolboxLiState);
                            stateLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });
            $.ajax({
                type: "GET",
                url: "/api/tapestry/apps/" + appId + "/blocks",
                dataType: "json",
                success: function (targetData) {
                    targetsInToolboxState = data.ToolboxState ? data.ToolboxState.Targets : [];
                    $(".tapestryToolbox .toolboxLi_Targets").remove();
                    for (targetIndex = 0; targetIndex < targetData.ListItems.length; targetIndex++) {
                        targetLibId = ++lastLibId;
                        targetLibraryItem = $('<div libId="' + targetLibId + '" libType="target" class="libraryItem" targetId="' + targetData.ListItems[targetIndex].Id + '">' + targetData.ListItems[targetIndex].Name + '</div>');
                        $('#libraryCategory-Targets').append(targetLibraryItem);
                        targetMatch = targetsInToolboxState.filter(function (value) {
                            return value.TargetId == targetData.ListItems[targetIndex].Id;
                        }).length;
                        if (targetMatch) {
                            newToolboxLiTarget = $('<li libId="' + targetLibId + '" class="toolboxLi toolboxLi_Targets"><div class="toolboxItem targetItem" targetId="' + targetData.ListItems[targetIndex].Id + '"><span class="itemLabel">'
                            + targetData.ListItems[targetIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_Templates").before(newToolboxLiTarget);
                            ToolboxItemDraggable(newToolboxLiTarget);
                            targetLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });
            $.ajax({
                type: "GET",
                url: "/api/hermes/" + appId + "/templates",
                dataType: "json",
                success: function (templateData) {
                    templatesInToolboxState = data.ToolboxState ? data.ToolboxState.Templates : [];
                    $(".tapestryToolbox .toolboxLi_Templates").remove();
                    for (templateIndex = 0; templateIndex < templateData.length; templateIndex++) {
                        templateLibId = ++lastLibId;
                        templateLibraryItem = $('<div libId="' + templateLibId + '" libType="template" class="libraryItem">' + templateData[templateIndex].Name + '</div>');
                        $('#libraryCategory-Templates').append(templateLibraryItem);
                        templateMatch = templatesInToolboxState.filter(function (value) {
                            return value.Label == templateData[templateIndex].Name;
                        }).length;
                        if (templateMatch) {
                            newToolboxLiIntegration = $('<li libId="' + templateLibId + '" class="toolboxLi toolboxLi_Templates"><div class="toolboxItem templateItem"><span class="itemLabel">'
                                + templateData[templateIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox .toolboxCategoryHeader_Integrations").before(newToolboxLiIntegration);
                            ToolboxItemDraggable(newToolboxLiIntegration);
                            templateLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });
            $.ajax({
                type: "GET",
                url: "/api/nexus/" + appId + "/gateways",
                dataType: "json",
                success: function (integrationData) {
                    integrationsInToolboxState = data.ToolboxState ? data.ToolboxState.Integrations : [];
                    $(".tapestryToolbox .toolboxLi_Integrations").remove();
                    for (integrationIndex = 0; integrationIndex < integrationData.Ldap.length; integrationIndex++) {
                        integrationLibId = ++lastLibId;
                        integrationLibraryItem = $('<div libId="' + integrationLibId + '" libType="ldap" class="libraryItem">LDAP: ' + integrationData.Ldap[integrationIndex].Name + '</div>');
                        $('#libraryCategory-Integration').append(integrationLibraryItem);
                        integrationMatch = integrationsInToolboxState.filter(function (value) {
                            return value.Label == "LDAP: " + integrationData.Ldap[integrationIndex].Name;
                        }).length;
                        if (integrationMatch) {
                            newToolboxLiIntegration = $('<li libId="' + integrationLibId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                                + 'LDAP: ' + integrationData.Ldap[integrationIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox").append(newToolboxLiIntegration);
                            ToolboxItemDraggable(newToolboxLiIntegration);
                            integrationLibraryItem.addClass("highlighted");
                        }
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.WS.length; integrationIndex++) {
                        integrationLibId = ++lastLibId;
                        integrationLibraryItem = $('<div libId="' + integrationLibId + '" libType="ws" libSubType="' + integrationData.WS[integrationIndex].Type + '" class="libraryItem">WS: ' + integrationData.WS[integrationIndex].Name + '</div>');
                        $('#libraryCategory-Integration').append(integrationLibraryItem);
                        integrationMatch = integrationsInToolboxState.filter(function (value) {
                            return value.Label == "WS: " + integrationData.WS[integrationIndex].Name;
                        }).length;
                        if (integrationMatch) {
                            newToolboxLiIntegration = $('<li libId="' + integrationLibId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                                + 'WS: ' + integrationData.WS[integrationIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox").append(newToolboxLiIntegration);
                            ToolboxItemDraggable(newToolboxLiIntegration);
                            integrationLibraryItem.addClass("highlighted");
                        }
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.SMTP.length; integrationIndex++) {
                        integrationLibId = ++lastLibId;
                        integrationLibraryItem = $('<div libId="' + integrationLibId + '" libType="smtp" class="libraryItem">SMTP: ' + integrationData.SMTP[integrationIndex].Name + '</div>');
                        $('#libraryCategory-Integration').append(integrationLibraryItem);
                        integrationMatch = integrationsInToolboxState.filter(function (value) {
                            return value.Label == "SMTP: " + integrationData.SMTP[integrationIndex].Name;
                        }).length;
                        if (integrationMatch) {
                            newToolboxLiIntegration = $('<li libId="' + integrationLibId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                                + 'SMTP: ' + integrationData.SMTP[integrationIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox").append(newToolboxLiIntegration);
                            ToolboxItemDraggable(newToolboxLiIntegration);
                            integrationLibraryItem.addClass("highlighted");
                        }
                    }
                    for (integrationIndex = 0; integrationIndex < integrationData.WebDAV.length; integrationIndex++) {
                        integrationLibId = ++lastLibId;
                        integrationLibraryItem = $('<div libId="' + integrationLibId + '" libType="smtp" class="libraryItem">WebDAV: ' + integrationData.WebDAV[integrationIndex].Name + '</div>');
                        $('#libraryCategory-Integration').append(integrationLibraryItem);
                        integrationMatch = integrationsInToolboxState.filter(function (value) {
                            return value.Label == "WebDAV: " + integrationData.WebDAV[integrationIndex].Name;
                        }).length;
                        if (integrationMatch) {
                            newToolboxLiIntegration = $('<li libId="' + integrationLibId + '" class="toolboxLi toolboxLi_Integrations"><div class="toolboxItem integrationItem"><span class="itemLabel">'
                                + 'WebDAV: ' + integrationData.WebDAV[integrationIndex].Name + '</span></div></li>');
                            $(".tapestryToolbox").append(newToolboxLiIntegration);
                            ToolboxItemDraggable(newToolboxLiIntegration);
                            integrationLibraryItem.addClass("highlighted");
                        }
                    }
                }
            });

            AssociatedPageIds = data.AssociatedPageIds;
            $("#blockHeaderScreenCount").text(data.AssociatedPageIds.length);
            uicInToolboxState = data.ToolboxState ? data.ToolboxState.UiComponents : [];
            $(".tapestryToolbox .toolboxLi_UI").remove();

            for (pageIndex = 0; pageIndex < data.AssociatedPageIds.length; pageIndex++) {
                pageId = data.AssociatedPageIds[pageIndex];
                $.ajax({
                    type: "GET",
                    url: "/api/mozaic-editor/apps/" + appId + "/pages/" + pageId,
                    dataType: "json",
                    success: function (uiPageData) {
                        for (componentIndex = 0; componentIndex < uiPageData.Components.length; componentIndex++) {
                            if (componentIndex == 0) {
                                uicLibId = ++lastLibId;
                                uicLibraryItem = $('<div libId="' + uicLibId + '" pageId="' + uiPageData.Id + '" libType="ui" class="libraryItem">Screen: '
                                    + uiPageData.Name + '</div>');
                                $('#libraryCategory-UI').append(uicLibraryItem);
                                uicMatch = uicInToolboxState.filter(function (value) {
                                    return value.PageId == uiPageData.Id && (!value.ComponentName || value.ComponentName == "undefined");
                                }).length;
                                if (uicMatch) {
                                    newToolboxLiUic = $('<li libId="' + uicLibId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem pageUi" pageId="' + uiPageData.Id + '">'
                                        + '<span class="itemLabel">Screen: ' + uiPageData.Name + '</span></div></li>');
                                    $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLiUic);
                                    ToolboxItemDraggable(newToolboxLiUic);
                                    uicLibraryItem.addClass("highlighted");
                                }
                            }

                            uicLibId = ++lastLibId;
                            uicLibraryItem = $('<div libId="' + uicLibId + '" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].Name + '" libType="ui" class="libraryItem">'
                            + uiPageData.Components[componentIndex].Name + '</div>');
                            $('#libraryCategory-UI').append(uicLibraryItem);
                            uicMatch = uicInToolboxState.filter(function (value) {
                                return value.PageId == uiPageData.Id && value.ComponentName == uiPageData.Components[componentIndex].Name;
                            }).length;
                            if (uicMatch) {
                                newToolboxLiUic = $('<li libId="' + uicLibId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].Name + '"><span class="itemLabel">'
                                    + uiPageData.Components[componentIndex].Name + '</span></div></li>');
                                $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLiUic);
                                ToolboxItemDraggable(newToolboxLiUic);
                                uicLibraryItem.addClass("highlighted");
                            }
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
                                    uicLibId = ++lastLibId;
                                    uicLibraryItem = $('<div libId="' + uicLibId + '" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '" libType="ui" class="libraryItem">'
                                    + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '</div>');
                                    $('#libraryCategory-UI').append(uicLibraryItem);
                                    uicMatch = uicInToolboxState.filter(function (value) {
                                        return value.PageId == uiPageData.Id && value.ComponentName == uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name;
                                    }).length;
                                    if (uicMatch) {
                                        newToolboxLiUic = $('<li libId="' + uicLibId + '" class="toolboxLi toolboxLi_UI"><div class="toolboxItem uiItem" pageId="' + uiPageData.Id + '" componentName="' + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '"><span class="itemLabel">'
                                            + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '</span></div></li>');
                                        $(".tapestryToolbox .toolboxCategoryHeader_Roles").before(newToolboxLiUic);
                                        ToolboxItemDraggable(newToolboxLiUic);
                                        uicLibraryItem.addClass("highlighted");
                                    }
                                    if (uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Type == "data-table-with-actions") {
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_edit" libType="ui" class="libraryItem">'
                                            + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '_EditAction</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_detail" libType="ui" class="libraryItem">'
                                            + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '_DetailsAction</div>');
                                        $("#libraryCategory-UI").append('<div libId="' + ++lastLibId + '" pageId="' + uiPageData.Id + '" componentName="datatable_delete" libType="ui" class="libraryItem">' + uiPageData.Components[componentIndex].ChildComponents[childComponentIndex].Name + '_DeleteAction</div>');
                                    }
                                }
                            }
                        }
                    },
                });
            }
            $(document).one("ajaxStop", function () {
                pageSpinner.hide();
            });
        }
    });
};
