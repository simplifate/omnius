var ZoomFactor = 1.0;
$(function () {
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
        newRule = $('<div class="rule"><div class="ruleHeader"></div></div>');
        $("#rulesPanel .scrollArea").append(newRule);
        newRule.resizable();
        newRule.draggable({ handle: ".ruleHeader" });
        CreateJsPlumbInstanceForRule(newRule);
        newRule.droppable({
            accept: ".menuItem",
            drop: function (e, ui) {
                droppedElement = ui.helper.clone();
                ui.helper.remove();
                droppedElement.appendTo(this);
                leftOffset = ui.draggable.parent().offset().left - $(this).offset().left;
                topOffset = ui.draggable.parent().offset().top - $(this).offset().top;
                if (droppedElement.hasClass("operator")) {
                    if (droppedElement.attr("operatorType") == "decision")
                        newOperator = $('<div class="decisionRhombus operatorSymbol"><svg width="70" height="60">'
                          + '<polygon points="35,8 67,30 35,52 3,30" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /></svg></div>');
                    else if (droppedElement.attr("operatorType") == "condition")
                        newOperator = $('<div class="conditionEllipse operatorSymbol"><svg width="70" height="60">'
                          + '<ellipse cx="35" cy="30" rx="32" ry="20" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /><text x="17" y="39" fill="#2ddef9" font-size="25">if...</text></svg></div>');
                    newOperator.appendTo(this);
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
    $(".rule").resizable();
    $(".rule").draggable({ handle: ".ruleHeader" });
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
        revert: true
    });
    $.contextMenu({
        selector: '.rule .item, .rule .operatorSymbol',
        trigger: 'right',
        zIndex: 300,
        callback: function (key, options) {
            item = options.$trigger;
            if (key == "delete") {
                currentInstance = item.parents(".rule").data("jsPlumbInstance");
                currentInstance.removeAllEndpoints(item, true);
                item.remove();
            }
            else if (key == "edit") {
                switch (item.attr("dialogType")) {
                    case "emailTemplate":
                        CurrentItem = item;
                        chooseEmailTemplateDialog.dialog("open");
                        break;
                    case "port":
                        CurrentItem = item;
                        choosePortDialog.dialog("open");
                        break;
                    case "condition":
                        CurrentItem = item;
                        editConditionDialog.dialog("open");
                        break;
                    default:
                        alert("This item doesn't have any properties to edit.");
                        break;
                }
            }
        },
        items: {
            "edit": { name: "Edit", icon: "edit" },
            "sep1": "---------",
            "delete": { name: "Delete", icon: "delete" }
        }
    });
    $.contextMenu({
        selector: '.rule',
        trigger: 'right',
        zIndex: 300,
        callback: function (key, options) {
            if (key == "delete") {
                options.$trigger.remove();
            }
        },
        items: {
            "delete": { name: "Delete rule", icon: "delete" }
        }
    });
    $(".rule").droppable({
        accept: ".menuItem",
        containment: ".rule",
        drop: function (e, ui) {
            droppedElement = ui.helper.clone();
            ui.helper.remove();
            droppedElement.appendTo(this);
            leftOffset = ui.draggable.parent().offset().left - $(this).offset().left;
            topOffset = ui.draggable.parent().offset().top - $(this).offset().top;
            if (droppedElement.hasClass("operator")) {
                if (droppedElement.attr("operatorType") == "decision")
                    newOperator = $('<div class="decisionRhombus operatorSymbol"><svg width="70" height="60">'
                      + '<polygon points="35,8 67,30 35,52 3,30" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /></svg></div>');
                else if (droppedElement.attr("operatorType") == "condition")
                    newOperator = $('<div class="conditionEllipse operatorSymbol"><svg width="70" height="60">'
                      + '<ellipse cx="35" cy="30" rx="32" ry="20" style="fill:#467ea8; stroke:#467ea8; stroke-width:2;" /><text x="17" y="39" fill="#2ddef9" font-size="25">if...</text></svg></div>');
                newOperator.appendTo(this);
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
});
