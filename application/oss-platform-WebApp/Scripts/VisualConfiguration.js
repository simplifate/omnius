basicType = {
    connector: "StateMachine",
    paintStyle: { strokeStyle: "red", lineWidth: 4 },
    hoverPaintStyle: { strokeStyle: "blue" },
    overlays: [
        "Arrow"
    ]
};
connectorPaintStyle = {
    lineWidth: 4,
    strokeStyle: "#61B7CF",
    joinstyle: "round",
    outlineColor: "white",
    outlineWidth: 2
};
connectorHoverStyle = {
    lineWidth: 4,
    strokeStyle: "#216477",
    outlineWidth: 2,
    outlineColor: "white"
};
endpointHoverStyle = {
    fillStyle: "#216477",
    strokeStyle: "#216477"
};
sourceEndpoint = {
    endpoint: "Dot",
    paintStyle: { fillStyle: "#505050", radius: 11 },
    isSource: true,
    connector: ["Straight", { stub: [0, 0], gap: 10 }],
    connectorStyle: connectorPaintStyle,
    hoverPaintStyle: endpointHoverStyle,
    connectorHoverStyle: connectorHoverStyle,
    dragOptions: {}
};
targetEndpoint = {
    endpoint: "Dot",
    paintStyle: { fillStyle: "#7AB02C", radius: 11 },
    hoverPaintStyle: endpointHoverStyle,
    maxConnections: -1,
    dropOptions: { hoverClass: "hover", activeClass: "active" },
    isTarget: true
};

yesEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.5, 1.5],
            label: "Yes",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
noEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.5, 1.5],
            label: "No",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
OKEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.5, 1.5],
            label: "OK",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
TimeoutEndpoint = $.extend({}, sourceEndpoint, {
    overlays: [
        ["Label", {
            location: [0.5, 1.5],
            label: "Timeout",
            cssClass: "endpointSourceLabel"
        }]
    ]
});
