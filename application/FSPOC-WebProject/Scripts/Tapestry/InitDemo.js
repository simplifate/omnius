$(function () {
    if ($("body.tapestryModule").length) {
        $(".rule").resizable({
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
                $(this).css("min-width", contentsWidth + 0);
                $(this).css("min-height", contentsHeight + 20);

                verticalLimit = 1000000;
                horizontalLimit = 1000000;

                ruleLeft = $(this).position().left;
                ruleRight = ruleLeft + $(this).width();
                ruleTop = $(this).position().top;
                ruleBottom = ruleTop + $(this).height();

                $("#rulesPanel .rule").each(function (index, element) {
                    otherRule = $(element);
                    otherRuleLeft = otherRule.position().left;
                    otherRuleRight = otherRuleLeft + otherRule.width();
                    otherRuleTop = otherRule.position().top;
                    otherRuleBottom = otherRuleTop + otherRule.height();

                    if (otherRuleLeft < ruleRight && otherRuleRight > ruleLeft
                        && otherRuleTop - 30 > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                        verticalLimit = otherRuleTop - ruleTop;
                    if (otherRuleTop - 30 < ruleBottom && otherRuleBottom > ruleTop - 30
                        && otherRuleLeft > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                        horizontalLimit = otherRuleLeft - ruleLeft;
                });
                $(this).css("max-width", horizontalLimit - 50);
                $(this).css("max-height", verticalLimit - 50);
            }
        });
        $(".rule").draggable({
            handle: ".ruleHeader",
            revert: "invalid",
            stop: function () {
                $(this).draggable("option", "revert", "invalid");
            }
        });
        $(".rule").droppable({
            containment: ".rule",
            greedy: true,
            tolerance: "touch",
            accept: ".menuItem, .rule",
            drop: function (e, ui) {
                if (ui.helper.hasClass("rule")) {
                    ui.draggable.draggable("option", "revert", true);
                    return false;
                }
                droppedElement = ui.helper.clone();
                ui.helper.remove();
                ruleContent = $(this).find(".ruleContent");
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
                }
                else {
                    droppedElement.removeClass("menuItem");
                    droppedElement.addClass("item");
                    AddIconToItem(droppedElement);
                    droppedElement.offset({ left: droppedElement.offset().left + leftOffset + 8, top: droppedElement.offset().top + topOffset + 8 });
                    AddToJsPlumb($(this).data("jsPlumbInstance"), droppedElement);
                }
            }
        });
        $(".rule .editRuleIcon").on("click", function () {
            currentRule = $(this).parents(".rule");
            renameRuleDialog.dialog("open");
        });
        $(".rule .deleteRuleIcon").on("click", function () {
            $(this).parents(".rule").remove();
        });
    }
});