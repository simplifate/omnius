var LastAssignedOperatorNumber = 0;

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
    newInstance.setContainer($(ruleElement).attr("id"));
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
    if ($(item).hasClass("operatorSymbol")) {
        $(item).draggable({
            containment: "parent",
            drag: function () {
                instance.repaintEverything();
            }
        });
        if (!$(item).attr("id")) {
            itemId = AssingOperatorID();
            $(item).attr("id", itemId);
        }
        else {
            itemId = $(item).attr("id");
        }
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
        }
    }
    else {
        instance.draggable(item, { containment: "parent" });
        itemId = $(item).attr("id");
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
    }

}
function AssingOperatorID() {
    LastAssignedOperatorNumber++;
    return "operator" + LastAssignedOperatorNumber;
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
