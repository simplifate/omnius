basicType = {
    connector: "StateMachine",
    paintStyle: { strokeStyle: "red", lineWidth: 2 },
    hoverPaintStyle: { strokeStyle: "blue" },
};
connectorPaintStyle = {
    lineWidth: 2,
    strokeStyle: "#467ea8",
    joinstyle: "round"
};
connectorHoverStyle = {
    lineWidth: 2,
    strokeStyle: "#dc555f"
};
endpointHoverStyle = {
    fillStyle: "#dc555f",
    strokeStyle: "#dc555f"
};
sourceEndpoint = {
    endpoint: "Rectangle",
    paintStyle: { fillStyle: "#f98e4b", width: 10, height: 18 },
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
    paintStyle: { fillStyle: "#f98e4b", width: 18, height: 10 },
    overlays: [
        ["Label", {
            location: [0.7, 2],
            label: "No",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
dataSourceConnectorPaintStyle = {
    lineWidth: 2,
    strokeStyle: "#467ea8",
    joinstyle: "round",
    "stroke-dasharray": "10, 5"
};
dataSourceEndpoint = $.extend({}, sourceEndpoint, {
    paintStyle: { fillStyle: "#f98e4b", width: 10, height: 18 },
    connectorStyle: dataSourceConnectorPaintStyle,
});

jsPlumb.ready(function () {
    if (CurrentModuleIs("tapestryModule")) {
        $("#rulesPanel .rule").each(function (ruleIndex, rule) {
            currentInstance = CreateJsPlumbInstanceForRule(rule);

            $(rule).find(".item").each(function (itemIndex, item) {
                AddIconToItem(item);
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
    }
});
