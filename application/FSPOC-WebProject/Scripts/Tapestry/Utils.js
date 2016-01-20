var LastAssignedNumber = 0;

function CreateJsPlumbInstanceForRule(ruleElement) {
    newInstance = jsPlumb.getInstance({
        Endpoint: ["Blank", {}],
        HoverPaintStyle: { strokeStyle: "#ff4000", lineWidth: 2 },
        ConnectionOverlays: [
            ["Arrow", {
                location: 1,
                length: 12,
                width: 15,
                height: 12,
                foldback: 0.8,
            }]
        ]
    });
    ruleContent = $(ruleElement).find(".ruleContent");
    ruleContent.attr("id", AssingID());
    // newInstance.setContainer(ruleContent.attr("id"));
    newInstance.setContainer(ruleElement);
    newInstance.bind("click", function (con) {
        this.detach(con);
    });
    newInstance.bind("beforeDrop", function (info) {
        if (!$(info.connection.source).hasClass("dataSource")) {
            instance = $(info.connection.source).parents(".rule").data("jsPlumbInstance");
            connectionArray = instance.getConnections({ target: info.targetId });
            for (i = 0; i < connectionArray.length; i++) {
                if (!$(connectionArray[i].source).hasClass("dataSource")) {
                    alert("An item can take multiple data sources (dashed arrows), but only one activating action (solid arrow).\n"
                    + "There already is a solid arrow pointing to this item.");
                    return false;
                }
            }
        }
        return true;
    });
    $(ruleElement).data("jsPlumbInstance", newInstance);
    return newInstance;
}

function AddToJsPlumb(instance, item) {
    if (!$(item).attr("id")) {
        itemId = AssingID();
        $(item).attr("id", itemId);
    }
    else {
        itemId = $(item).attr("id");
    }
    $(item).draggable({
        drag: function (event, ui) {
            element = $(this);
            rule = element.parents(".rule");
            limits = CheckRuleResizeLimits(rule);

            if (ui.position.left < 10)
                ui.position.left = 10;
            else if (ui.position.left > limits.horizontal - element.width() - 40)
                ui.position.left = limits.horizontal - element.width() - 40;
            if (ui.position.top < 10)
                ui.position.top = 10;
            else if (ui.position.top > limits.vertical - element.height() - 60)
                ui.position.top = limits.vertical - element.height() - 60;

            rightEdge = ui.position.left + element.width() + element.data("rightOffset");
            bottomEdge = ui.position.top + element.height() + element.data("bottomOffset");

            if (rightEdge > rule.width()) {
                rule.width(rightEdge);
            }
            else if (rule.width() > limits.horizontal - 50)
                rule.width(limits.horizontal - 50);
            if (bottomEdge > rule.height()) {
                rule.height(bottomEdge);
            }
            rule.data("jsPlumbInstance").repaintEverything();
        },
        revert: "invalid",
        revertDuration: 0,
        stop: function () {
            $(this).draggable("option", "revert", "invalid");
            $(this).parents(".rule").data("jsPlumbInstance").repaintEverything();
        },
    });
    if ($(item).hasClass("operatorSymbol")) {
        if ($(item).hasClass("decisionRhombus")) {
            instance.addEndpoint(itemId, yesEndpoint, {
                anchor: "RightMiddle", uuid: itemId + "RightMiddle"
            });
            instance.addEndpoint(itemId, noEndpoint, {
                anchor: [0.5, 1, 0, 1, -5, 0], uuid: itemId + "BottomCenter"
            });
            instance.makeTarget(item, {
                dropOptions: { hoverClass: "dragHover" },
                anchor: "LeftMiddle",
                allowLoopback: false
            });
            $(item).data("topOffset", 10);
            $(item).data("rightOffset", -25);
            $(item).data("bottomOffset", -5);
        }
        else if ($(item).hasClass("conditionEllipse")) {
            instance.addEndpoint(itemId, sourceEndpoint, {
                anchor: "RightMiddle", uuid: itemId + "RightMiddle"
            });
            instance.makeTarget(item, {
                dropOptions: { hoverClass: "dragHover" },
                anchor: "Continuous",
                allowLoopback: false
            });
            $(item).data("topOffset", 10);
            $(item).data("rightOffset", -25);
            $(item).data("bottomOffset", -30);
        }
    }
    else {
        if ($(item).hasClass("dataSource"))
            instance.addEndpoint(itemId, dataSourceEndpoint, {
                anchor: "RightMiddle", uuid: itemId + "RightMiddle"
            });
        else
            instance.addEndpoint(itemId, sourceEndpoint, {
                anchor: "RightMiddle", uuid: itemId + "RightMiddle"
            });
        instance.makeTarget(item, {
            dropOptions: { hoverClass: "dragHover" },
            anchor: ["Continuous", { faces: ["left"] }],
            allowLoopback: false
        });
        $(item).data("topOffset", 0);
        $(item).data("rightOffset", -15);
        $(item).data("bottomOffset", 10);
    }

}
function AssingID() {
    LastAssignedNumber++;
    return "tapestryElement" + LastAssignedNumber;
}
function AddIconToItem(element) {
    item = $(element);
    if (item.hasClass("attribute")) {
        item.prepend($('<i class="fa fa-database" style="margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("port")) {
        item.prepend($('<i class="fa fa-sign-out" style="margin-left: 1px; margin-right: 5px;"></i>'));
    }
    else if (item.hasClass("role")) {
        item.prepend($('<i class="fa fa-user" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("view")) {
        item.prepend($('<i class="fa fa-paint-brush" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("action")) {
        item.prepend($('<i class="fa fa-cogs" style="margin-left: 1px; margin-right: 6px;"></i>'));
    }
    else if (item.hasClass("state")) {
        item.prepend($('<i class="fa fa-ellipsis-v" style="margin-left: 4px; margin-right: 8px;"></i>'));
    }
}
function CheckRuleResizeLimits(rule) {
    horizontalLimit = 1000000;
    verticalLimit = 1000000;

    ruleLeft = rule.position().left;
    ruleRight = ruleLeft + rule.width();
    ruleTop = rule.position().top - 40;
    ruleBottom = rule.position().top + rule.height();

    $("#rulesPanel .rule").each(function (index, element) {
        otherRule = $(element);
        if (otherRule.attr("id") != rule.attr("id")) {
            otherRuleLeft = otherRule.position().left;
            otherRuleRight = otherRuleLeft + otherRule.width();
            otherRuleTop = otherRule.position().top - 40;
            otherRuleBottom = otherRule.position().top + otherRule.height();

            if (otherRuleTop < ruleBottom && otherRuleBottom > ruleTop
                && otherRuleLeft > ruleRight && otherRuleLeft - ruleLeft < horizontalLimit)
                horizontalLimit = otherRuleLeft - ruleLeft;
            if (otherRuleLeft < ruleRight && otherRuleRight > ruleLeft
                && otherRuleTop > ruleBottom && otherRuleTop - ruleTop < verticalLimit)
                verticalLimit = otherRuleTop - ruleTop;
        }
    });
    return { horizontal: horizontalLimit, vertical: verticalLimit }
}