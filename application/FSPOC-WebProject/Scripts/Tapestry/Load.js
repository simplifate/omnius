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
            $("#assignResourcesPanel .resourceRule").remove();
            $("#workflowRulesPanel .workflowRule").remove();
            for (i = 0; i < data.ResourceRules.length; i++) {
                currentRuleData = data.ResourceRules[i];
                newRule = $('<div class="ruleItem resourceRule" style="width: '+currentRuleData.Width+'px; height: '+currentRuleData.Height+'px; left: '
                    +currentRuleData.PositionX+'px; top: '+currentRuleData.PositionY+'px;"></div>');
                $("#assignResourcesPanel .scrollArea").append(newRule);
                newRule.draggable({ containment: "#assignResourcesPanel" });
                newRule.resizable();
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
                        droppedElement.draggable({ containment: "parent" });
                        ui.helper.remove();
                    }
                });
                for (j = 0; j < currentRuleData.ResourceItems.length; j++) {
                    currentItemData = currentRuleData.ResourceItems[j];
                    newItem = $('<div id="resItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                        + currentItemData.PositionY + 'px;">'
                        + currentItemData.Label + '</div>');
                    newItem.addClass(currentItemData.TypeClass);
                    newRule.append(newItem);
                    newItem.draggable({
                        containment: "parent",
                        drag: function () {
                            instance = $(this).parents(".resourceRule").data("jsPlumbInstance");
                            instance.recalculateOffsets();
                            instance.repaintEverything();
                        }
                    });
                }
                currentInstance = newRule.data("jsPlumbInstance");
                for (j = 0; j < currentRuleData.Connections.length; j++) {
                    currentConnectionData = currentRuleData.Connections[j];
                    currentInstance.connect({
                        source: "resItem" + currentConnectionData.Source, target: "resItem" + currentConnectionData.Target,
                        anchors: ["Continuous", "Continuous"], editable: false, connector: "Straight",
                        paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' }}).removeAllOverlays();
                }
            }
            for (i = 0; i < data.WorkflowRules.length; i++) {
                currentRuleData = data.WorkflowRules[i];
                newRule = $('<div id="wfRule1" class="ruleItem workflowRule" style="width: ' + currentRuleData.Width + 'px; height: ' + currentRuleData.Height
                    + 'px; left: ' + currentRuleData.PositionX + 'px; top: ' + currentRuleData.PositionY + 'px;"><div class="workflowRuleHeader">'
                    + '<div class="vericalLabel">' + currentRuleData.Name + '</div></div><div class="swimlaneArea"><div class="swimlane">'
                    + '<div class="swimlaneRolesArea"><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div>'
                + '<div class="swimlane"><div class="swimlaneRolesArea"><div class="rolePlaceholder"><div class="rolePlaceholderLabel">Pokud chcete specifikovat roli<br />'
                + 'přetáhněte ji do této oblasti</div></div></div><div class="swimlaneContentArea"></div></div></div></div>');
                $("#workflowRulesPanel").append(newRule);
                newRule.draggable({ containment: "#workflowRulesPanel" });
                newRule.resizable();
                CreateJsPlumbInstanceForRule(newRule);
                newRule.find(".swimlaneRolesArea").droppable({
                    containment: ".swimlaneContentArea",
                    tolerance: "touch",
                    accept: ".toolboxItem.roleItem",
                    greedy: true,
                    drop: function (e, ui) {
                        droppedElement = ui.helper.clone();
                        $(this).find(".rolePlaceholder, .roleItem").remove();
                        $(this).append($('<div class="roleItem">' + droppedElement.text() + '</div>'));
                        ui.helper.remove();
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
                        droppedElement.draggable({ containment: "parent" });
                        ui.helper.remove();
                    }
                });
                for (j = 0; j < currentRuleData.Swimlanes.length; j++) {
                    currentSwimlaneData = currentRuleData.Swimlanes[j];
                    if (currentSwimlaneData.Roles.length > 0) {
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneRolesArea");
                        targetSwimlane.find(".rolePlaceholder, .roleItem").remove();
                        targetSwimlane.append($('<div class="roleItem">' + currentSwimlaneData.Roles[0] + '</div>'));
                    }
                    for (k = 0; k < currentSwimlaneData.WorkflowItems.length; k++) {
                        currentItemData = currentSwimlaneData.WorkflowItems[k];
                        newItem = $('<div id="wfItem' + currentItemData.Id + '" class="item" style="left: ' + currentItemData.PositionX + 'px; top: '
                            + currentItemData.PositionY + 'px;">' + currentItemData.Label + '</div>');
                        newItem.addClass(currentItemData.TypeClass);
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneContentArea");
                        targetSwimlane.append(newItem);
                        newItem.draggable({
                            containment: "parent",
                            drag: function () {
                                instance = $(this).parents(".workflowRule").data("jsPlumbInstance");
                                instance.recalculateOffsets();
                                instance.repaintEverything();
                            }
                        });
                    }
                    for (k = 0; k < currentSwimlaneData.WorkflowSymbols.length; k++) {
                        currentSymbolData = currentSwimlaneData.WorkflowSymbols[k];
                        newSymbol = $('<img id="wfSymbol' + currentSymbolData.Id + '" class="symbol" symbolType="' + currentSymbolData.Type +
                            '" src="/Content/images/TapestryIcons/' + currentSymbolData.Type + '.png" style="left: ' + currentSymbolData.PositionX + 'px; top: '
                            + currentSymbolData.PositionY + 'px;" />');
                        targetSwimlane = newRule.find(".swimlane").eq(currentSwimlaneData.SwimlaneIndex).find(".swimlaneContentArea");
                        targetSwimlane.append(newSymbol);
                        newSymbol.draggable({
                            containment: "parent",
                            drag: function () {
                                instance = $(this).parents(".workflowRule").data("jsPlumbInstance");
                                instance.recalculateOffsets();
                                instance.repaintEverything();
                            }
                        });
                    }
                }
                currentInstance = newRule.data("jsPlumbInstance");
                for (j = 0; j < currentRuleData.Connections.length; j++) {
                    currentConnectionData = currentRuleData.Connections[j];
                    sourceId = (currentConnectionData.SourceType == 1 ? "wfSymbol" : "wfItem") + currentConnectionData.Source;
                    targetId = (currentConnectionData.TargetType == 1 ? "wfSymbol" : "wfItem") + currentConnectionData.Target;
                    currentInstance.connect({
                        source: sourceId, target: targetId, anchors: ["Continuous", "Continuous"], editable: false, connector: "Straight",
                        paintStyle: { lineWidth: 2, strokeStyle: '#54c6f0' } });
                }
            }
        }
    });
};
