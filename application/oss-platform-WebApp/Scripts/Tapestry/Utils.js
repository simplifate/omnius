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
    $(ruleElement).data("jsPlumbInstance", newInstance);
    return newInstance;
}

function AddToJsPlumb(instance, item) {
    instance.draggable(item, { containment: "parent" });
    itemId = $(item).attr("id");

    instance.addEndpoint(itemId, sourceEndpoint, {
        anchor: "RightMiddle", uuid: itemId + "RightMiddle"
    });

    instance.makeTarget(item, {
        dropOptions: { hoverClass: "dragHover" },
        anchor: "LeftMiddle",
        allowLoopback: false
    });
}
