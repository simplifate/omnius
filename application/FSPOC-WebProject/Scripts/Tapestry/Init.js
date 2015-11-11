var ZoomFactor = 1.0;
$(function () {
    if ($("body.tapestryModule").length) {
        $("#headerBlockName").on("click", function () {
            renameBlockDialog.dialog("open");
        });
        $("#headerTableName").on("click", function () {
            chooseTableDialog.dialog("open");
        });
        $("#headerOverview").on("click", function () {
            window.location.href = "/overview";
        });
        $("#btnAddActions").on("click", function () {
            addActionsDialog.dialog("open");
        });
        $("#btnAddRule").on("click", function () {
            lowestRuleBottom = 0;
            highestRuleNumber = 0;
            $("#rulesPanel .rule").each(function (index, element) {
                bottom = $(element).position().top + $(element).height();
                if (bottom > lowestRuleBottom)
                    lowestRuleBottom = bottom;
                name = $(element).find(".ruleHeader").text();
                if (name.startsWith("Rule") && !isNaN(name.substring(4, name.length))) {
                    ruleNumber = parseInt(name.substring(4, name.length));
                    if (ruleNumber > highestRuleNumber)
                        highestRuleNumber = ruleNumber;
                }
            });
            newRule = $('<div class="rule"><div class="ruleHeader">Rule' + (highestRuleNumber + 1) + '</div>'
                + '<div class="editRuleIcon fa fa-edit"></div>'
                + '<div class="deleteRuleIcon fa fa-remove"></div><div class="ruleContent"</div></div>');
            $("#rulesPanel .scrollArea").append(newRule);
            newRule.css("left", 25);
            newRule.css("top", lowestRuleBottom + 60);
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
                            && otherRuleTop > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                            verticalLimit = otherRuleTop - ruleTop;
                        if (otherRuleTop < ruleBottom && otherRuleBottom > ruleTop
                            && otherRuleLeft > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                            horizontalLimit = otherRuleLeft - ruleLeft;
                    });
                    $(this).css("max-width", horizontalLimit - 50);
                    $(this).css("max-height", verticalLimit - 50);
                }
            });
            newRule.draggable({ handle: ".ruleHeader" });
            newRule.attr("id") = AssingID();
            CreateJsPlumbInstanceForRule(newRule);
            newRule.droppable({
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
        });
        $(".scrollArea").droppable({
            tolerance: "fit",
            accept: ".rule"
        });
        $("#upperVerticalDivider").draggable({
            axis: "x",
            drag: function (event, ui) {
                shift = ui.position.left - 722;
                $("#attributesPanel").width(676 + shift);
                $("#rolesPanel").css("left", 728 + shift);
            }
        });
        $("#lowerLeftVerticalDivider").draggable({
            axis: "x",
            drag: function (event, ui) {
                shift = ui.position.left - 722;
                dividerDistance = parseInt($("#lowerRightVerticalDivider").css("left")) - ui.position.left;
                $("#viewsPanel").width(676 + shift);
                $("#statesPanel").width(dividerDistance - 52);
                $("#statesPanel").css("left", 728 + shift);
            }
        });
        $("#lowerRightVerticalDivider").draggable({
            axis: "x",
            drag: function (event, ui) {
                shift = ui.position.left - 1170;
                dividerDistance = ui.position.left - parseInt($("#lowerLeftVerticalDivider").css("left"));
                $("#statesPanel").width(dividerDistance - 52);
                $("#portsPanel").css("left", 1176 + shift);
            }
        });
        $("#rightHorizontalDivider").draggable({
            axis: "y",
            drag: function (event, ui) {
                shift = ui.position.top - 587;
                $("#actionsPanel").height(548 + shift);
                $("#operatorsPanel").css("top", 598 + shift);
            }
        });
        $(".menuItem").draggable({
            helper: "clone",
            tolerance: "fit",
            revert: true,
            scroll: false
        });
        $(".menuItem").on("mousedown", function () {
            blockPanel = $("#blockPanel");
            rightScreenEdge = blockPanel.position().left + blockPanel.width();
            bottomScreenEdge = blockPanel.position().top + blockPanel.height();
            $(this).draggable("option", "containment", [0, 0, rightScreenEdge - $(this).width() - 20, bottomScreenEdge - $(this).height() - 20]);
        });
        $("#btnZoomIn").on("click", function () {
            ZoomFactor += 0.1;
            $("#rulesPanel .scrollArea").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        });
        $("#btnZoomOut").on("click", function () {
            if (ZoomFactor >= 0.2)
                ZoomFactor -= 0.1;
            $("#rulesPanel .scrollArea").css("transform", "scale(" + ZoomFactor + ")");
            $("#zoomLabel").text("Zoom " + Math.floor(ZoomFactor * 100) + "%");
        });
    }
});
