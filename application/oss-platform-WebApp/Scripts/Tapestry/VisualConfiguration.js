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
    paintStyle: { fillStyle: "#0070c0", width: 10, height: 18 },
    isSource: true,
    connector: ["Straight", { stub: [0, 0], gap: 10 }],
    connectorStyle: connectorPaintStyle,
    hoverPaintStyle: endpointHoverStyle,
    connectorHoverStyle: connectorHoverStyle,
    cssClass: "sourceEndpoint",
    dragOptions: {}
};

jsPlumb.ready(function () {
    $("#rulesPanel .rule").each(function (ruleIndex, rule) {
        currentInstance = CreateJsPlumbInstanceForRule(rule);

        $(rule).find(".item").each(function (itemIndex, item) {
            AddToJsPlumb(currentInstance, item);
        });
    });
});
