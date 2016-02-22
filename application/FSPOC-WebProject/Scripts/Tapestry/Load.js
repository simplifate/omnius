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
        error: function () { alert("ERROR") },
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
                        ChangedSinceLastSave = true;
                    }
                });
                for (j = 0; j < currentRuleData.ResourceItems.length; j++) {
                    currentItemData = currentRuleData.ResourceItems[j];
                    newItem = $('<div id="resItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                        + currentItemData.PositionY + 'px;">'
                        + currentItemData.Label + '</div>');
                    if (currentItemData.PageId != null)
                        newItem.attr("pageId", currentItemData.PageId);
                    if (currentItemData.ComponentId != null)
                        newItem.attr("componentId", currentItemData.ComponentId);
                    if (currentItemData.TableId != null) {
                        newItem.data("columnFilter", currentItemData.ColumnFilter);
                        newItem.attr("tableId", currentItemData.TableId);
                        newItem.addClass("tableAttribute");
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
                    newSwimlane = $('<div class="swimlane" style="height: ' + (100/currentRuleData.Swimlanes.length) + '%;"><div class="swimlaneRolesArea"><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                        + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>');
                    newRule.find(".swimlaneArea").append(newSwimlane);
                    if (currentSwimlaneData.Roles.length > 0) {
                        newSwimlane.find(".rolePlaceholder").remove();
                        newSwimlane.find(".swimlaneRolesArea").append($('<div class="roleItem">' + currentSwimlaneData.Roles[0] + '</div>'));
                    }
                    for (k = 0; k < currentSwimlaneData.WorkflowItems.length; k++) {
                        currentItemData = currentSwimlaneData.WorkflowItems[k];
                        newItem = $('<div id="wfItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                            + currentItemData.PositionY + 'px;"><span class="itemLabel">' + currentItemData.Label + '</span></div>');
                        newItem.addClass(currentItemData.TypeClass);
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
                        droppedElement = ui.helper.clone();
                        $(this).find(".rolePlaceholder, .roleItem").remove();
                        $(this).append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                        ui.helper.remove();
                        ChangedSinceLastSave = true;
                    }
                });
                newRule.find(".swimlaneContentArea").droppable({
                    containment: ".swimlaneContentArea",
                    tolerance: "touch",
                    accept: ".toolboxSymbol, .toolboxItem",
                    greedy: false,
                    drop: function (e, ui) {
                        droppedElement = ui.helper.clone();
                        if (droppedElement.hasClass("roleItem")) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        }
                        $(this).append(droppedElement);
                        ruleContent = $(this);
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
                        ChangedSinceLastSave = true;
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
            url = "/api/database/apps/" + appId + "/commits/latest",
            $.ajax({
                type: "GET",
                url: url,
                dataType: "json",
                success: function (data) {
                    for (i = 0; i < data.Tables.length; i++) {
                        lastLibId++;
                        newLibItem = $('<div libId="' + lastLibId + '" libType="table-attribute" class="libraryItem" tableId="' + data.Tables[i].Id
                            + '">Table: ' + data.Tables[i].Name + '</div>');
                        $("#libraryCategory-Attributes").append(newLibItem);
                    }
                    $("#blockHeaderDbResCount").text(data.Tables.length);
                }
            });

            url = "api/Persona/app-roles/" + appId;
            $.ajax({
                type: "GET",
                url: url,
                dataType: "json",
                success: function (data) {
                    a = b;
                }
            });

            $("#blockHeaderScreenCount").text(data.AssociatedPageIds.length);
            for (i = 0; i < data.AssociatedPageIds.length; i++) {
                pageId = data.AssociatedPageIds[i];
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
        }
    });
};
