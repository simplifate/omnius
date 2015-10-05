basicType = {
    connector: "StateMachine",
    paintStyle: { strokeStyle: "red", lineWidth: 2 },
    hoverPaintStyle: { strokeStyle: "blue" },
};
connectorPaintStyle = {
    lineWidth: 2,
    strokeStyle: "#0070c0",
    joinstyle: "round"
};
connectorHoverStyle = {
    lineWidth: 2,
    strokeStyle: "#ff0000"
};
endpointHoverStyle = {
    fillStyle: "#000000",
    strokeStyle: "#000000"
};
sourceEndpoint = {
    endpoint: "Rectangle",
    paintStyle: { fillStyle: "#35acff", width: 10, height: 18 },
    isSource: true,
    connector: ["Straight", { stub: [0, 0], gap: 10 }],
    connectorStyle: connectorPaintStyle,
    hoverPaintStyle: endpointHoverStyle,
    connectorHoverStyle: connectorHoverStyle,
    cssClass: "sourceEndpoint",
    dragOptions: {}
};
yesEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.7, 1.5],
            label: "Yes",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
noEndpoint = $.extend({}, sourceEndpoint, {
    paintStyle: { fillStyle: "#35acff", width: 18, height: 10 },
    overlays: [
        ["Label", {
            location: [0.7, 2],
            label: "No",
            cssClass: "endpointSourceLabel"
        }]
    ]
});

jsPlumb.ready(function () {
    $("#rulesPanel .rule").each(function (ruleIndex, rule) {
        currentInstance = CreateJsPlumbInstanceForRule(rule);

        $(rule).find(".item").each(function (itemIndex, item) {
            AddToJsPlumb(currentInstance, item);
        });
        $(rule).find(".decisionRhombus").each(function (itemIndex, item) {
            AddToJsPlumb(currentInstance, item);
        });
    });
    $("#rule1").data("jsPlumbInstance").connect({ uuids: ["item1RightMiddle"], target: "item2", editable: true });
    $("#rule2").data("jsPlumbInstance").connect({ uuids: ["item3RightMiddle"], target: "item4", editable: true });
    $("#rule3").data("jsPlumbInstance").connect({ uuids: ["item5RightMiddle"], target: "item6", editable: true });
    $("#rule3").data("jsPlumbInstance").connect({ uuids: ["item6RightMiddle"], target: "decision1", editable: true });
    $("#rule3").data("jsPlumbInstance").connect({ uuids: ["decision1RightMiddle"], target: "item7", editable: true });
    $("#rule3").data("jsPlumbInstance").connect({ uuids: ["decision1BottomCenter"], target: "item8", editable: true });
});
