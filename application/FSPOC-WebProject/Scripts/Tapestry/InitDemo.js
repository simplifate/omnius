$(function () {
    if (CurrentModuleIs("tapestryModule")) {

        // Resource rules
        $("#resourceRulesPanel  .resourceRule").draggable({
            containment: "parent",
            revert: function (event, ui) {
                return ($(this).collision("#resourceRulesPanel .resourceRule").length > 1);
            }
        });
        $("#resourceRulesPanel .resourceRule").droppable({
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
            }
        });
        $("#resourceRulesPanel .resourceRule").resizable({
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
            }
        });

        // Workflow rules
        $("#workflowRulesPanel .workflowRule").draggable({
            containment: "parent",
            handle: ".workflowRuleHeader",
            revert: function (event, ui) {
                return ($(this).collision("#workflowRulesPanel .workflowRule").length > 1);
            }
        });
        $(".swimlaneRolesArea").droppable({
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
        $(".swimlaneContentArea").droppable({
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
            }
        });
        $("#workflowRulesPanel .workflowRule").resizable({
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
            }
        });
    }
});
