function LoadBlock(commitId) {
    // TODO: replace hardcoded IDs with real app/block IDs
    if (commitId)
        url = "/api/tapestry/apps/1/blocks/30/commits/" + commitId;
    else
        url = "/api/tapestry/apps/1/blocks/30";
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        error: function () { alert("ERROR") },
        success: function (data) {
            $("#headerBlockName").text(data.Name),
            $("#headerTableName").text(data.AssociatedTableName),
            $("#rulesPanel .rule").remove();
            for (i = 0; i < data.Rules.length; i++) {
                currentRuleData = data.Rules[i];
                newRule = $('<div class="rule" style="left: ' + currentRuleData.PositionX + 'px; top: ' + currentRuleData.PositionY + 'px; '
                   + 'width: ' + currentRuleData.Width + 'px; height: ' + currentRuleData.Height + 'px;"><div class="ruleHeader">'
                   + currentRuleData.Name + '</div><div class="editRuleIcon fa fa-edit"></div>'
                   + '<div class="deleteRuleIcon fa fa-remove"></div><div class="ruleContent"></div></div>');
                $("#rulesPanel .scrollArea").append(newRule);
                currentInstance = CreateJsPlumbInstanceForRule(newRule);
                for (j = 0; j < currentRuleData.Items.length; j++) {
                    currentItemData = currentRuleData.Items[j];
                    newItem = $('<div class="item" style="left: '
                        + currentItemData.PositionX + 'px; top: ' + currentItemData.PositionY + 'px;">'
                        + currentItemData.Label + '</div>');
                    newItem.addClass(currentItemData.TypeClass);
                    if (currentItemData.IsDataSource)
                        newItem.addClass("dataSource");
                    newItem.attr("saveId", currentItemData.Id);
                    newItem.attr("dialogType", currentItemData.DialogType);
                    newRule.find(".ruleContent").append(newItem);
                    AddIconToItem(newItem);
                    AddToJsPlumb(currentInstance, newItem);
                    newItem.droppable({
                        greedy: true,
                        tolerance: "touch",
                        accept: ".item, .operatorSymbol",
                        drop: function (event, ui) {
                            ui.draggable.draggable("option", "revert", true);
                            revertActive = true;
                        }
                    });
                }
                for (j = 0; j < currentRuleData.Operators.length; j++) {
                    currentOperatorData = currentRuleData.Operators[j];
                    if (currentOperatorData.Type == "decision")
                        newOperator = $('<div class="decisionRhombus operatorSymbol"><svg width="70" height="60">'
                          + '<polygon points="35,8 67,30 35,52 3,30" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /></svg></div>');
                    else if (currentOperatorData.Type == "condition")
                        newOperator = $('<div class="conditionEllipse operatorSymbol"><svg width="70" height="60">'
                          + '<ellipse cx="35" cy="30" rx="32" ry="20" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /><text x="17" y="39" fill="#2ddef9" font-size="25">if...</text></svg></div>');
                    newRule.find(".ruleContent").append(newOperator);
                    newOperator.css("left", currentOperatorData.PositionX);
                    newOperator.css("top", currentOperatorData.PositionY);
                    newOperator.attr("saveId", currentOperatorData.Id);
                    newOperator.attr("dialogType", currentOperatorData.DialogType);
                    AddToJsPlumb(currentInstance, newOperator);
                }
                for (j = 0; j < currentRuleData.Connections.length; j++) {
                    currentConnectionData = currentRuleData.Connections[j];
                    sourceDiv = newRule.find(".item[saveId='" + currentConnectionData.Source + "'], .operatorSymbol[saveId='" + currentConnectionData.Source + "']");
                    targetDiv = newRule.find(".item[saveId='" + currentConnectionData.Target + "'], .operatorSymbol[saveId='" + currentConnectionData.Target + "']");
                    if (currentConnectionData.SourceSlot == 1)
                        sourceEndpointUuid = "BottomCenter";
                    else
                        sourceEndpointUuid = "RightMiddle";
                    currentInstance.connect({ uuids: [sourceDiv.attr("id") + sourceEndpointUuid], target: targetDiv.attr("id"), editable: true });
                }
                newRule.find(".editRuleIcon").on("click", function () {
                    currentRule = $(this).parents(".rule");
                    renameRuleDialog.dialog("open");
                });
                newRule.find(".deleteRuleIcon").on("click", function () {
                    $(this).parents(".rule").remove();
                });
                newRule.resizable({
                    start: function (event, ui) {
                        contentsWidth = 120;
                        contentsHeight = 40;
                        $(this).find(".item, .operatorSymbol").each(function (index, element) {
                            rightEdge = $(element).position().left + $(element).width();
                            if (rightEdge > contentsWidth)
                                contentsWidth = rightEdge;
                            bottomEdge = $(element).position().top + $(element).height();
                            if (bottomEdge > contentsHeight)
                                contentsHeight = bottomEdge;
                        });
                        $(this).css("min-width", contentsWidth - 10);
                        $(this).css("min-height", contentsHeight + 20);

                        limits = CheckRuleResizeLimits($(this));
                        $(this).css("max-width", limits.horizontal - 50);
                        $(this).css("max-height", limits.vertical - 50);
                    },
                    resize: function (event, ui) {
                        limits = CheckRuleResizeLimits($(this));
                        $(this).css("max-width", limits.horizontal - 50);
                        $(this).css("max-height", limits.vertical - 50);
                    }
                });
                newRule.draggable({ handle: ".ruleHeader" });
                newRule.attr("id", AssingID());
                newRule.droppable({
                    containment: ".rule",
                    greedy: false,
                    tolerance: "touch",
                    accept: ".item, .operatorSymbol, .menuItem, .rule",
                    drop: function (e, ui) {
                        if (ui.helper.hasClass("item") || ui.helper.hasClass("operatorSymbol")) {
                            return false;
                        }
                        if (ui.helper.hasClass("rule")) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        }
                        if (ui.helper.collision(".item, .operatorSymbol").length > 0) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        };
                        ruleContent = $(this).find(".ruleContent");
                        if (ui.offset.left < ruleContent.offset().left || ui.offset.top < ruleContent.offset().top
                            || ui.offset.left + ui.helper.width() > ruleContent.offset().left + ruleContent.width() - 20
                            || ui.offset.top + ui.helper.height() > ruleContent.offset().top + ruleContent.height() - 20) {
                            ui.draggable.draggable("option", "revert", true);
                            return false;
                        }
                        droppedElement = ui.helper.clone();
                        ui.helper.remove();
                        droppedElement.appendTo(ruleContent);
                        leftOffset = ui.draggable.parent().offset().left - ruleContent.offset().left;
                        topOffset = ui.draggable.parent().offset().top - ruleContent.offset().top;
                        if (droppedElement.hasClass("operator")) {
                            if (droppedElement.attr("operatorType") == "decision")
                                newOperator = $('<div class="decisionRhombus operatorSymbol"><svg width="70" height="60">'
                                  + '<polygon points="35,8 67,30 35,52 3,30" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /></svg></div>');
                            else if (droppedElement.attr("operatorType") == "condition")
                                newOperator = $('<div class="conditionEllipse operatorSymbol"><svg width="70" height="60">'
                                  + '<ellipse cx="35" cy="30" rx="32" ry="20" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /><text x="17" y="39" fill="#2ddef9" font-size="25">if...</text></svg></div>');
                            newOperator.appendTo(ruleContent);
                            newOperator.offset({ left: droppedElement.offset().left + leftOffset + 8, top: droppedElement.offset().top + topOffset + 8 });
                            newOperator.attr("dialogType", droppedElement.attr("dialogType"));
                            droppedElement.remove();
                            AddToJsPlumb($(this).data("jsPlumbInstance"), newOperator);
                            newOperator.droppable({
                                greedy: true,
                                tolerance: "touch",
                                accept: ".item, .operatorSymbol",
                                drop: function (event, ui) {
                                    ui.draggable.draggable("option", "revert", true);
                                    revertActive = true;
                                }
                            });
                        }
                        else {
                            droppedElement.removeClass("menuItem");
                            droppedElement.addClass("item");
                            droppedElement.offset({ left: droppedElement.offset().left + leftOffset + 8, top: droppedElement.offset().top + topOffset + 8 });
                            AddIconToItem(droppedElement);
                            if (droppedElement.position().left + droppedElement.width() > ruleContent.width() - 25)
                                droppedElement.css("left", ruleContent.width() - droppedElement.width() - 25);
                            AddToJsPlumb($(this).data("jsPlumbInstance"), droppedElement);
                            droppedElement.droppable({
                                greedy: true,
                                tolerance: "touch",
                                accept: ".item, .operatorSymbol",
                                drop: function (event, ui) {
                                    ui.draggable.draggable("option", "revert", true);
                                    revertActive = true;
                                }
                            });
                        }
                    }
                });
            }
        }
    });
};
